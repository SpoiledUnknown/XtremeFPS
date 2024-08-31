/*Copyright © Spoiled Unknown*/
/*2024*/

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace XtremeFPS.InputHandling
{
    public class FPSInputManager : MonoBehaviour
    {
        public static FPSInputManager Instance {  get; private set; }

        #region Variables
        public int maxTouchLimit = 10;
        public TouchDetectMode touchDetectionMode;

        [HideInInspector] private PlayerInputAction playerInputAction;

        //sprinting
        [HideInInspector] public bool isSprintingHold;
        [HideInInspector] public bool isSprintingTapped;

        //crouching
        [HideInInspector] public bool isCrouchingHold;
        [HideInInspector] public bool isCrouchingTapped;

        //Vectors
        [HideInInspector] public Vector2 moveDirection;
         public Vector2 mouseDirection;

        //Jump
        [HideInInspector] public bool haveJumped;

        //Zooming
        [HideInInspector] public bool isZoomingHold;
        [HideInInspector] public bool isZoomingTapped;

        //Weapon
        [HideInInspector] public bool isFiringHold;
        [HideInInspector] public bool isFiringTapped;
        [HideInInspector] public bool isReloading;
        [HideInInspector] public bool isAimingHold;
        [HideInInspector] public bool isAimingTapped;
        [HideInInspector] public bool isTryingToInteract;

        #region Touch Controls
        public enum TouchDetectMode
        {
            FirstTouch,
            LastTouch,
            All
        }
        private Func<TouchControl, bool> isTouchAvailable;                        // Delegate takes parameter touch and return true if touch is the available touch for camera rotation
        private List<string> availableTouchIds = new List<string>();     // Get all the touches that began without colliding with any UI Image/Button
        private EventSystem eventStytem;
        #endregion
        #endregion

        #region Initialization
        private void Awake()
        {
            playerInputAction = new PlayerInputAction();

            if (Instance != null) Destroy(Instance);
            else Instance = this;
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
            playerInputAction.Player.Interaction.started += Interaction_performed;

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

            playerInputAction.Shooting.Reload.canceled += ReloadingInput;
            playerInputAction.Shooting.FireHold.canceled += ShootInput;
            playerInputAction.Shooting.ADSHold.canceled += ADSHoldInput;

            #endregion

#if UNITY_ANDROID || UNITY_IOS
            if (EventSystem.current != null) eventStytem = EventSystem.current;
            else Debug.LogError($"Scene has no Event System!");
            SetIsTouchDelegate();
#endif
        }

#if UNITY_ANDROID || UNITY_IOS
        private void Update()
        {
            // Check for touch input
            if (Touchscreen.current == null || Touchscreen.current.touches.Count == 0) return;

            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                // Handle touch input
                if ((touch.phase.value == TouchPhase.Began && eventStytem != null) &&
                    !eventStytem.IsPointerOverGameObject(touch.touchId.ReadValue()) &&
                    availableTouchIds.Count <= maxTouchLimit)
                {
                    availableTouchIds.Add(touch.touchId.ReadValue().ToString());
                }

                if (availableTouchIds.Count == 0) continue;

                if (isTouchAvailable(touch))
                {
                    mouseDirection += new Vector2(touch.delta.x.value, touch.delta.y.value);
                    if (touch.phase.value == TouchPhase.Ended) availableTouchIds.RemoveAt(0);
                }
                else if (touch.phase.value == TouchPhase.Ended)
                {
                    availableTouchIds.Remove(touch.touchId.ReadValue().ToString());
                }
            }
        }

        public void SetIsTouchDelegate()
        {
            switch (touchDetectionMode)
            {
                case TouchDetectMode.FirstTouch:
                    isTouchAvailable = (TouchControl touch) => { return touch.touchId.ReadValue().ToString() == availableTouchIds[0]; };
                    break;
                case TouchDetectMode.LastTouch:
                    isTouchAvailable = (TouchControl touch) => { return touch.touchId.ReadValue().ToString() == availableTouchIds[availableTouchIds.Count - 1]; };
                    break;
                case TouchDetectMode.All:
                    isTouchAvailable = (TouchControl touch) => { return availableTouchIds.Contains(touch.touchId.ReadValue().ToString()); };
                    break;
            }
        }
#endif

        #endregion

        #region Player Inputs
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
            isCrouchingTapped = !isCrouchingTapped;
        }
        private void SprintHoldInput(InputAction.CallbackContext context)
        {
            isSprintingHold = context.ReadValueAsButton();
        }
        private void SprintTapInput(InputAction.CallbackContext context)
        {
            isSprintingTapped = !isSprintingTapped;
        }
        private void ZoomHoldInput(InputAction.CallbackContext context)
        {
            isZoomingHold = context.ReadValueAsButton();
        }
        private void ZoomTapInput(InputAction.CallbackContext context)
        {
            isZoomingTapped = !isZoomingTapped;
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

        #region Weapon Inputs
        private void ShootInput(InputAction.CallbackContext context)
        {
            isFiringHold = context.ReadValueAsButton();
        }

        private void ShootTapInput(InputAction.CallbackContext context)
        {
            if (isFiringTapped) return;
            isFiringTapped = true;
            StartCoroutine(CancelFire());
        }
        IEnumerator CancelFire()
        {
            yield return new WaitForSeconds(0.05f);
            isFiringTapped = false;
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
            isAimingTapped = !isAimingTapped;
        }

        private void Interaction_performed(InputAction.CallbackContext obj)
        {
            isTryingToInteract = true;
            Invoke(nameof(SetIsTryingToInteractToFalse), 0.001f);
        }

        private void SetIsTryingToInteractToFalse()
        {
            isTryingToInteract = false;
        }
        #endregion
    }
}