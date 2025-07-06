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
        [Header("References")]
        [SerializeField] private PlayerMovementController movementController;
        [SerializeField] private Transform firstPersonCameraRoot;
        [SerializeField] private CinemachineCamera firstPersonCamera;

        //general
        [Header("General Settings")]
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private float clampAngle;
        [SerializeField] private float sprintFOV;
        [SerializeField] private float walkFOV;

        private float rotationY;
        private float mouseDirectionX;
        private float mouseDirectionY;

        //Zooming
        [Header("Zoom Settings")]
        [SerializeField] private bool canZoom;
        [SerializeField] private bool isZoomHold;
        [SerializeField] private float transitionSpeed;
        [SerializeField] private float zoomFOV = 30f;

        private bool isZooming;

        //Head Bobbing effect
        [Header("Head Bob Settings")]
        [SerializeField] private bool canHeadBob;
        [SerializeField] private float headBobAmplitude = 0.01f;
        [SerializeField] private float headBobFrequency = 18.5f;

        private Vector3 headBobStartPosition;

        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            headBobStartPosition = firstPersonCameraRoot.localPosition;
            firstPersonCamera.Lens.FieldOfView = walkFOV;
        }

        private void Update()
        {
            HandleInputs();
            HandleFOVChange();

            if (!canHeadBob || inputManager.IsTryingToSwitchCamera) return;
            HandleHeadBob();
            firstPersonCameraRoot.LookAt(FocusTarget());
        }

        private void LateUpdate()
        {
            rotationY -= mouseDirectionY;
            rotationY = Mathf.Clamp(rotationY, clampAngle * -1f, clampAngle);

            movementController.CharacterController.transform.Rotate(mouseDirectionX * movementController.CharacterController.transform.up);
            firstPersonCameraRoot.localRotation = Quaternion.Euler(rotationY, 0f, 0f);

            inputManager.mouseDirection = Vector2.zero;
        }

        private void HandleInputs()
        {
            mouseDirectionX = inputManager.mouseDirection.x * mouseSensitivity * Time.deltaTime;
            mouseDirectionY = inputManager.mouseDirection.y * mouseSensitivity * Time.deltaTime;

            if (isZoomHold) isZooming = inputManager.isZoomHold;
            else isZooming = inputManager.isZoomTap;
        }

        private void AdjustFOVSettings(float targetFOV)
        {
            float currentFOV = firstPersonCamera.Lens.FieldOfView;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, (transitionSpeed * Time.deltaTime));
            firstPersonCamera.Lens.FieldOfView = newFOV;
        }

        private void HandleFOVChange()
        {
            if (movementController.MovementState == PlayerMovementController.PlayerMovementState.Sprinting)
            {
                AdjustFOVSettings(sprintFOV);
                return;
            }
            if (movementController.MovementState == PlayerMovementController.PlayerMovementState.Walking)
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
            if (movementController.IsMoving)
            {
                Vector3 headBobMotion = Vector3.zero;
                headBobMotion.y += Mathf.Sin(Time.time * headBobFrequency) * (headBobAmplitude * 0.001f);
                headBobMotion.x += Mathf.Cos(Time.time * headBobFrequency / 2) * (headBobAmplitude * 0.001f) * 2;

                firstPersonCameraRoot.localPosition += headBobMotion;
            }
            else if (firstPersonCameraRoot.localPosition != headBobStartPosition)
            {
                firstPersonCameraRoot.localPosition = Vector3.Lerp(firstPersonCameraRoot.localPosition, headBobStartPosition, 1f * Time.deltaTime);
            }
        }

        private Vector3 FocusTarget()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + firstPersonCameraRoot.localPosition.y, transform.position.z);
            pos += firstPersonCameraRoot.forward * 15.0f;
            return pos;
        }
    }
}
