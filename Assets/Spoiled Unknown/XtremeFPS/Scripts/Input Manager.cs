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
    public class XtremeFPSInputHandler : MonoBehaviour
    {
        public static XtremeFPSInputHandler Instance {  get; private set; }

        #region Variables
        private PlayerInputAction playerInputAction;

        public int maxTouchLimit = 10;
        public TouchDetectMode touchDetectionMode;

        [HideInInspector] public Vector2 moveDirection;
        [HideInInspector] public Vector2 mouseDirection;

        [HideInInspector] public bool isSprintingHold;
        [HideInInspector] public bool isSprintingTapped;

        [HideInInspector] public bool isCrouchingHold;
        [HideInInspector] public bool isCrouchingTapped;

        [HideInInspector] public bool isJumping;

        [HideInInspector] public bool isZoomingHold;
        [HideInInspector] public bool isZoomingTapped;

        [HideInInspector] public bool isFiringHold;
        [HideInInspector] public bool isFiringTapped;

        [HideInInspector] public bool isReloading;

        [HideInInspector] public bool isAimingHold;
        [HideInInspector] public bool isAimingTapped;

        [HideInInspector] public bool isTryingToInteract;
        [HideInInspector] public bool isTryingToInteractAlternate;

        [HideInInspector] public float MouseScroll;

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
            isJumping = context.ReadValueAsButton();
        }

        private void InteractionInput(InputAction.CallbackContext context)
        {
            isTryingToInteract = context.ReadValueAsButton();
        }
        private void InteractionAltInput(InputAction.CallbackContext context)
        {
            isTryingToInteractAlternate = context.ReadValueAsButton();
        }
        #endregion

        #region Weapon Inputs
        private void ShootHoldInput(InputAction.CallbackContext context)
        {
            isFiringHold = context.ReadValueAsButton();
        }
        private void ShootTapInput(InputAction.CallbackContext context)
        {
            isFiringTapped = context.ReadValueAsButton();
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

        private void ScrollInput(InputAction.CallbackContext context)
        {
            MouseScroll = context.ReadValue<float>();
        }
        #endregion
    }
}