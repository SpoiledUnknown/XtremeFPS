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
        public Vector2 MoveDirection {  get; private set; }
        public Vector2 MouseDirection {  get; private set; }
        public float MouseScrollDirection {  get; private set; }
        public bool IsSprintHold {  get; private set; }
        public bool IsSprintTap {  get; private set; }
        public bool IsCrouchHold {  get; private set; }
        public bool IsCrouchTap {  get; private set; }
        public bool IsTryingToJump {  get; private set; }
        public bool IsZoomHold {  get; private set; }
        public bool IsZoomTap {  get; private set; }
        public bool IsShootHold {  get; private set; }
        public bool IsShootTap {  get; private set; }
        public bool IsTryingToReload {  get; private set; }
        public bool IsAimHold {  get; private set; }
        public bool IsAimTap {  get; private set; }
        public bool IsTryingToInteract {  get; private set; }
        public bool IsTryingToSwitchCamera {  get; private set; }
        public bool IsUsingTouchscreen {  get; private set; }

        //only for demo purpose, please remove in production
        public bool Escape {  get; private set; }
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
                              
            playerInputAction.Weapon.ADSHold.performed += AdsHoldInput;
            playerInputAction.Weapon.ADSHold.canceled += AdsHoldInput;
                              
            playerInputAction.Weapon.ADSTap.performed += AdsTapInput;
                              
            playerInputAction.Weapon.WeaponScroll.performed += ScrollInput;
            playerInputAction.Weapon.WeaponScroll.canceled += ScrollInput;
            
            playerInputAction.Weapon.WeaponNext.started += ControllerScrollNextInput;
            playerInputAction.Weapon.WeaponNext.canceled += ControllerScrollNextInput;
            
            playerInputAction.Weapon.WeaponPrevious.started += ControllerScrollPreviousInput;
            playerInputAction.Weapon.WeaponPrevious.canceled += ControllerScrollPreviousInput;
            #endregion

            //only for demo purpose, please remove in production
            playerInputAction.Demo.PauseMenu.started += (context) =>
            {
                Escape = context.ReadValueAsButton();
            };

            playerInputAction.Demo.PauseMenu.canceled += (context) =>
            {
                Escape = context.ReadValueAsButton();
            };
        }
        #endregion

        #region Player Inputs
        
        private void MouseInput(InputAction.CallbackContext context)
        {
            if (context.control.device is Touchscreen touchscreen)
            {
                IsUsingTouchscreen = true;
                
                Vector2 delta = context.ReadValue<Vector2>();
                
                foreach (var touch in touchscreen.touches)
                {
                    if (EventSystem.current != null &&
                        EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue())) return;
                }
                MouseDirection += delta;
            }
            else
            {
                IsUsingTouchscreen = false;
                MouseDirection = context.ReadValue<Vector2>();
            }
        }

        public void ResetMouseDirection()
        {
            MouseDirection = Vector2.zero;
        }

        private void MoveInput(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
        }

        private void CrouchHoldInput(InputAction.CallbackContext context)
        {
            IsCrouchHold = context.ReadValueAsButton();
        }
        
        private void CrouchTapInput(InputAction.CallbackContext context)
        {
            IsCrouchTap = !IsCrouchTap;
        }

        private void SprintHoldInput(InputAction.CallbackContext context)
        {
            IsSprintHold = context.ReadValueAsButton();
        }
        
        private void SprintTapInput(InputAction.CallbackContext context)
        {
            IsSprintTap = !IsSprintTap;
        }

        private void ZoomHoldInput(InputAction.CallbackContext context)
        {
            IsZoomHold = context.ReadValueAsButton();
        }
        
        private void ZoomTapInput(InputAction.CallbackContext context)
        {
            IsZoomTap = !IsZoomTap;
        }

        private void JumpInput(InputAction.CallbackContext context)
        {
            IsTryingToJump = context.ReadValueAsButton();
        }

        private void InteractionInput(InputAction.CallbackContext context)
        {
            IsTryingToInteract = context.ReadValueAsButton();
        }

        private void CameraSwitchInput(InputAction.CallbackContext obj)
        {
            IsTryingToSwitchCamera = !IsTryingToSwitchCamera;
        }
        #endregion

        #region Weapon Inputs
        private void ShootHoldInput(InputAction.CallbackContext context)
        {
            IsShootHold = context.ReadValueAsButton();
        }
        
        private void ShootTapInput(InputAction.CallbackContext context)
        {
            IsShootTap = context.ReadValueAsButton();
        }

        private void ReloadingInput(InputAction.CallbackContext context)
        {
            IsTryingToReload = context.ReadValueAsButton();
        }

        private void AdsHoldInput(InputAction.CallbackContext context)
        {
            IsAimHold = context.ReadValueAsButton();
        }
        
        private void AdsTapInput(InputAction.CallbackContext context)
        {
            IsAimTap = !IsAimTap;
        }

        private void ScrollInput(InputAction.CallbackContext context)
        {
            MouseScrollDirection = context.ReadValue<float>();
        }
        
        private void ControllerScrollNextInput(InputAction.CallbackContext context)
        {
            MouseScrollDirection = context.ReadValueAsButton() ? 1 : 0;
        }
        
        private void ControllerScrollPreviousInput(InputAction.CallbackContext context)
        {
            MouseScrollDirection = context.ReadValueAsButton() ? -1 : 0;
        }
        #endregion
    }
}