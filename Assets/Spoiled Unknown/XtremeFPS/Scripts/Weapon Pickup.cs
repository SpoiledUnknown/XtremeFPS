/*Copyright � Spoiled Unknown*/
/*2024*/

using TMPro;
using UnityEngine;
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
        public float dropTorqueMultiplier;

        private UniversalWeaponSystem weaponSystem;
        private BoxCollider Collider;
        private Rigidbody rb;
        #endregion

        #region Monobehaviour Callbacks
        private void Start()
        {
            Collider = GetComponent<BoxCollider>();
            weaponSystem = GetComponent<UniversalWeaponSystem>();

            if (equipped) Equip();
            else UnEquip();
        }
        #endregion

        #region Private methods
        private void UnEquip()
        {
            if (!gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            weaponSystem.enabled = false;
            Collider.isTrigger = false;
            equipped = false;
            IsWeaponEquipped = false;
        }

        private void Equip()
        {
            Destroy(rb);
            equipped = true;
            weaponSystem.enabled = true;
            Collider.isTrigger = true;
            IsWeaponEquipped = true;
        }

        public void PickUp()
        {
            Equip();
            transform.SetParent(weaponHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        public void Drop()
        {
            UnEquip();
            bulletText.SetText("00 / 00");
            transform.SetParent(null);

            rb.linearVelocity = playerArmature.velocity;
            rb.AddForce(cameraRoot.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(cameraRoot.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            rb.AddTorque(new Vector3(random, random, random) * dropTorqueMultiplier);
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
