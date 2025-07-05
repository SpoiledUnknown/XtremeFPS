using Unity.Cinemachine;
using UnityEngine;
using XtremeFPS.InputHandling;
using XtremeFPS.Player.Controller;

namespace XtremeFPS.Player.CameraSystem
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/FPP Camera Manager")]
    public class FPPCameraManager : MonoBehaviour
    {
        private XtremeFPSInputHandler inputManager;

        // References
        public MovementController movementController;
        public Transform FirstPersonCameraRoot;
        public CinemachineCamera FirstPersonCamera;

        //general
        public float mouseSensitivity;
        public float clampAngle;
        public float sprintFOV;
        public float walkFOV;

        private float rotationY;
        private float mouseDirectionX;
        private float mouseDirectionY;

        //Zooming
        public float transitionSpeed;
        public bool canZoom;
        public bool isZoomHold;
        public float zoomFOV = 30f;

        private bool isZooming;

        //Head Bobbing effect
        public bool canHeadBob;
        public float headBobAmplitude = 0.01f;
        public float headBobFrequency = 18.5f;

        private Vector3 headBobStartPosition;

        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            headBobStartPosition = FirstPersonCameraRoot.localPosition;
            FirstPersonCamera.Lens.FieldOfView = walkFOV;
        }

        private void Update()
        {
            HandleInputs();
            HandleFOVChange();

            if (!canHeadBob || inputManager.IsSwitchingCamera) return;
            HandleHeadBob();
            FirstPersonCameraRoot.LookAt(FocusTarget());
        }

        private void LateUpdate()
        {
            rotationY -= mouseDirectionY;
            rotationY = Mathf.Clamp(rotationY, clampAngle * -1f, clampAngle);

            movementController.CharacterController.transform.Rotate(mouseDirectionX * movementController.CharacterController.transform.up);
            FirstPersonCameraRoot.localRotation = Quaternion.Euler(rotationY, 0f, 0f);

            inputManager.mouseDirection = Vector2.zero;
        }

        private void HandleInputs()
        {
            mouseDirectionX = inputManager.mouseDirection.x * mouseSensitivity * Time.deltaTime;
            mouseDirectionY = inputManager.mouseDirection.y * mouseSensitivity * Time.deltaTime;

            if (isZoomHold) isZooming = inputManager.isZoomingHold;
            else isZooming = inputManager.isZoomingTapped;
        }

        private void AdjustFOVSettings(float targetFOV)
        {
            float currentFOV = FirstPersonCamera.Lens.FieldOfView;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, (transitionSpeed * Time.deltaTime));
            FirstPersonCamera.Lens.FieldOfView = newFOV;
        }

        private void HandleFOVChange()
        {
            if (movementController.MovementState == MovementController.PlayerMovementState.Sprinting)
            {
                AdjustFOVSettings(sprintFOV);
                return;
            }
            if (movementController.MovementState == MovementController.PlayerMovementState.Walking)
            {
                AdjustFOVSettings(walkFOV);
            }

            if (canZoom)
            {
                if (isZooming) AdjustFOVSettings(zoomFOV);
                else if (!isZooming) AdjustFOVSettings(walkFOV);
            }
        }

        private void HandleHeadBob()
        {
            if (movementController.isMoving)
            {
                Vector3 headBobMotion = Vector3.zero;
                headBobMotion.y += Mathf.Sin(Time.time * headBobFrequency) * headBobAmplitude;
                headBobMotion.x += Mathf.Cos(Time.time * headBobFrequency / 2) * headBobAmplitude * 2;

                FirstPersonCameraRoot.localPosition += headBobMotion;
            }
            else if (FirstPersonCameraRoot.localPosition != headBobStartPosition)
            {
                FirstPersonCameraRoot.localPosition = Vector3.Lerp(FirstPersonCameraRoot.localPosition, headBobStartPosition, 1f * Time.deltaTime);
            }
        }

        private Vector3 FocusTarget()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + FirstPersonCameraRoot.localPosition.y, transform.position.z);
            pos += FirstPersonCameraRoot.forward * 15.0f;
            return pos;
        }
    }
}
