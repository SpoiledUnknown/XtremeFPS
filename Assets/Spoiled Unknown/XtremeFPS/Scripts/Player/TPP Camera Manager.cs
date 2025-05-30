using UnityEngine;
using Unity.Cinemachine;
using XtremeFPS.InputHandling;

namespace XtremeFPS.CameraSystem
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/TPP Camera Manager")]
    public class TPPCameraManager : MonoBehaviour
    {
        private XtremeFPSInputHandler inputManager;

        // References
        public CharacterController characterController;
        public Transform ThirdPersonCameraRoot;
        public CinemachineCamera ThirdPersonCamera;

        //general
        public bool isCursorLocked;
        public float transitionSpeed;
        public float mouseSensitivity;
        public float clampAngle;
        public float sprintFOV;
        public float walkFOV;

        private float rotationY;
        private float rotationX;
        private float mouseDirectionX;
        private float mouseDirectionY;
        private float vRecoil = 0f;
        private float hRecoil = 0f;
        private Vector3 horizontalVelocity;

        private void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;

            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;

            ThirdPersonCamera.Lens.FieldOfView = walkFOV;
        }

        private void Update()
        {
            horizontalVelocity = characterController.velocity;
            horizontalVelocity.y = 0f;

            mouseDirectionX = inputManager.mouseDirection.x * mouseSensitivity * Time.deltaTime + hRecoil;
            mouseDirectionY = inputManager.mouseDirection.y * mouseSensitivity * Time.deltaTime + vRecoil;

            HandleFOVChange();
        }

        private void LateUpdate()
        {
            ThirdPersonCameraRoot.localPosition = characterController.transform.localPosition;

            rotationY -= mouseDirectionY;
            rotationY = Mathf.Clamp(rotationY, clampAngle * -1f, clampAngle);

            ThirdPersonCameraRoot.localRotation = Quaternion.Euler(rotationY, (rotationX += mouseDirectionX), 0f);

            inputManager.mouseDirection = Vector2.zero;
        }

        public void AddRecoil(float hRecoil, float vRecoil)
        {
            this.hRecoil = hRecoil;
            this.vRecoil = vRecoil;
        }

        private void AdjustFOVSettings(float targetFOV)
        {
            float currentFOV = ThirdPersonCamera.Lens.FieldOfView;
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, (transitionSpeed * Time.deltaTime));
            ThirdPersonCamera.Lens.FieldOfView = newFOV;
        }

        private void HandleFOVChange()
        {
            if (Mathf.RoundToInt(horizontalVelocity.magnitude) >= 4)
            {
                AdjustFOVSettings(sprintFOV);
                return;
            }
            if (Mathf.RoundToInt(horizontalVelocity.magnitude) >= 2)
            {
                AdjustFOVSettings(walkFOV);
            }
        }
    }
}
