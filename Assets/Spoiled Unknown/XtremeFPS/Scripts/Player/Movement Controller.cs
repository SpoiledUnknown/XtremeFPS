/*Copyright © Spoiled Unknown*/
/*2024*/
using System.Collections;
using UnityEngine;
using XtremeFPS.InputHandling;
using Unity.Cinemachine;
using UnityEngine.UI;
using XtremeFPS.Interfaces;
using XtremeFPS.WeaponSystem.Holder;
using XtremeFPS.CameraSystem;

namespace XtremeFPS.Controller
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(XtremeFPSInputHandler))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Movement Controller")]
    public class MovementController : MonoBehaviour
    {
        #region Variables
        // Player
        public float transitionSpeed;
        public float walkSpeed = 5f;
        public float walkSoundSpeed;

        public CharacterController CharacterController {  get; private set; }
        private XtremeFPSInputHandler inputManager;
        public Transform cameraFollowTPS;
        public Transform cameraFollow;
        public PlayerMovementState MovementState {  get; private set; }
        public enum PlayerMovementState
        {
            Walking,
            Sprinting,
            Crouching,
            Sliding,
            Default
        }
        public float targetSpeed;

        private float transitionDelta;
        private Vector3 horizontalMovement;
        private float turnSmoothVelocity;

        //sprinting
        public bool canPlayerSprint;
        public bool unlimitedSprinting;
        public bool isSprintHold;
        public float sprintSpeed = 8f;
        public float sprintDuration = 8f;
        public float sprintCooldown = 8f;
        public Slider staminaBar;
        public float sprintSoundSpeed;

        public bool isSprinting;
        private readonly float sprintCooldownReset;
        private float sprintRemaining;


        // Gravity and Jumping
        public bool canJump;
        public float jumpHeight = 2f;
        public float gravitationalForce = 10f;
        public Transform groundSphere;
        public float groundRadius;
        public int groundLayerID;

        private Vector3 groundSpherePosition;
        private LayerMask groundLayerMask;

        public bool IsGrounded { get; private set; }
        public Vector3 JumpVelocity { get; private set; }


        // Crouching
        public bool canPlayerCrouch;
        public bool isCrouchHold;
        public float crouchedHeight = 1f;
        public float crouchedSpeed = 1f;
        public float crouchSoundPlayTime;

        public bool isCrouching;
        private float newHeight;
        private float initialHeight;
        private Vector3 initialCameraPosition;

        //Sliding
        public float slidingSpeed;
        public float slidingDuration;

        private bool canSlide;
        private float slidingTime;
        private bool isOnSlope;
        private readonly float slopeCheckInterval = 0.2f;
        private float nextSlopeCheckTime;
        private RaycastHit slopeHit;

        

        //Sound System
        public string SurfaceType { get; private set; }
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

        public AudioClip landingAudioClip;
        public AudioClip jumpingAudioClip;
        public AudioClip slidingAudioClip;
        public float footstepSensitivity;

        private AudioSource audioSource;
        private float AudioEffectSpeed;
        public bool isMoving = false;


        // Handling Physics
        public bool canPush;
        public float pushStrength = 1.1f;
        public int pushLayerId;

        private LayerMask pushLayerMask;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            audioSource = GetComponent<AudioSource>();
            CharacterController = GetComponent<CharacterController>();

            AudioEffectSpeed = walkSoundSpeed;
            StartCoroutine(PlayFootstepSounds());

            pushLayerMask = 1 << pushLayerId;
            groundLayerMask = 1 << groundLayerID;
            groundSpherePosition = groundSphere.localPosition;

            if (!canPlayerCrouch) return;
            initialHeight = CharacterController.height;
            initialCameraPosition = cameraFollow.transform.localPosition;
        }

        private void Update()
        {
            transitionDelta = Time.deltaTime * transitionSpeed;

            //character Controller movement
            if (!inputManager.IsSwitchingCamera)
            {
                horizontalMovement = inputManager.moveDirection.x * targetSpeed * Time.deltaTime * transform.right +
                      inputManager.moveDirection.y * targetSpeed * Time.deltaTime * transform.forward;
            }
            else
            {
                Vector3 direction = new Vector3(inputManager.moveDirection.x, 0f, inputManager.moveDirection.y).normalized;
                if (direction.magnitude >= 0.1f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraFollowTPS.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, transitionDelta);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    horizontalMovement = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward * Time.deltaTime * targetSpeed;
                }
            }

            Vector3 verticalMovement = JumpVelocity.y * Time.deltaTime * transform.up;
            CharacterController.Move(horizontalMovement + verticalMovement);
            horizontalMovement = Vector3.zero;

            //checking if player is moving or not by using Inverse of transform direction for god knows what reason\
            //but yeah this looks cool
            Vector3 localVelocity = transform.InverseTransformDirection(CharacterController.velocity);
            isMoving = Mathf.Abs(localVelocity.z) > footstepSensitivity || Mathf.Abs(localVelocity.x) > footstepSensitivity;

            PlayerInputs();
            HandleSprintCooldown();
            GravityAndJump();
            HandleStateMachine();
            DetectSurfaceAndMovement();
            if (MovementState == PlayerMovementState.Sliding) HanldeSliding();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!canPush) return;

            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            LayerMask bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & pushLayerMask) == 0) return;
            if (hit.moveDirection.y < -0.3f) return;

            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
            body.AddForce(pushDirection * pushStrength, ForceMode.Impulse);
        }
        #endregion

        #region Private Methods
        private void PlayerInputs()
        {
            if (isSprintHold) isSprinting = inputManager.isSprintingHold;
            else isSprinting = inputManager.isSprintingTapped;

            if (isCrouchHold) isCrouching = inputManager.isCrouchingHold;
            else isCrouching = inputManager.isCrouchingTapped;

            canSlide = isCrouching && isSprinting && canPlayerCrouch;
        }
        

        private void HandleSprintCooldown()
        {
            if (unlimitedSprinting) return;

            if (MovementState == PlayerMovementState.Sprinting &&
                CharacterController.velocity.magnitude > 0)
            {
                sprintRemaining -= 1 * Time.deltaTime;
                if (sprintRemaining <= 0)
                {
                    inputManager.isSprintingTapped = false;
                    inputManager.isSprintingHold = false;
                    sprintCooldown -= 1 * Time.deltaTime;
                }
                else sprintCooldown = sprintCooldownReset;
            }
            else sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);

            if (staminaBar != null)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                staminaBar.value = sprintRemainingPercent;
            }
        }

        private void AdjustCrouchHeight(float targetHeight, bool isTryingToUncrouch)
        {
            if (isTryingToUncrouch)
            {
                Vector3 castOrigin = transform.position + new Vector3(0f, newHeight / 2, 0f);
                if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.2f))
                {
                    float distanceToCeiling = hit.point.y - castOrigin.y;
                    targetHeight = Mathf.Max(newHeight + distanceToCeiling - 0.1f, crouchedHeight);
                }
            }

            newHeight = Mathf.Lerp(CharacterController.height, targetHeight, transitionDelta);
            CharacterController.height = newHeight;

            // Adjust the camera position based on the new height
            Vector3 halfHeightDifference = new Vector3(0, (initialHeight - newHeight) * 0.5f, 0);

            Vector3 newCameraHeight = initialCameraPosition - halfHeightDifference;
            groundSphere.localPosition = groundSpherePosition + halfHeightDifference;
            cameraFollow.localPosition = newCameraHeight;
            cameraFollowTPS.localPosition = new Vector3(transform.localPosition.x, newCameraHeight.y, transform.localPosition.z);
        }

        #region Sliding
        private void HanldeSliding()
        {
            if (Time.time >= nextSlopeCheckTime)
            {
                nextSlopeCheckTime = Time.time + slopeCheckInterval;
                isOnSlope = CheckIfOnSlope();
            }
            if (!isOnSlope && IsGrounded) slidingTime -= Time.deltaTime;
            if (slidingTime <= 0)
            {
                inputManager.isSprintingHold = false;
                inputManager.isSprintingTapped = false;
                MovementState = PlayerMovementState.Crouching;
            }
        }

        private bool CheckIfOnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, CharacterController.height * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                if (angle > CharacterController.slopeLimit || angle == 0) return false;
                Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, slopeHit.normal).normalized;
                Vector3 movementDirection = new Vector3(CharacterController.velocity.x, 0, CharacterController.velocity.z).normalized;
                float dotProduct = Vector3.Dot(movementDirection, slopeDirection);
                return dotProduct > 0;
            }
            return false;
        }
        #endregion
        private void HandleStateMachine()
        {
            if (canSlide && (targetSpeed > (sprintSpeed * 0.5f + 1.0f)) && MovementState != PlayerMovementState.Sliding)
            {
                slidingTime = slidingDuration;
                MovementState = PlayerMovementState.Sliding;
            }
            else if (canPlayerSprint && isSprinting && !isCrouching) MovementState = PlayerMovementState.Sprinting;
            else if (canPlayerCrouch && isCrouching && !isSprinting) MovementState = PlayerMovementState.Crouching;
            else if (!isSprinting && !isCrouching) MovementState = PlayerMovementState.Walking;

            SwitchMoveState(MovementState);
        }

        private void SwitchMoveState(PlayerMovementState movementState)
        {
            switch (movementState)
            {
                case PlayerMovementState.Sprinting:
                    targetSpeed = Mathf.Lerp(targetSpeed, sprintSpeed, transitionDelta);
                    AudioEffectSpeed = sprintSoundSpeed;
                    AdjustCrouchHeight(initialHeight, true);
                    break;

                case PlayerMovementState.Crouching:
                    targetSpeed = Mathf.Lerp(targetSpeed, crouchedSpeed, transitionDelta);
                    AudioEffectSpeed = crouchSoundPlayTime;
                    AdjustCrouchHeight(crouchedHeight, false);
                    break;

                case PlayerMovementState.Walking:
                    targetSpeed = Mathf.Lerp(targetSpeed, walkSpeed, transitionDelta);
                    AudioEffectSpeed = walkSoundSpeed;
                    AdjustCrouchHeight(initialHeight, true);
                    break;

                case PlayerMovementState.Sliding:
                    targetSpeed = Mathf.Lerp(targetSpeed, slidingSpeed, transitionDelta);
                    AdjustCrouchHeight(crouchedHeight, false);
                    if (!audioSource.isPlaying && IsGrounded) audioSource.PlayOneShot(slidingAudioClip);
                    else if (!IsGrounded) audioSource.Stop();
                    canSlide = false;
                    break;

                case PlayerMovementState.Default:
                    break;
            }
        }

        private void GravityAndJump()
        {
            bool wasPreviouslyGrounded = IsGrounded;
            IsGrounded = Physics.CheckSphere(groundSphere.position, groundRadius, groundLayerMask, QueryTriggerInteraction.Ignore);

            if (!IsGrounded)
            {
                JumpVelocity = new Vector3(JumpVelocity.x, JumpVelocity.y - gravitationalForce * Time.deltaTime, JumpVelocity.z);
                return; 
            }

            if (!canJump)
            {
                JumpVelocity = new Vector3(JumpVelocity.x, -0.5f, JumpVelocity.z);
                return;
            }

            if (!wasPreviouslyGrounded) audioSource.PlayOneShot(landingAudioClip);

            if (inputManager.isJumping && 
                MovementState != PlayerMovementState.Crouching &&
                MovementState != PlayerMovementState.Sliding)
            {
                JumpVelocity =  new Vector3(JumpVelocity.x, Mathf.Sqrt(jumpHeight * 2f * gravitationalForce), JumpVelocity.z); ;
                if (wasPreviouslyGrounded) audioSource.PlayOneShot(jumpingAudioClip);
            }
            else if (!IsGrounded && JumpVelocity.y < 0f) JumpVelocity = new Vector3(JumpVelocity.x, -0.5f, JumpVelocity.z);
        }
        #region Sound Management
        private void DetectSurfaceAndMovement()
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5f)) return;
            SurfaceType = hit.collider.tag.ToLower() switch
            {
                "grass" => "grass",
                "metals" => "metal",
                "gravel" => "gravel",
                "water" => "water",
                "concrete" => "concrete",
                "wood" => "wood",
                _ => "Unknown",
            };
        }

        private IEnumerator PlayFootstepSounds()
        {
            while (true)
            {
                if (!IsGrounded || !isMoving || MovementState == PlayerMovementState.Sliding)
                {
                    yield return null;
                    continue;
                }

                switch (SurfaceType)
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
                else yield return null;
            }
        }
        #endregion

        
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (IsGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(groundSphere.position, groundRadius);
        }
#endif
    }
}