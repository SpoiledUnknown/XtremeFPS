using UnityEngine;
using XtremeFPS.InputHandling;
using XtremeFPS.Player.Controller;

namespace XtremeFPS.WeaponSystem.Effects
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Effects")]
    public class EffectsManager : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private PlayerMovementController fpsController;
        private XtremeFPSInputHandler inputManager;
        private bool aiming;

        [Header("Weapon Bobbing Settings")]
        [SerializeField] private Transform bobberTransform;
        [SerializeField] private bool haveBobbing = true;
        [SerializeField] private float bobMagnitude = 9f;
        [SerializeField] private float idleSpeed = 2f;
        [SerializeField] private float walkSpeedMultiplier = 4f;
        [SerializeField] private float aimReduction = 4f;

        private float sinY = 0f;
        private float sinX = 0f;
        private Vector3 lastPosition;

        [Header("Sway Settings")]
        [SerializeField] private bool haveSway = true;
        [Header("Jump")]
        [SerializeField] private float jumpSwayIntensity = 5f;
        [SerializeField] private float maxJumpSwayAngle = 20f;
        [SerializeField] private float smoothJump = 10f;
        [SerializeField] private float recoverySpeed = 50f;

        private float impactForce = 0;
        private float mouseX;
        private float mouseY;

        [Header("Position")]
        [SerializeField] private float positionAmount = 0.02f;
        [SerializeField] private float maxPositionAmount = 0.06f;
        [SerializeField] private float smoothPosition = 20f;

        private Vector3 initialPosition;
        
        [Header("Rotation")] 
        [SerializeField] private float rotationAmount = 4f;
        [SerializeField] private float maxRotationAmount = 5f;
        [SerializeField] private float smoothRotation = 15f;
        [Space] 
        [SerializeField] private bool rotationX = true; 
        [SerializeField] private bool rotationZ = true;
        
        private Quaternion initialRotation;
        private Vector3 refVelocity = new Vector3(0, 0, 0);

        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;

            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
            lastPosition =  bobberTransform.position;
        }

        private void Update()
        {
            mouseX = inputManager.MouseDirection.x;
            mouseY = inputManager.MouseDirection.y;
            aiming = inputManager.IsAimHold || inputManager.IsAimTap;

            WeaponBobbing();
            if (!haveSway) return;
            JumpSwayEffect();
            PositionalSway();
            RotationalSway();
        }

        #region Effects
        private void WeaponBobbing()
        {
            if (!haveBobbing) return;
            if (!fpsController.IsGrounded || fpsController.MovementState == PlayerMovementController.PlayerMovementState.Sliding)
            {
                bobberTransform.localPosition = Vector3.Lerp(bobberTransform.localPosition, Vector3.zero, Time.deltaTime);
                return;
            }

            float delta = Time.deltaTime * idleSpeed;
            float velocity = (lastPosition - bobberTransform.position).magnitude * walkSpeedMultiplier;
            delta += Mathf.Clamp(velocity, 0, idleSpeed * 3f);

            sinX += delta / 2;
            sinY += delta;
            sinX %= Mathf.PI * 2;
            sinY %= Mathf.PI * 2;

            float magnitude = aiming ? bobMagnitude / (aimReduction * 1000f) : (bobMagnitude / 1000f);
            bobberTransform.localPosition = Vector3.zero + magnitude * Mathf.Sin(sinY) * Vector3.up;
            bobberTransform.localPosition += magnitude * Mathf.Sin(sinX) * Vector3.right;

            lastPosition = bobberTransform.position;
        }

        private void JumpSwayEffect()
        {
            switch (fpsController.IsGrounded)
            {
                case false:
                    float yVelocity = fpsController.JumpVelocity.y;
                    yVelocity = Mathf.Clamp(yVelocity, -maxJumpSwayAngle, maxJumpSwayAngle);
                    impactForce = -yVelocity * jumpSwayIntensity;
                    if (aiming) yVelocity = Mathf.Max(yVelocity, 0);
                    
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, yVelocity * jumpSwayIntensity), 1f - Mathf.Exp(-smoothJump * Time.deltaTime));
                    break;

                case true when impactForce >= 0:
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, impactForce), 1f - Mathf.Exp(-smoothJump * Time.deltaTime));
                    impactForce -= recoverySpeed * Time.deltaTime;
                    break;
            }
        }

        private void PositionalSway()
        {
            float moveX = Mathf.Clamp(mouseX * positionAmount, -maxPositionAmount, maxPositionAmount);
            float moveY = Mathf.Clamp(mouseY * positionAmount, -maxPositionAmount, maxPositionAmount);
            
            Vector3 finalPosition = new Vector3(0, moveY, moveX);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition + initialPosition, ref refVelocity,Time.deltaTime * smoothPosition);
        }

        private void RotationalSway()
        {
            float tiltY = Mathf.Clamp(mouseY * rotationAmount, -maxRotationAmount, maxRotationAmount);
            float tiltX = Mathf.Clamp(mouseX * rotationAmount, -maxRotationAmount, maxRotationAmount);

            Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? tiltX : 0f, 0f, rotationZ ? tiltY : 0f));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, 1f - Mathf.Exp(-smoothRotation * Time.deltaTime));
        }

        #endregion
    }
}
