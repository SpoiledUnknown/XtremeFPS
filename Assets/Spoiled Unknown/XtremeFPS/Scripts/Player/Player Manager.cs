using Unity.Cinemachine;
using UnityEngine;
using XtremeFPS.Player.CameraSystem;
using XtremeFPS.InputHandling;
using XtremeFPS.Interfaces;
using XtremeFPS.WeaponSystem.Holder;

namespace XtremeFPS.Player
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Player Manager")]
    public class PlayerManager : MonoBehaviour
    {
        private XtremeFPSInputHandler inputManager;

        //Cameras
        [Header("Camera Settings")]
        [SerializeField] private bool isCursorLocked;
        [Space(10)]
        [SerializeField] private CinemachineCamera firstPersonCamera;
        [SerializeField] private CinemachineCamera thirdPersonCamera;
        [Space(10)]
        [SerializeField] private TPPCameraManager tppCameraManager;
        [SerializeField] private FPPCameraManager fppCameraManager;

        //Interactions
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange;
        [SerializeField] private int interactionLayerId;


        private LayerMask interactionLayerMask;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;

            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        // Update is called once per frame
        void Update()
        {
            if (XtremeFPSInputHandler.Instance.IsTryingToSwitchCamera)
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

            InteractionHandling();
        }

        private void InteractionHandling()
        {
            bool Aiming = inputManager.isAimHold || inputManager.isAimTap;
            if (inputManager.isTryingToInteract)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayerMask);

                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out IWeaponPickup pickup) && !Aiming)
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
                    if (collider.TryGetComponent(out IWeaponPickup pickup) && !Aiming)
                    {
                        if (pickup.IsEquiped() && pickup.IsActive()) pickup.Drop();
                        break;
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
#endif
    }
}
