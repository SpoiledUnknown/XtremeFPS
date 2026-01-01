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

        [HideInInspector] public bool isTryingToSwitchCamera;
        [HideInInspector] public bool isUsingTouchscreen;

        //only for demo purpose, please remove in production
        public bool escape;
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

            playerInputAction.Player.CrouchTap.started += CrouchTapInput;
            playerInputAction.Player.CrouchTap.performed += CrouchTapInput;
            playerInputAction.Player.CrouchTap.canceled += CrouchTapInput;
            
            playerInputAction.Player.SprintTap.started += SprintTapInput;
            playerInputAction.Player.SprintTap.performed += SprintTapInput;
            playerInputAction.Player.SprintTap.canceled += SprintTapInput;
            
            playerInputAction.Player.ZoomTap.started += ZoomTapInput;
            playerInputAction.Player.ZoomTap.performed += ZoomTapInput;
            playerInputAction.Player.ZoomTap.canceled += ZoomTapInput;
            
            playerInputAction.Player.CameraSwitch.performed += CameraSwitchInput;

            playerInputAction.Player.Interaction.started += InteractionInput;
            playerInputAction.Player.Interaction.performed += InteractionInput;
            playerInputAction.Player.Interaction.canceled += InteractionInput;
            #endregion

            #region Weapon System
            playerInputAction.Weapon.FireHold.started += ShootHoldInput;
            playerInputAction.Weapon.FireHold.performed += ShootHoldInput;
            playerInputAction.Weapon.FireHold.canceled += ShootHoldInput;
                              
            playerInputAction.Weapon.FireTap.started += ShootTapInput;
            playerInputAction.Weapon.FireTap.performed += ShootTapInput;
            playerInputAction.Weapon.FireTap.canceled += ShootTapInput;
                              
            playerInputAction.Weapon.Reload.performed += ReloadingInput;
            playerInputAction.Weapon.Reload.canceled += ReloadingInput;
                              
            playerInputAction.Weapon.ADSHold.performed += ADSHoldInput;
            playerInputAction.Weapon.ADSHold.canceled += ADSHoldInput;
                              
            playerInputAction.Weapon.ADSTap.performed += ADSTapInput;
                              
            playerInputAction.Weapon.WeaponScroll.performed += ScrollInput;
            playerInputAction.Weapon.WeaponScroll.canceled += ScrollInput;
            #endregion

            //only for demo purpose, please remove in production
            playerInputAction.Demo.PauseMenu.started += (context) =>
            {
                escape = context.ReadValueAsButton();
            };

            playerInputAction.Demo.PauseMenu.canceled += (context) =>
            {
                escape = context.ReadValueAsButton();
            };
        }
        #endregion
        
        bool IsPointerOverUI(TouchControl touch)
        {
            if (EventSystem.current == null) return false;
            return EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue());
        }

        #region Player Inputs
        private void MouseInput(InputAction.CallbackContext context)
        {
            //mouseDirection = context.ReadValue<Vector2>();
            
            Vector2 delta = context.ReadValue<Vector2>();

            // Ignore touches that started over UI
            if (context.control.device is Touchscreen touchscreen)
            {
                foreach (var touch in touchscreen.touches)
                {
                    if (IsPointerOverUI(touch))
                        return;
                }
            }

            mouseDirection += delta;
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

        private void CameraSwitchInput(InputAction.CallbackContext obj)
        {
            isTryingToSwitchCamera = !isTryingToSwitchCamera;
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