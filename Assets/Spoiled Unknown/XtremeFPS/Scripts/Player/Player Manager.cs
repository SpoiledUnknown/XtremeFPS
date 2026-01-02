using Unity.Cinemachine;
using UnityEngine;
using XtremeFPS.Player.CameraSystem;
using XtremeFPS.InputHandling;
using XtremeFPS.Interfaces;
using XtremeFPS.WeaponSystem.WeaponHolder;

namespace XtremeFPS.Player
{
    /// <summary>
    /// A temporary player manager script that handles camera switching, interactions, and weapon pickups.
    /// I am still figuring out how to implement a proper player manager.
    /// </summary>
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Player Manager")]
    public class PlayerManager : MonoBehaviour
    {
        private XtremeFPSInputHandler inputManager;

        //Cameras
        [Header("Camera Settings")]
        public bool isCursorLocked;
        
        [Space(10)]
        public bool canSwitchCameras;
        [SerializeField] private CinemachineCamera firstPersonCamera;
        [SerializeField] private CinemachineCamera thirdPersonCamera;
        [Space(10)]
        [SerializeField] private TPPCameraManager tppCameraManager;
        [SerializeField] private FPPCameraManager fppCameraManager;

        //Interactions
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange;
        [SerializeField] private LayerMask interactionLayerMask;

        [Header("Weapon Pickup (Optional)")]
        [SerializeField] private WeaponHolder weaponHolder;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            /*
             * TODO: Find a better way to handle FPP and TPP switching
             */
            if (canSwitchCameras)
            {
                if (inputManager.IsTryingToSwitchCamera)
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


            InteractionHandling();
        }

        private void InteractionHandling()
        {
            bool Aiming = inputManager.IsAimHold || inputManager.IsAimTap;
            if (inputManager.IsTryingToInteract)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayerMask);

                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out IWeaponPickup pickup) && !Aiming)
                    {
                        if (!pickup.IsEquiped() && weaponHolder.GetWeaponCount() < 3) 
                        {
                            pickup.Equip();
                            weaponHolder.SelectWeapon();
                            break;
                        }

                        if (pickup.IsEquiped() && pickup.IsActive() && weaponHolder.GetWeaponCount() == 3)
                        {
                            pickup.Drop();
                            weaponHolder.SelectWeapon();
                            break;
                        }
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
