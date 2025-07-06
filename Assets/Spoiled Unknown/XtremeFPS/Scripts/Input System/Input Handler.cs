using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace XtremeFPS.InputHandling
{
    public class XtremeFPSInputHandler : MonoBehaviour
    {
        public static XtremeFPSInputHandler Instance {  get; private set; }

        #region Variables
        private PlayerInputAction playerInputAction;

        [HideInInspector] public Vector2 moveDirection;
        [HideInInspector] public Vector2 mouseDirection;
        [HideInInspector] public float mouseScrollDirection;

        [HideInInspector] public bool isSprintHold;
        [HideInInspector] public bool isSprintTap;

        [HideInInspector] public bool isCrouchHold;
        [HideInInspector] public bool isCrouchTap;

        [HideInInspector] public bool isTryingToJump;

        [HideInInspector] public bool isZoomHold;
        [HideInInspector] public bool isZoomTap;

        [HideInInspector] public bool isShootHold;
        [HideInInspector] public bool isShootTap;

        [HideInInspector] public bool isTryingToReload;

        [HideInInspector] public bool isAimHold;
        [HideInInspector] public bool isAimTap;

        [HideInInspector] public bool isTryingToInteract;
        [HideInInspector] public bool isTryingToInteractAlternate;

        [HideInInspector] public bool IsTryingToSwitchCamera;

        #region Touch Controls
#if UNITY_ANDROID || UNITY_IOS
        public int maxTouchLimit = 10;
        public TouchDetectMode touchDetectionMode;

        public enum TouchDetectMode
        {
            FirstTouch,
            LastTouch,
            All
        }
        private Func<TouchControl, bool> isTouchAvailable;                        // Delegate takes parameter touch and return true if touch is the available touch for camera rotation
        private List<string> availableTouchIds = new List<string>();     // Get all the touches that began without colliding with any UI Image/Button
        private EventSystem eventStytem;
#endif
        #endregion
        #endregion

        #region Initialization
        private void Awake()
        {
            if (Instance != null) Destroy(Instance);
            else Instance = this;

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
            playerInputAction.Player.Jump.started += JumpInput;
            playerInputAction.Player.Jump.performed += JumpInput;
            playerInputAction.Player.Jump.canceled += JumpInput;

            playerInputAction.Player.Movements.performed += MoveInput;
            playerInputAction.Player.Movements.canceled += MoveInput;

            playerInputAction.Player.Look.performed += MouseInput;
            playerInputAction.Player.Look.canceled += MouseInput;

            playerInputAction.Player.CrouchHold.performed += CrouchHoldInput;
            playerInputAction.Player.CrouchHold.canceled += CrouchHoldInput;

            playerInputAction.Player.SprintHold.performed += SprintHoldInput;
            playerInputAction.Player.SprintHold.canceled += SprintHoldInput;

            playerInputAction.Player.ZoomHold.performed += ZoomHoldInput;
            playerInputAction.Player.ZoomHold.canceled += ZoomHoldInput;

            playerInputAction.Player.CrouchTap.performed += CrouchTapInput;
            playerInputAction.Player.SprintTap.performed += SprintTapInput;
            playerInputAction.Player.ZoomTap.performed += ZoomTapInput;
            playerInputAction.Player.CameraSwitch.performed += CameraSwitchInput;

            playerInputAction.Player.Interaction.started += InteractionInput;
            playerInputAction.Player.Interaction.performed += InteractionInput;
            playerInputAction.Player.Interaction.canceled += InteractionInput;

            playerInputAction.Player.InteractionAlt.started += InteractionAltInput;
            playerInputAction.Player.InteractionAlt.performed += InteractionAltInput;
            playerInputAction.Player.InteractionAlt.canceled += InteractionAltInput;
            #endregion

            #region Weapon System
            playerInputAction.Weapon.FireHold.performed += ShootHoldInput;
            playerInputAction.Weapon.FireHold.canceled += ShootHoldInput;
                              
            playerInputAction.Weapon.FireTap.started += ShootTapInput;
            playerInputAction.Weapon.FireTap.performed += ShootTapInput;
            playerInputAction.Weapon.FireTap.canceled += ShootTapInput;
                              
            playerInputAction.Weapon.Reload.performed += ReloadingInput;
            playerInputAction.Weapon.Reload.canceled += ReloadingInput;
                              
            playerInputAction.Weapon.ADSHold.canceled += ADSHoldInput;
            playerInputAction.Weapon.ADSHold.performed += ADSHoldInput;
                              
            playerInputAction.Weapon.ADSTap.performed += ADSTapInput;
                              
            playerInputAction.Weapon.WeaponScroll.performed += ScrollInput;
            playerInputAction.Weapon.WeaponScroll.canceled += ScrollInput;
            #endregion

#if UNITY_ANDROID || UNITY_IOS
            if (EventSystem.current != null) eventStytem = EventSystem.current;
            else Debug.LogError("Scene has no Event System!");
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

        private void MoveInput(InputAction.CallbackContext context)
        {
            moveDirection = context.ReadValue<Vector2>();
        }

        private void CrouchHoldInput(InputAction.CallbackContext context)
        {
            isCrouchHold = context.ReadValueAsButton();
        }
        private void CrouchTapInput(InputAction.CallbackContext context)
        {
            isCrouchTap = !isCrouchTap;
        }

        private void SprintHoldInput(InputAction.CallbackContext context)
        {
            isSprintHold = context.ReadValueAsButton();
        }
        private void SprintTapInput(InputAction.CallbackContext context)
        {
            isSprintTap = !isSprintTap;
        }

        private void ZoomHoldInput(InputAction.CallbackContext context)
        {
            isZoomHold = context.ReadValueAsButton();
        }
        private void ZoomTapInput(InputAction.CallbackContext context)
        {
            isZoomTap = !isZoomTap;
        }

        private void JumpInput(InputAction.CallbackContext context)
        {
            isTryingToJump = context.ReadValueAsButton();
        }

        private void InteractionInput(InputAction.CallbackContext context)
        {
            isTryingToInteract = context.ReadValueAsButton();
        }
        private void InteractionAltInput(InputAction.CallbackContext context)
        {
            isTryingToInteractAlternate = context.ReadValueAsButton();
        }

        private void CameraSwitchInput(InputAction.CallbackContext obj)
        {
            IsTryingToSwitchCamera = !IsTryingToSwitchCamera;
        }
        #endregion

        #region Weapon Inputs
        private void ShootHoldInput(InputAction.CallbackContext context)
        {
            isShootHold = context.ReadValueAsButton();
        }
        private void ShootTapInput(InputAction.CallbackContext context)
        {
            isShootTap = context.ReadValueAsButton();
        }

        private void ReloadingInput(InputAction.CallbackContext context)
        {
            isTryingToReload = context.ReadValueAsButton();
        }

        private void ADSHoldInput(InputAction.CallbackContext context)
        {
            isAimHold = context.ReadValueAsButton();
        }
        private void ADSTapInput(InputAction.CallbackContext context)
        {
            isAimTap = !isAimTap;
        }

        private void ScrollInput(InputAction.CallbackContext context)
        {
            mouseScrollDirection = context.ReadValue<float>();
        }
        #endregion
    }
}