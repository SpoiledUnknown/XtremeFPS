using Unity.Cinemachine;
using UnityEngine;
using XtremeFPS.Player.CameraSystem;
using XtremeFPS.InputHandling;

namespace XtremeFPS.Player
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Player")]
    public class Player : MonoBehaviour
    {
        public CinemachineCamera firstPersonCamera;
        public CinemachineCamera thirdPersonCamera;

        public TPPCameraManager tppCameraManager;
        public FPPCameraManager fppCameraManager;

        //Interactions
        public float interactionRange;
        public int interactionLayerId;

        public bool isCursorLocked;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        // Update is called once per frame
        void Update()
        {
            if (XtremeFPSInputHandler.Instance.IsSwitchingCamera)
            {
                tppCameraManager.enabled = true;
                fppCameraManager.enabled = false;

                thirdPersonCamera.Priority = 1;
                firstPersonCamera.Priority = 0;
            }
            else
            {
                tppCameraManager.enabled = false;
                fppCameraManager.enabled = true;

                thirdPersonCamera.Priority = 0;
                firstPersonCamera.Priority = 1;
            }
        }

        private void InteractionHandling()
        {
            if (inputManager.isTryingToInteract)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayerMask);

                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out IWeaponPickup pickup) && !isZoomed)
                    {
                        if (pickup.IsEquiped()) continue;
                        if (WeaponHolder.Instance.GetWeaponCount() < 3) pickup.PickUp();
                        break;
                    }
                }
            }
            else if (inputManager.isTryingToInteractAlternate)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayerMask);

                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out IWeaponPickup pickup) && !isZoomed)
                    {
                        if (pickup.IsEquiped() && pickup.IsActive()) pickup.Drop();
                        break;
                    }
                }
            }
        }
    }
}
