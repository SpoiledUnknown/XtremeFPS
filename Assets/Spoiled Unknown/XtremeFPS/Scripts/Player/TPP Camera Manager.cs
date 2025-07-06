using UnityEngine;
using Unity.Cinemachine;
using XtremeFPS.InputHandling;
using XtremeFPS.Player.Controller;

namespace XtremeFPS.Player.CameraSystem
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/TPP Camera Manager")]
    public class TPPCameraManager : MonoBehaviour
    {
        private XtremeFPSInputHandler inputManager;

        // References
        [Header("References")]
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private CinemachineCamera cinemachineCamera;

        //general
        [Header("General Settings")]
        [SerializeField] private float transitionSpeed;
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private float clampAngle;
        [SerializeField] private float sprintFOV;
        [SerializeField] private float walkFOV;

        private float rotationY;
        private float rotationX;
        private float mouseDirectionX;
        private float mouseDirectionY;

        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            cinemachineCamera.Lens.FieldOfView = walkFOV;
        }

        private void Update()
        {
            mouseDirectionX = inputManager.mouseDirection.x * mouseSensitivity * Time.deltaTime;
            mouseDirectionY = inputManager.mouseDirection.y * mouseSensitivity * Time.deltaTime;

            HandleFOVChange();
        }

        private void LateUpdate()
        {
            cameraRoot.localPosition = playerMovementController.transform.localPosition;

            rotationY -= mouseDirectionY;
            rotationY = Mathf.Clamp(rotationY, clampAngle * -1f, clampAngle);

            cameraRoot.localRotation = Quaternion.Euler(rotationY, (rotationX += mouseDirectionX), 0f);

            inputManager.mouseDirection = Vector2.zero;
        }

        private void AdjustFOVSettings(float targetFOV)
        {
            float currentFOV = cinemachineCamera.Lens.FieldOfView;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, (transitionSpeed * Time.deltaTime));
            cinemachineCamera.Lens.FieldOfView = newFOV;
        }

        private void HandleFOVChange()
        {
            if (playerMovementController.MovementState == PlayerMovementController.PlayerMovementState.Sprinting)
            {
                AdjustFOVSettings(sprintFOV);
                return;
            }
            if (playerMovementController.MovementState == PlayerMovementController.PlayerMovementState.Walking)
            {
                AdjustFOVSettings(walkFOV);
            }
        }
    }
}
