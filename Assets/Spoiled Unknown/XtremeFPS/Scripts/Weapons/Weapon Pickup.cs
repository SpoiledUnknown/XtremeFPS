using TMPro;
using UnityEngine;
using XtremeFPS.Interfaces;
using XtremeFPS.WeaponSystem.Effects;

namespace XtremeFPS.WeaponSystem.Pickup
{
    [RequireComponent(typeof(UniversalWeaponSystem))]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Weapon Pickup")]
    public class WeaponPickup : MonoBehaviour, IWeaponPickup
    {
        #region Variables
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private TextMeshProUGUI bulletText;

        [Header("Pickup Settings")]
        [SerializeField] private bool equipped;
        [SerializeField] private float dropForwardForce;
        [SerializeField] private float dropUpwardForce;
        [SerializeField] private float dropTorqueMultiplier;

        private UniversalWeaponSystem weaponSystem;
        private EffectsManager effects;
        private BoxCollider Collider;
        private Rigidbody rb;
        #endregion

        #region Monobehaviour Callbacks
        private void Start()
        {
            Collider = GetComponent<BoxCollider>();
            weaponSystem = GetComponent<UniversalWeaponSystem>();
            effects = GetComponentInChildren<EffectsManager>();

            if (equipped) Equip();
            else UnEquip();
        }
        #endregion

        private void UnEquip()
        {
            if (!gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            weaponSystem.enabled = false;
            effects.enabled = false;
            Collider.isTrigger = false;
            equipped = false;
        }

        public void Equip()
        {
            Destroy(rb);
            equipped = true;
            weaponSystem.enabled = true;
            effects.enabled = true;
            Collider.isTrigger = true;
            transform.SetParent(weaponHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        public void Drop()
        {
            UnEquip();
            bulletText.SetText("00 / 00");
            transform.SetParent(null);

            rb.linearVelocity = characterController.velocity;
            rb.AddForce(cameraRoot.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(cameraRoot.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            rb.AddTorque(new Vector3(random, random, random) * dropTorqueMultiplier);
        }

        public bool IsEquiped()
        {
            return equipped;
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}
