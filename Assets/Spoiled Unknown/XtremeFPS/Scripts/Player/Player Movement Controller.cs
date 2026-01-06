using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XtremeFPS.InputHandling;

namespace XtremeFPS.Player.Controller
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Player Movement Controller")]
    public class PlayerMovementController : MonoBehaviour
    {
        #region Variables
        // Player
        [Header("Player Settings")]
        [SerializeField] private float transitionSpeed;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private PlayerManager playerManager;

        public CharacterController CharacterController { private set; get; }
        public PlayerMovementState MovementState {  get; private set; }
        public enum PlayerMovementState
        {
            Walking,
            Sprinting,
            Crouching,
            Sliding,
            Default
        }

        private XtremeFPSInputHandler inputManager;
        private float targetSpeed;
        private float transitionDelta;
        private Vector3 horizontalMovement;
        private float turnSmoothVelocity;

        //sprinting
        [Header("Sprinting Settings")]
        [SerializeField] private bool canPlayerSprint;
        [SerializeField] private bool unlimitedSprinting;
        [SerializeField] private bool isSprintHold;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float sprintCooldown = 8f;
        public float sprintDuration = 8f;

        private bool isSprinting;
        private bool sprintAllowed;
        private readonly float sprintCooldownReset;
        
        public float SprintRemaining { get; private set; }

        // Gravity and Jumping
        [Header("Jumping Settings")]
        [SerializeField] private bool canJump;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravitationalForce = 10f;
        [SerializeField] private Transform groundSphere;
        [SerializeField] private float groundRadius;
        [SerializeField] private LayerMask groundLayerMask;

        private Vector3 groundSpherePosition;
        private Vector3 cameraRootPositon;
        
        public Vector3 JumpVelocity { get; private set; }
        public bool IsGrounded { get; private set; }

        // Crouching
        [Header("Crouch & Slide Settings")]
        [SerializeField] private bool canPlayerCrouch;
        [SerializeField] private bool isCrouchHold;
        [SerializeField] private float crouchedHeight = 1f;
        [SerializeField] private float crouchedSpeed = 1f;

        private bool isCrouching;
        private float newHeight;
        private float initialHeight;
        [Space(10)]
        //Sliding
        [SerializeField] private float slidingSpeed;
        [SerializeField] private float slidingDuration;

        private bool canSlide;
        private float slidingTime;
        private bool isOnSlope;
        private readonly float slopeCheckInterval = 0.2f;
        private float nextSlopeCheckTime;
        private RaycastHit slopeHit;

        //Sound System
        [Header("Sound Settings")]
        [SerializeField] private float footstepSensitivity;
        [Space(5)]
        [SerializeField] private AudioClip[] grassAudioClip;
        [Space(5)]
        [SerializeField] private AudioClip[] waterAudioClip;
        [Space(5)]
        [SerializeField] private AudioClip[] metalAudioClip;
        [Space(5)]
        [SerializeField] private AudioClip[] concreteAudioClip;
        [Space(5)]
        [SerializeField] private AudioClip[] gravelAudioClip;
        [Space(5)]
        [SerializeField] private AudioClip[] woodAudioClip;
        [Space(5)]
        [SerializeField] private AudioClip landAudioClip;
        [SerializeField] private AudioClip jumpAudioClip;
        [SerializeField] private AudioClip slideAudioClip;

        private AudioSource audioSource;
        private float AudioEffectSpeed;
        public bool IsMoving { private set; get; }


        // Handling Physics
        [Header("Physics Settings")]
        [SerializeField] private bool canPush;
        [SerializeField] private float pushStrength = 1.1f;
        [SerializeField] private LayerMask pushLayerMask;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            audioSource = GetComponent<AudioSource>();
            CharacterController = GetComponent<CharacterController>();

            StartCoroutine(PlayFootstepSounds());

            groundSpherePosition = groundSphere.localPosition;
            cameraRootPositon = cameraRoot.localPosition;

            sprintAllowed = canPlayerSprint;

            if (!canPlayerCrouch) return;
            initialHeight = CharacterController.height;
        }

        private void Update()
        {
            transitionDelta = Time.deltaTime * transitionSpeed;
            AudioEffectSpeed = Mathf.Clamp(1f / targetSpeed, 0.3f, 1f);
            
            if (playerManager.isTpp)
            {
                Vector3 direction = new Vector3( inputManager.MoveDirection.x, 0f, inputManager.MoveDirection.y ).normalized;

                if (direction.sqrMagnitude > 0.01f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraRoot.eulerAngles.y;

                    float angle = Mathf.SmoothDampAngle( transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, transitionDelta);

                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                    horizontalMovement = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * (targetSpeed * Time.deltaTime);
                }
            }
            else
            {
                horizontalMovement =
                    inputManager.MoveDirection.x * targetSpeed * Time.deltaTime * transform.right +
                    inputManager.MoveDirection.y * targetSpeed * Time.deltaTime * transform.forward;
            }


            Vector3 verticalMovement = JumpVelocity.y * Time.deltaTime * transform.up;
            CharacterController.Move(horizontalMovement + verticalMovement);
            horizontalMovement = Vector3.zero;

            Vector3 horizontalVelocity = new Vector3(CharacterController.velocity.x, 0f, CharacterController.velocity.z);
            IsMoving = horizontalVelocity.magnitude > footstepSensitivity;

            PlayerInputs();
            HandleSprintCooldown();
            GravityAndJump();
            HandleStateMachine();
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
            isSprinting = canPlayerSprint && (isSprintHold ? inputManager.IsSprintHold : inputManager.IsSprintTap);

            isCrouching = canPlayerCrouch && (isCrouchHold ? inputManager.IsCrouchHold : inputManager.IsCrouchTap);

            canSlide = isCrouching && isSprinting && canPlayerCrouch;
        }

        private void HandleSprintCooldown()
        {
            if (unlimitedSprinting)
            {
                canPlayerSprint = sprintAllowed;
                return;
            }

            if (MovementState == PlayerMovementState.Sprinting &&
                CharacterController.velocity.magnitude > 0)
            {
                SprintRemaining -= Time.deltaTime;

                if (SprintRemaining <= 0f)
                {
                    canPlayerSprint = false;
                    sprintCooldown -= Time.deltaTime;
                }
                else
                {
                    sprintCooldown = sprintCooldownReset;
                }
            }
            else
            {
                SprintRemaining = Mathf.Clamp(
                    SprintRemaining + Time.deltaTime,
                    0f,
                    sprintDuration);
            }

            if (sprintCooldown <= 0f)
                canPlayerSprint = true;
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

            Vector3 halfHeightDifference = new Vector3(0, (initialHeight - newHeight) * 0.5f, 0);
            groundSphere.localPosition = groundSpherePosition + halfHeightDifference;
        }

        #region Sliding
        private void HanldeSliding()
        {
            if (Time.time >= nextSlopeCheckTime)
            {
                nextSlopeCheckTime = Time.time + slopeCheckInterval;
                isOnSlope = CheckIfOnSlope();
            }

            if (!isOnSlope && IsGrounded)
                slidingTime -= Time.deltaTime;

            if (slidingTime <= 0f)
            {
                canPlayerSprint = false;
                inputManager.DisableSprint();
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
            if (canSlide && IsMoving && IsGrounded && (Mathf.Abs(targetSpeed - sprintSpeed) <= 0.3f) && MovementState != PlayerMovementState.Sliding)
            {
                slidingTime = slidingDuration;
                MovementState = PlayerMovementState.Sliding;
            }
            else if (isSprinting && !isCrouching) MovementState = PlayerMovementState.Sprinting;
            else if (isCrouching && !isSprinting) MovementState = PlayerMovementState.Crouching;
            else if (!isSprinting && !isCrouching) MovementState = PlayerMovementState.Walking;

            SwitchMoveState(MovementState);
        }
        
        

        private void SwitchMoveState(PlayerMovementState movementState)
        {
            switch (movementState)
            {
                case PlayerMovementState.Sprinting:
                    targetSpeed = Mathf.Lerp(targetSpeed, sprintSpeed, transitionDelta);
                    AdjustCrouchHeight(initialHeight, true);
                    break;

                case PlayerMovementState.Crouching:
                    targetSpeed = Mathf.Lerp(targetSpeed, crouchedSpeed, transitionDelta);
                    AdjustCrouchHeight(crouchedHeight, false);
                    break;

                case PlayerMovementState.Walking:
                    targetSpeed = Mathf.Lerp(targetSpeed, walkSpeed, transitionDelta);
                    AdjustCrouchHeight(initialHeight, true);
                    break;

                case PlayerMovementState.Sliding:
                    targetSpeed = Mathf.Lerp(targetSpeed, slidingSpeed, transitionDelta);
                    AdjustCrouchHeight(crouchedHeight, false);
                    if (!audioSource.isPlaying && IsGrounded) audioSource.PlayOneShot(slideAudioClip);
                    else if (!IsGrounded) audioSource.Stop();
                    canSlide = false;

                    HanldeSliding();
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

            if (!wasPreviouslyGrounded) audioSource.PlayOneShot(landAudioClip);

            if (inputManager.IsTryingToJump && 
                MovementState != PlayerMovementState.Crouching &&
                MovementState != PlayerMovementState.Sliding)
            {
                JumpVelocity =  new Vector3(JumpVelocity.x, Mathf.Sqrt(jumpHeight * 2f * gravitationalForce), JumpVelocity.z); ;
                if (wasPreviouslyGrounded) audioSource.PlayOneShot(jumpAudioClip);
            }
            else if (!IsGrounded && JumpVelocity.y < 0f) JumpVelocity = new Vector3(JumpVelocity.x, -0.5f, JumpVelocity.z);
        }
        
        #region Sound Management
        private IEnumerator PlayFootstepSounds()
        {
            while (true)
            {
                if (!IsGrounded || !IsMoving || MovementState == PlayerMovementState.Sliding)
                {
                    yield return null;
                    continue;
                }

                if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, CharacterController.height))
                {
                    yield return null;
                    continue;
                }
                switch (hit.collider.tag.ToLower())
                {
                    case "grass":
                        audioSource.clip = grassAudioClip[Random.Range(0, grassAudioClip.Length)];
                        break;
                    case "gravel":
                        audioSource.clip = gravelAudioClip[Random.Range(0, gravelAudioClip.Length)];
                        break;
                    case "water":
                        audioSource.clip = waterAudioClip[Random.Range(0, waterAudioClip.Length)];
                        break;
                    case "metal":
                        audioSource.clip = metalAudioClip[Random.Range(0, metalAudioClip.Length)];
                        break;
                    case "concrete":
                        audioSource.clip = concreteAudioClip[Random.Range(0, concreteAudioClip.Length)];
                        break;
                    case "wood":
                        audioSource.clip = woodAudioClip[Random.Range(0, woodAudioClip.Length)];
                        break;
                    default:
                        yield return null;
                        break;
                }

                if (audioSource.clip)
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
            Color transparentGreen = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Gizmos.color = transparentGreen;
            Gizmos.DrawSphere(groundSphere.position, groundRadius);
        }
#endif
    }
}