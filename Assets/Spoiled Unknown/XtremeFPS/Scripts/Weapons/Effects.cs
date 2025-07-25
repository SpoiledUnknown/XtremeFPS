using UnityEngine;
using XtremeFPS.InputHandling;
using XtremeFPS.Player.Controller;

namespace XtremeFPS.WeaponSystem.Effects
{
    /// <summary>
    /// This class is responsible for handling various effects in weapons.
    /// </summary>
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Effects")]
    public class Effects : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private PlayerMovementController fpsController;
        [SerializeField] private float transitionSpeed;
        private XtremeFPSInputHandler inputManager;
        private bool aiming;

        //Weapon Move Bobbing
        [Header("Weapon Bobbing Settings")]
        [SerializeField] private bool haveBobbing = true;
        [SerializeField] private float bobMagnitude = 9f;
        [SerializeField] private float idleSpeed = 2f;
        [SerializeField] private float walkSpeedMultiplier = 4f;
        [SerializeField] private float aimReduction = 4f;

        private float sinY = 0f;
        private float sinX = 0f;
        private Vector3 lastPosition;

        //Sway
        [Header("Sway Settings")]
        [SerializeField] private bool haveSway = true;
        [SerializeField] private float Intensity = 5f;
        [SerializeField] private float rotationalIntensity = 2f;
        [SerializeField] private float maxAngleClamp = 20f;
        [SerializeField] private float recoverySpeed = 50f;

        private float impactForce = 0;
        private Quaternion originRotation;
        private float mouseX;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;

            bobMagnitude = bobMagnitude / 1000f;
            lastPosition = transform.position;
            if (haveSway) originRotation = transform.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            mouseX = inputManager.mouseDirection.x;
            aiming = inputManager.isAimHold || inputManager.isAimTap;

            WeaponBobbing();
            SwayEffect();
        }

        #region Effects
        private void WeaponBobbing()
        {
            if (!haveBobbing) return;
            if (!fpsController.IsGrounded || fpsController.MovementState == PlayerMovementController.PlayerMovementState.Sliding)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
                return;
            }

            // Calculate delta time based on the player's movement speed.
            float delta = Time.deltaTime * idleSpeed;
            float velocity = (lastPosition - transform.position).magnitude * walkSpeedMultiplier;
            delta += Mathf.Clamp(velocity, 0, idleSpeed * 3f);

            // Update the sinX and sinY values to create a bobbing effect.
            sinX += delta / 2;
            sinY += delta;
            sinX %= Mathf.PI * 2;
            sinY %= Mathf.PI * 2;

            // Adjust the weapon's local position to create the bobbing effect.
            float magnitude = aiming ? bobMagnitude / aimReduction : bobMagnitude;
            transform.localPosition = Vector3.zero + magnitude * Mathf.Sin(sinY) * Vector3.up;
            transform.localPosition += magnitude * Mathf.Sin(sinX) * Vector3.right;

            lastPosition = transform.position;
        }

        private void SwayEffect()
        {
            if (!haveSway || aiming) return;

            Quaternion targetRotation = originRotation * Quaternion.AngleAxis(rotationalIntensity * mouseX * -1f, Vector3.up);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, transitionSpeed * Time.deltaTime);

            switch (fpsController.IsGrounded)
            {
                case false:
                    // Adjust the weapon's rotation based on the player's jump velocity.
                    float yVelocity = fpsController.JumpVelocity.y;
                    yVelocity = Mathf.Clamp(yVelocity, -maxAngleClamp, maxAngleClamp);
                    impactForce = -yVelocity * Intensity;

                    if (aiming) yVelocity = Mathf.Max(yVelocity, 0);

                    // Update the weapon's local rotation to simulate the jump sway effect.
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0f, 0f, yVelocity * Intensity), Time.deltaTime * transitionSpeed);
                    break;

                case true when impactForce >= 0: // If the player is grounded and has impact force.
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, impactForce), Time.deltaTime * transitionSpeed);
                    impactForce -= recoverySpeed * Time.deltaTime;
                    break;

                case true: // If the player is grounded and there's no impact force.
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * transitionSpeed);
                    break;
            }
        }
        #endregion
    }
}
