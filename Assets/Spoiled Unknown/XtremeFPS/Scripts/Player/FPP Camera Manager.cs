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
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private CinemachineCamera cinemachineCamera;

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
            headBobStartPosition = cameraRoot.localPosition;
            cinemachineCamera.Lens.FieldOfView = walkFOV;
        }

        private void Update()
        {
            HandleInputs();
            HandleFOVChange();

            if (!canHeadBob || inputManager.isTryingToSwitchCamera) return;
            HandleHeadBob();
            cameraRoot.LookAt(FocusTarget());
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            
            rotationY -= mouseDirectionY * dt;
            rotationY = Mathf.Clamp(rotationY, clampAngle * -1f, clampAngle);

            movementController.CharacterController.transform.Rotate(Vector3.up * (mouseDirectionX * dt));
            cameraRoot.localRotation = Quaternion.Euler(rotationY, 0f, 0f);
            
            inputManager.mouseDirection = Vector2.zero;
            
            //if (inputManager.isUsingTouchscreen) inputManager.mouseDirection = Vector2.zero;
        }

        private void HandleInputs()
        {
            mouseDirectionX = inputManager.mouseDirection.x * mouseSensitivity;
            mouseDirectionY = inputManager.mouseDirection.y * mouseSensitivity;

            if (isZoomHold) isZooming = inputManager.isZoomHold;
            else isZooming = inputManager.isZoomTap;
        }

        private void AdjustFOVSettings(float targetFOV)
        {
            float currentFOV = cinemachineCamera.Lens.FieldOfView;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, (transitionSpeed * Time.deltaTime));
            cinemachineCamera.Lens.FieldOfView = newFOV;
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

                cameraRoot.localPosition += headBobMotion;
            }
            else if (cameraRoot.localPosition != headBobStartPosition)
            {
                cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, headBobStartPosition, 1f * Time.deltaTime);
            }
        }

        private Vector3 FocusTarget()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraRoot.localPosition.y, transform.position.z);
            pos += cameraRoot.forward * 15.0f;
            return pos;
        }
    }
}
