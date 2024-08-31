/*Copyright � Spoiled Unknown*/
/*2024*/

using UnityEngine;
using XtremeFPS.PoolingSystem;

namespace XtremeFPS.Demo
{
    [RequireComponent(typeof(Rigidbody))]
    public class ShellEjection : MonoBehaviour
    {
        [SerializeField] private float minForce;
        [SerializeField] private float maxForce;
        [SerializeField] private float lifeTime;
        private Rigidbody rb;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
            float force = Random.Range(minForce, maxForce);
            rb.AddForce(transform.right * force);
            rb.AddTorque(Random.insideUnitSphere * force);

            Invoke(nameof(DestroyShell), lifeTime);
        }

        private void DestroyShell()
        {
            PoolManager.Instance.DespawnObject(this.gameObject);
        }
    }
}
