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
        public MovementController movementController;
        public Transform ThirdPersonCameraRoot;
        public CinemachineCamera ThirdPersonCamera;

        //general
        public float transitionSpeed;
        public float mouseSensitivity;
        public float clampAngle;
        public float sprintFOV;
        public float walkFOV;

        private float rotationY;
        private float rotationX;
        private float mouseDirectionX;
        private float mouseDirectionY;

        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            ThirdPersonCamera.Lens.FieldOfView = walkFOV;
        }

        private void Update()
        {
            mouseDirectionX = inputManager.mouseDirection.x * mouseSensitivity * Time.deltaTime;
            mouseDirectionY = inputManager.mouseDirection.y * mouseSensitivity * Time.deltaTime;

            HandleFOVChange();
        }

        private void LateUpdate()
        {
            ThirdPersonCameraRoot.localPosition = movementController.transform.localPosition;

            rotationY -= mouseDirectionY;
            rotationY = Mathf.Clamp(rotationY, clampAngle * -1f, clampAngle);

            ThirdPersonCameraRoot.localRotation = Quaternion.Euler(rotationY, (rotationX += mouseDirectionX), 0f);

            inputManager.mouseDirection = Vector2.zero;
        }

        private void AdjustFOVSettings(float targetFOV)
        {
            float currentFOV = ThirdPersonCamera.Lens.FieldOfView;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, (transitionSpeed * Time.deltaTime));
            ThirdPersonCamera.Lens.FieldOfView = newFOV;
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
        }
    }
}
