/*Copyright © Spoiled Unknown*/
/*2024*/

using TMPro;
using UnityEngine;
using XtremeFPS.InputHandler;
using XtremeFPS.Interfaces;

namespace XtremeFPS.WeaponSystem.Pickup
{
    [RequireComponent(typeof(UniversalWeaponSystem))]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Weapon Pickup")]
    public class WeaponPickup : MonoBehaviour, IPickup
    {
        #region Variables
        public static bool IsWeaponEquipped { get; private set; }
        public CharacterController playerArmature;
        public Transform weaponHolder;
        public Transform cameraRoot;
        public TextMeshProUGUI bulletText;

        public bool equipped;
        public int Priority;
        public float dropForwardForce;
        public float dropUpwardForce;

        private UniversalWeaponSystem weaponSystem;
        private BoxCollider Collider;
        private Vector3 currentVelocity;
        private Vector3 angularVelocity;
        #endregion

        #region Monobehaviour Callbacks
        private void Start()
        {
            Collider = GetComponent<BoxCollider>();
            weaponSystem = GetComponent<UniversalWeaponSystem>();

            if (!equipped) UnEquip();
            else Equip();
        }

        private void FixedUpdate()
        {
            if (equipped) return;
            if (Physics.CheckBox(Collider.center, Collider.size * 0.5f, Quaternion.identity)) return;
            if (Physics.Raycast(new Ray(transform.position, currentVelocity.normalized), out RaycastHit hit, Collider.size.magnitude * 0.45f))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle < 10f)
                {
                    currentVelocity = Vector3.zero;
                    angularVelocity = Vector3.zero;
                    return;
                }
                currentVelocity = Vector3.Reflect(currentVelocity, hit.normal);
                angularVelocity = Vector3.Reflect(angularVelocity, hit.normal);
            }
            transform.position += currentVelocity * Time.deltaTime;
            transform.Rotate(angularVelocity * Time.deltaTime);
            currentVelocity += Physics.gravity * Time.deltaTime;
        }
        #endregion

        #region Private methods
        private void UnEquip()
        {
            weaponSystem.enabled = false;
            Collider.isTrigger = false;
        }

        private void Equip()
        {
            weaponSystem.enabled = true;
            Collider.isTrigger = true;
            IsWeaponEquipped = true;
        }

        public void PickUp()
        {
            Equip();
            equipped = true;
            transform.SetParent(weaponHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        public void Drop()
        {
            bulletText.SetText("00 / 00");
            equipped = false;
            IsWeaponEquipped = false;
            transform.SetParent(null);
            float random = Random.Range(-1f, 1f);
            weaponSystem.animator.gameObject.SetActive(true);
            if (weaponSystem.aimUIImage != null) weaponSystem.aimUIImage.SetActive(false);

            currentVelocity = cameraRoot.forward * dropForwardForce + cameraRoot.up * dropUpwardForce;
            angularVelocity = new Vector3(random, random, random) * 100f;
            UnEquip();
        }

        public bool IsEquiped()
        {
            return equipped;
        }

        public Transform GetTransform()
        {
            return transform;
        }
        #endregion
    }
}
