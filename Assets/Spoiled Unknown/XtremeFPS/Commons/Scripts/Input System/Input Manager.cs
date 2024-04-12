/*Copyright � Non-Dynamic Studio*/
/*2023*/

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;

namespace XtremeFPS.Common.InputSystem
{
    public class FPSInputManager : MonoBehaviour
    {
        #region Variables

        [HideInInspector] private PlayerInputAction playerInputAction;
        //sprinting
        [HideInInspector] public bool isSprintingHold;
        [HideInInspector] public bool isSprintingTap;
        //crouching
        [HideInInspector] public bool isCrouchingTap;
        [HideInInspector] public bool isCrouchingHold;
        //Vectors
        [HideInInspector] public Vector2 moveDirection;
        [HideInInspector] public Vector2 mouseDirection;
        //Jump
        [HideInInspector] public bool haveJumped;
        //Zooming
        [HideInInspector] public bool isZoomingHold;
        [HideInInspector] public bool isZoomingTap;
        //Weapon
        [HideInInspector] public bool isFiringHold;
        [HideInInspector] public bool isFiringTap;
        [HideInInspector] public bool isReloading;
        [HideInInspector] public bool isAimingTap;
        [HideInInspector] public bool isAimingHold;
        //Peeking
        [HideInInspector] public Vector2 peekDirection;

        #endregion
        #region Initialization
        private void Awake()
        {
            playerInputAction = new PlayerInputAction();
        }
        private void OnEnable()
        {
            playerInputAction.Enable();
        }
        private void OnDisable()
        {
            playerInputAction.Disable();
        }
        private void Start()
        {
            #region Player Movement
            // Subscribe to input performing events
            playerInputAction.Player.Movements.performed += MovementInput;
            playerInputAction.Player.Look.performed += MouseInput;
            playerInputAction.Player.CrouchHold.performed += CrouchHoldInput;
            playerInputAction.Player.CrouchTap.performed += CrouchTapInput;
            playerInputAction.Player.SprintHold.performed += SprintHoldInput;
            playerInputAction.Player.SprintTap.performed += SprintTapInput;
            playerInputAction.Player.Jump.performed += JumpInput;
            playerInputAction.Player.ZoomHold.performed += ZoomHoldInput;
            playerInputAction.Player.ZoomTap.performed += ZoomTapInput;


            // Subscribe to input cancellation events 
            playerInputAction.Player.Movements.canceled += MovementInput;
            playerInputAction.Player.Look.canceled += MouseInput;
            playerInputAction.Player.CrouchHold.canceled += CrouchHoldInput;
            playerInputAction.Player.SprintHold.canceled += SprintHoldInput;
            playerInputAction.Player.ZoomHold.canceled += ZoomHoldInput;




            #endregion
            #region Weapon System
            playerInputAction.Shooting.FireHold.performed += ShootInput;
            playerInputAction.Shooting.FireTap.performed += ShootTapInput;
            playerInputAction.Shooting.Reload.performed += ReloadingInput;
            playerInputAction.Shooting.ADSTap.performed += ADSTapInput;
            playerInputAction.Shooting.ADSHold.performed += ADSHoldInput;
            playerInputAction.Shooting.Peek.performed += PeekInput;


            playerInputAction.Shooting.Reload.canceled += ReloadingInput;
            playerInputAction.Shooting.FireHold.canceled += ShootInput;
            playerInputAction.Shooting.ADSHold.canceled += ADSHoldInput;
            playerInputAction.Shooting.Peek.canceled += PeekInput;


            #endregion
        }
#if UNITY_ANDROID || UNITY_IOS
        private void Update()
        {
            if (Touchscreen.current == null || Touchscreen.current.touches.Count == 0) return;

            if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[0].touchId.ReadValue()))
            {
                if (Touchscreen.current.touches.Count > 1 && Touchscreen.current.touches[1].isInProgress)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[1].touchId.ReadValue())) return;

                    mouseDirection = Touchscreen.current.touches[1].delta.ReadValue();
                }
                else
                {
                    mouseDirection = Vector2.zero;
                }
            }
            else
            {
                if (Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].isInProgress)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[0].touchId.ReadValue())) return;

                    mouseDirection = Touchscreen.current.touches[0].delta.ReadValue();
                }
                else
                {
                    mouseDirection = Vector2.zero;
                }
            }
        }
#endif
#endregion


        #region Input Handling
        private void MouseInput(InputAction.CallbackContext context)
        {
            mouseDirection = context.ReadValue<Vector2>();
        }
        private void MovementInput(InputAction.CallbackContext context)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        private void CrouchHoldInput(InputAction.CallbackContext context)
        {
            isCrouchingHold = context.ReadValueAsButton();
        }
        private void CrouchTapInput(InputAction.CallbackContext context)
        {
            if(isCrouchingTap)
            {
                isCrouchingTap = false;
            }
            else
            {
                isCrouchingTap = true;
            }
        }
        private void SprintHoldInput(InputAction.CallbackContext context)
        {
            isSprintingHold = context.ReadValueAsButton();
        }
        private void SprintTapInput(InputAction.CallbackContext context)
        {
            if(isSprintingTap)
            {
                isSprintingTap = false;
            }
            else
            {
                isSprintingTap = true;
            }
        }
        private void ZoomHoldInput(InputAction.CallbackContext context)
        {
            isZoomingHold = context.ReadValueAsButton();
        }
        private void ZoomTapInput(InputAction.CallbackContext context)
        {
            if(isZoomingTap)
            {
                isZoomingTap = false;
            }
            else
            {
                isZoomingTap = true;
            }
        }
        private void JumpInput(InputAction.CallbackContext context)
        {
            if (haveJumped) return;
            haveJumped = true;
            StartCoroutine(CancelJump());

        }
        IEnumerator CancelJump()
        {
            yield return new WaitForSeconds(0.05f);
            haveJumped = false;
        }
        #endregion
        #region Shooting
        private void ShootInput(InputAction.CallbackContext context)
        {
            isFiringHold = context.ReadValueAsButton();
        }
        private void ReloadingInput(InputAction.CallbackContext context)
        {
            isReloading = context.ReadValueAsButton();
        }
        private void ADSHoldInput(InputAction.CallbackContext context)
        {
            isAimingHold = context.ReadValueAsButton();
        }
        private void ADSTapInput(InputAction.CallbackContext context)
        {
            if (isAimingTap)
            {
                isAimingTap = false;
            }
            else
            {
                isAimingTap = true;
            }
        }
        private void ShootTapInput(InputAction.CallbackContext context)
        {
            if (isFiringTap) return;
            isFiringTap = true;
            StartCoroutine(CancelFire());
        }
        IEnumerator CancelFire()
        {
            yield return new WaitForSeconds(0.05f);
            isFiringTap = false;
        }

        private void PeekInput(InputAction.CallbackContext context)
        {
            peekDirection = context.ReadValue<Vector2>();
        }
        #endregion
    }
}

