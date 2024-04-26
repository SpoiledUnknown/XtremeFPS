/*Copyright � Spoiled Unknown*/
/*2024*/

using System.Collections;
using UnityEngine;
using XtremeFPS.InputHandler;
using Cinemachine;
using UnityEngine.UI;

namespace XtremeFPS.FirstPersonController
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(FPSInputManager))]
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/First Person Controller")]
    public class FirstPersonController : MonoBehaviour
    {
        #region Variables
        // Player
        public CharacterController characterController;
        public bool canPlayerMove;
        public float transitionSpeed;
        public float walkSpeed = 5f;
        public float walkSoundSpeed;

        private FPSInputManager inputManager;
        private enum PlayerMovementState
        {
            Sprinting,
            Crouching,
            Walking,
            Default
        }


        //sprinting
        public bool canPlayerSprint;
        public bool unlimitedSprinting;
        public bool isSprintHold;
        public float sprintSpeed = 8f;
        public float sprintDuration = 8f;
        public float sprintCooldown = 8f;
        public bool hasStaminaBar;
        public Slider staminaSlider;
        public float sprintSoundSpeed;

        private float targetSpeed;
        private float transitionDelta;
        private Vector3 moveDirection;
        private bool isSprinting;
        private bool isSprintCooldown = false;
        private readonly float sprintCooldownReset;
        private float sprintRemaining;


        // Gravity and Jumping
        public bool canJump;
        public float jumpHeight = 2f;
        public float gravitationalForce = 10f;
        public bool IsGrounded { get; private set; }
        public Vector3 jumpVelocity;

        private bool havePreviouslyJumped;


        // Crouching
        public bool canPlayerCrouch;
        public bool isCrouchHold;
        public float crouchedHeight = 1f;
        public float crouchedSpeed = 1f;
        public float crouchSoundPlayTime;

        private bool isCrouching;
        private float newHeight;
        private float initialHeight;
        private bool isTryingToUncrouch;
        private Vector3 initialCameraPosition;


        // Camera
        public bool isCursorLocked;
        public Transform cameraFollow;
        public CinemachineVirtualCamera playerVirtualCamera;
        public float mouseSensitivity;
        public float maximumClamp;
        public float minimumClamp;
        public float sprintFOV;
        public float FOV;

        private float rotationY;


        //Zooming
        public bool enableZoom;
        public bool isZoomingHold;
        public float zoomFOV = 30f;

        private bool isZoomed = false;


        //Head Bobbing effect
        public bool canHeadBob;
        public float headBobAmplitude = 0.01f;
        public float headBobFrequency = 18.5f;

        private Vector3 _startPos;

        //Sound System
        public bool canPlaySound;
        public AudioSource audioSource;
        public string grassTag;
        public AudioClip[] soundGrass;
        public string waterTag;
        public AudioClip[] soundWater;
        public string metalTag;
        public AudioClip[] soundMetal;
        public string concreteTag;
        public AudioClip[] soundConcrete;
        public string gravelTag;
        public AudioClip[] soundGravel;
        public string woodTag;
        public AudioClip[] soundWood;
        public AudioClip landClip;
        public AudioClip jumpClip;
        public float footstepSensitivity;

        private float AudioEffectSpeed;
        private bool moving = false;
        private string floortag;


        // Handling Physics
        public bool canPush;
        public int pushLayersID;
        public float pushStrength = 1.1f;

        private LayerMask pushLayers;


        //Recoil For Weapon System;
        public bool haveCameraRecoil = false;

        private float hRecoil = 0f;
        private float vRecoil = 0f;
        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            inputManager = FPSInputManager.instance;
            playerVirtualCamera.m_Lens.FieldOfView = FOV;
            AudioEffectSpeed = walkSoundSpeed;
            _startPos = cameraFollow.localPosition;

            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;

            // If canPush is true, get the layer mask from the pushLayersID
            if (canPush) pushLayers = LayerMaskFromLayer(pushLayersID);

            // Start the coroutine for senseSteps
            if (canPlaySound) StartCoroutine(SenseSteps());

            // If hasStaminaBar is true and unlimitedSprinting is true, deactivate the stamina slider
            if (hasStaminaBar && unlimitedSprinting) staminaSlider.gameObject.SetActive(false);

            initialHeight = characterController.height;
            if (!canPlayerCrouch) return;
            initialCameraPosition = cameraFollow.transform.localPosition;
        }

        private void Update()
        {
            transitionDelta = Time.deltaTime * transitionSpeed;
            GravityAndJump();
            HandleMovements();
            Crouch();
            SoundSense();
            HandleZoom();
            HandleSprinting();

            if (!canHeadBob) return;
            CheckMotion();
            ResetPosition();
            cameraFollow.LookAt(FocusTarget());
        }

        private void LateUpdate()
        {
            HandleCameraLook();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (canPush)
            {
                PushRigidBodies(hit);
            }
        }

        #endregion
        #region Private Methods

        // Get the layer mask from the layer ID
        LayerMask LayerMaskFromLayer(int layer)
        {
            return 1 << layer;
        }

        private void HandleSprinting()
        {

            // If player cannot sprint, exit the function
            if (!canPlayerSprint) return;

            // Determine if the player is sprinting based on input
            if (isSprintHold) 
            {
                isSprinting = inputManager.isSprintingHold;
            }
            else 
            {
                isSprinting = inputManager.isSprintingTap;
            }

            // Check if the player is sprinting and has non-zero velocity
            if (isSprinting && characterController.velocity.magnitude > 0)
            {
                // Drain sprint remaining while sprinting
                if (!unlimitedSprinting)
                {
                    sprintRemaining -= 1 * Time.deltaTime;
                    if (sprintRemaining <= 0)
                    {
                        isSprinting = false;
                        isSprintCooldown = true;
                    }
                }
            }
            else
            {
                // Regain sprint while not sprinting
                sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            // Handle sprint cooldown
            // When sprint remaining == 0, stop sprint ability until hitting cooldown
            if (isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    isSprintCooldown = false;
                }
            }
            else
            {
                sprintCooldown = sprintCooldownReset;
            }

            // Handle sprintBar
            if (hasStaminaBar && !unlimitedSprinting)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                staminaSlider.value = sprintRemainingPercent;
            }
        }
        #region Crouch System
        private void Crouch()
        {
            // Check if the character can crouch
            if (!canPlayerCrouch)
            {
                // If not, return immediately
                return;
            }

            // Determine the crouching state based on the input
            if (isCrouchHold)
            {
                // If crouch hold is enabled, set crouching state based on isCrouchingHold
                isCrouching = inputManager.isCrouchingHold;
            }
            else
            {
                // Otherwise, set crouching state based on isCrouchingTap
                isCrouching = inputManager.isCrouchingTap;
            }

            // Check if the character is crouching
            if (isCrouching)
            {
                // Set trying to uncrouch to false
                isTryingToUncrouch = false;

                // Adjust crouch settings to crouched height
                AdjustCrouchSettings(crouchedHeight);

                // Set the audio effect speed for crouching
                AudioEffectSpeed = crouchSoundPlayTime;
            }
            else
            {
                // Set trying to uncrouch to true
                isTryingToUncrouch = true;

                // Adjust crouch settings to initial height
                AdjustCrouchSettings(initialHeight);

                // Check if the character is not sprinting
                if (!isSprinting)
                {
                    // If not sprinting, set the audio effect speed for walking
                    AudioEffectSpeed = walkSoundSpeed;
                }
            }
        }

        private void AdjustCrouchSettings(float targetHeight)
        {
            if (isTryingToUncrouch)
            {
                // Calculate the origin of the raycast for ceiling detection
                Vector3 castOrigin = transform.position + new Vector3(0f, newHeight / 2, 0f);

                // Perform a raycast to detect the distance to the ceiling
                if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.2f))
                {
                    // Calculate the distance to the ceiling and adjust the target height
                    float distanceToCeiling = hit.point.y - castOrigin.y;
                    targetHeight = Mathf.Max(newHeight + distanceToCeiling - 0.1f, crouchedHeight);
                }
            }

            // Interpolate the character's height towards the target height
            newHeight = Mathf.Lerp(characterController.height, targetHeight, transitionDelta);

            // Update the character controller's height
            characterController.height = newHeight;

            // Adjust the camera position based on the new height
            Vector3 halfHeightDifference = new Vector3(0, (initialHeight - newHeight) / 2, 0);
            Vector3 newCameraHeight = initialCameraPosition - halfHeightDifference;
            cameraFollow.localPosition = newCameraHeight;
        }
        #endregion

        /// <summary>
        /// The method assigns the horizontal recoil and vertical recoil values provided as parameters to the camera.
        /// </summary>
        /// <param name="hRecoil">Float</param>
        /// <param name="vRecoil">Float</param>
        public void AddRecoil(float hRecoil, float vRecoil)
        {
            if (!haveCameraRecoil) return;
            this.hRecoil = hRecoil;
            this.vRecoil = vRecoil;
        }

        //Basic Camera Motion
        private void HandleCameraLook()
        {
            float mouseDirectionX = inputManager.mouseDirection.x * mouseSensitivity * Time.deltaTime + hRecoil;
            float mouseDirectionY = inputManager.mouseDirection.y * mouseSensitivity * Time.deltaTime + vRecoil;

            rotationY -= mouseDirectionY;
            rotationY = Mathf.Clamp(rotationY, minimumClamp, maximumClamp);

            transform.Rotate(mouseDirectionX * transform.up);
            cameraFollow.localRotation = Quaternion.Euler(rotationY, 0f, 0f);
            inputManager.mouseDirection = Vector2.zero;
        }
        private void HandleZoom()
        {
            if (!enableZoom) return;
            if (isZoomingHold) isZoomed = inputManager.isZoomingHold && !isSprinting;
            else isZoomed = inputManager.isZoomingTap && !isSprinting;

            if (isZoomed)
            {
                playerVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(playerVirtualCamera.m_Lens.FieldOfView, zoomFOV, transitionDelta);
            }
            else if (!isZoomed && !isSprinting)
            {
                playerVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(playerVirtualCamera.m_Lens.FieldOfView, FOV, transitionDelta);
            }
        }
        private void AdjustFOVSettings(float targetFOV) 
        {
            // If already zoomed, exit the function
            if (isZoomed) return;

            // If not moving, set the target field of view to FOV
            if (!moving)
            {
                targetFOV = FOV;
            }

            // Get the current field of view of the player virtual camera
            float currentFOV = playerVirtualCamera.m_Lens.FieldOfView;

            // Calculate the new field of view using linear interpolation
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, transitionDelta);

            // Set the field of view of the player virtual camera to the new calculated value
            playerVirtualCamera.m_Lens.FieldOfView = newFOV;
        }
        private void HandleMovements()
        {
            // If player cannot move, exit the function
            if (!canPlayerMove) return;

            // Initialize the movement state to Default
            PlayerMovementState movementState = PlayerMovementState.Default;

            // Check if the character height is approximately equal to the initial height
            bool approxHeight = Mathf.Approximately(characterController.height, initialHeight);

            // Determine the movement state based on player input and character state
            if (isSprinting && !inputManager.isCrouchingTap && canPlayerSprint && approxHeight)
            {
                movementState = PlayerMovementState.Sprinting;
            }
            else if (inputManager.isCrouchingTap)
            {
                movementState = PlayerMovementState.Crouching;
            }
            else if (!isSprinting && approxHeight)
            {
                movementState = PlayerMovementState.Walking;
            }

            // Update the player's movement state
            SwitchMoveState(movementState);

            // Calculate the move direction based on input and target speed
            moveDirection = inputManager.moveDirection.x * targetSpeed * Time.deltaTime * transform.right
                            + inputManager.moveDirection.y * targetSpeed * Time.deltaTime * transform.forward;

            // Move the character controller based on the calculated move direction
            characterController.Move(moveDirection);
        }

        private void SwitchMoveState(PlayerMovementState movementState)
        {
            switch (movementState)
            {
                case PlayerMovementState.Sprinting:
                    AudioEffectSpeed = sprintSoundSpeed;
                    targetSpeed = sprintSpeed;
                    AdjustFOVSettings(sprintFOV);
                    break;

                case PlayerMovementState.Crouching:
                    targetSpeed = crouchedSpeed;
                    break;

                case PlayerMovementState.Walking:
                    AudioEffectSpeed = walkSoundSpeed;
                    targetSpeed = walkSpeed;
                    AdjustFOVSettings(FOV);
                    break;

                case PlayerMovementState.Default:
                    break;
            }
        }

        private void GravityAndJump()
        {
            characterController.Move(Time.deltaTime * jumpVelocity.y * transform.up);
            bool isPreviouslyGrounded = IsGrounded; // Store previous grounded state
            IsGrounded = characterController.isGrounded;

            if (IsGrounded && !isPreviouslyGrounded)
            {
                audioSource.PlayOneShot(landClip); // Play land audio only when just touching ground
                havePreviouslyJumped = false;
            }

            if (IsGrounded)
            {
                if (!canJump)
                {
                    jumpVelocity.y = -1f;
                    return;
                }

                if (inputManager.haveJumped && !inputManager.isCrouchingTap)
                {
                    jumpVelocity.y = Mathf.Sqrt(jumpHeight * 2f * gravitationalForce);

                    if (!havePreviouslyJumped)
                    {
                        audioSource.PlayOneShot(jumpClip);
                        havePreviouslyJumped = true;
                    }
                }
                else if (!IsGrounded && jumpVelocity.y < 0f)
                {
                    jumpVelocity.y = -1f; // Reset jump velocity on landing
                }
            }
            else
            {
                jumpVelocity.y -= gravitationalForce * Time.deltaTime;
            }
        }

        private void PushRigidBodies(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body == null || body.isKinematic)
            {
                return;
            }

            var bodyLayerMask = 1 << body.gameObject.layer;

            if ((bodyLayerMask & pushLayers.value) == 0)
            {
                return;
            }

            if (hit.moveDirection.y < -0.3f)
            {
                return;
            }

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
            body.AddForce(pushDir * pushStrength, ForceMode.Impulse);
        }

        #region Head Bobbing
        // Calculate the footstep motion based on head bobbing parameters
        private Vector3 FootStepMotion()
        {
            Vector3 pos = Vector3.zero;
            pos.y += Mathf.Sin(Time.time * headBobFrequency) * headBobAmplitude;
            pos.x += Mathf.Cos(Time.time * headBobFrequency / 2) * headBobAmplitude * 2;
            return pos;
        }

        // Check motion to determine if the character is moving
        private void CheckMotion()
        {
            if (!moving || !IsGrounded)
            {
                // If the character is not moving or not grounded, return without further action
                return;
            }

            // If the character is moving and grounded, play the calculated motion
            PlayMotion(FootStepMotion());
        }

        // Apply the calculated motion to the camera follow position
        private void PlayMotion(Vector3 motion)
        {
            cameraFollow.localPosition += motion;
        }

        // Calculate the focus target position based on the character's position and camera follow position
        private Vector3 FocusTarget()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraFollow.localPosition.y, transform.position.z);
            pos += cameraFollow.forward * 15.0f;
            return pos;
        }

        // Reset the position of the camera follow to the starting position with interpolation
        private void ResetPosition()
        {
            if (cameraFollow.localPosition != _startPos)
            {
                // If the camera follow position is not at the starting position, interpolate towards the starting position
                cameraFollow.localPosition = Vector3.Lerp(cameraFollow.localPosition, _startPos, 1f * Time.deltaTime);
            }
        }
        #endregion

        #region Sound Management
        // Method to sense the floor material and player movement
        private void SoundSense()
        {
            if (!canPlaySound) return;
            Vector3 castOrigin = transform.position;
            if (Physics.Raycast(castOrigin, Vector3.down, out RaycastHit hit, 5f))
            {
                switch (hit.collider.tag.ToLower())
                {
                    case "grass":
                        floortag = "grass";
                        break;
                    case "metals":
                        floortag = "metal";
                        break;
                    case "gravel":
                        floortag = "gravel";
                        break;
                    case "water":
                        floortag = "water";
                        break;
                    case "concrete":
                        floortag = "concrete";
                        break;
                    case "wood":
                        floortag = "wood";
                        break;
                    default:
                        floortag = "";
                        break;
                }
            }

            // Sensing movement for players
            Vector3 velocity = characterController.velocity;
            Vector3 localVel = transform.InverseTransformDirection(velocity);

            moving = (localVel.z > footstepSensitivity || localVel.z < -footstepSensitivity || localVel.x > footstepSensitivity || localVel.x < -footstepSensitivity);
        }

        // Coroutine to sense steps and play sound
        private IEnumerator SenseSteps()
        {
            if (!canPlaySound) yield return null;
            while (true)
            {
                if (IsGrounded && moving)
                {
                    switch (floortag)
                    {
                        case "grass":
                            audioSource.clip = soundGrass[Random.Range(0, soundGrass.Length)];
                            break;
                        case "gravel":
                            audioSource.clip = soundGravel[Random.Range(0, soundGravel.Length)];
                            break;
                        case "water":
                            audioSource.clip = soundWater[Random.Range(0, soundWater.Length)];
                            break;
                        case "metal":
                            audioSource.clip = soundMetal[Random.Range(0, soundMetal.Length)];
                            break;
                        case "concrete":
                            audioSource.clip = soundConcrete[Random.Range(0, soundConcrete.Length)];
                            break;
                        case "wood":
                            audioSource.clip = soundWood[Random.Range(0, soundWood.Length)];
                            break;
                        default:
                            yield return null;
                            break;
                    }
                    if (audioSource.clip != null)
                    {
                        audioSource.PlayOneShot(audioSource.clip);
                        yield return new WaitForSeconds(AudioEffectSpeed);
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }
        #endregion

        #endregion
    }
}