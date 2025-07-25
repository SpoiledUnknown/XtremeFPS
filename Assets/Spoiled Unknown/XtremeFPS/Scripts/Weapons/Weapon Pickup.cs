using TMPro;
using UnityEngine;
using XtremeFPS.Interfaces;

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
        private BoxCollider Collider;
        private Rigidbody rb;
        #endregion

        #region Monobehaviour Callbacks
        private void Start()
        {
            Collider = GetComponent<BoxCollider>();
            weaponSystem = GetComponent<UniversalWeaponSystem>();

            if (equipped) PickUp();
            else Drop();
        }
        #endregion
        public void PickUp()
        {
            Destroy(rb);
            equipped = true;
            weaponSystem.enabled = true;
            Collider.isTrigger = true;
            transform.SetParent(weaponHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        public void Drop()
        {
            bulletText.SetText("00 / 00");
            transform.SetParent(null);
            if (!gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            weaponSystem.enabled = false;
            Collider.isTrigger = false;
            equipped = false;

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
