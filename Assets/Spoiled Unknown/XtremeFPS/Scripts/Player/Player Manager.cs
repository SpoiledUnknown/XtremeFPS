using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using XtremeFPS.Player.CameraSystem;
using XtremeFPS.InputHandling;
using XtremeFPS.Interfaces;
using XtremeFPS.WeaponSystem.WeaponHolder;

namespace XtremeFPS.Player
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Player Manager")]
    public class PlayerManager : MonoBehaviour
    {
        private XtremeFPSInputHandler inputManager;

        //Cameras
        [Header("Camera Settings")]
        public bool isCursorLocked;
        public bool canSwitchCameras;
        public bool isTpp;
        
        [Space(10)]
        [SerializeField] private CinemachineCamera firstPersonCamera;
        [SerializeField] private CinemachineCamera thirdPersonCamera;
        [Space(10)]
        [SerializeField] private TPPCameraManager tppCameraManager;
        [SerializeField] private FPPCameraManager fppCameraManager;
        public Transform cameraRoot;

        //Interactions
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange;
        [SerializeField] private LayerMask interactionLayerMask;

        [Header("Weapon Pickup (Optional)")]
        [SerializeField] private WeaponHolder weaponHolder;
        
        public enum CameraMode
        {
            FirstPerson,
            ThirdPerson
        }
        
        public CameraMode CurrentCameraMode { get; private set; }
        
        void Start()
        {
            inputManager = XtremeFPSInputHandler.Instance;
            inputManager.CameraSwitchRequested += OnCameraSwitchRequested;
        }

        void Update()
        {
            InteractionHandling();
        }
        
        private void OnCameraSwitchRequested()
        {
            if (!canSwitchCameras)
                return;

            ToggleCameraMode();
        }
        
        private void ToggleCameraMode()
        {
            CurrentCameraMode =
                CurrentCameraMode == CameraMode.FirstPerson
                    ? CameraMode.ThirdPerson
                    : CameraMode.FirstPerson;

            ApplyCameraMode();
        }
        
        private void ApplyCameraMode()
        {
            isTpp = CurrentCameraMode == CameraMode.ThirdPerson;

            tppCameraManager.enabled = isTpp;
            fppCameraManager.enabled = !isTpp;

            thirdPersonCamera.Priority = isTpp ? 1 : 0;
            firstPersonCamera.Priority = isTpp ? 0 : 1;

            cameraRoot.SetParent(
                isTpp ? transform.parent : transform
            );
        }
        
        private void InteractionHandling()
        {
            if (!inputManager.IsTryingToInteract) return;

            if (inputManager.IsAimHold || inputManager.IsAimTap) return;

            foreach (var pickup in GetNearbyWeaponPickups())
            {
                if (TryHandleWeaponPickup(pickup)) break;
            }
        }
        
        private IEnumerable<IWeaponPickup> GetNearbyWeaponPickups()
        {
            Collider[] colliders = Physics.OverlapSphere( transform.position, interactionRange, interactionLayerMask);

            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out IWeaponPickup pickup)) yield return pickup;
            }
        }
        
        private bool TryHandleWeaponPickup(IWeaponPickup pickup)
        {
            int weaponCount = weaponHolder.GetWeaponCount();

            if (!pickup.IsEquiped() && weaponCount < 3)
            {
                pickup.Equip();
                weaponHolder.SelectWeapon();
                return true;
            }

            if (pickup.IsEquiped() && pickup.IsActive() && weaponCount == 3)
            {
                pickup.Drop();
                weaponHolder.SelectWeapon();
                return true;
            }

            return false;
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
