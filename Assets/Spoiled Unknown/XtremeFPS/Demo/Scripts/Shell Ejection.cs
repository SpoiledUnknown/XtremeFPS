/*Copyright � Spoiled Unknown*/
/*2024*/

using UnityEngine;
using XtremeFPS.Common.Pool;

[RequireComponent(typeof(Rigidbody))]
public class ShellEjection : MonoBehaviour
{
    public float minForce;
    public float maxForce;
    public float lifeTime;
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
        PoolManager.Instance.ReturnObjectToPool(this.gameObject);
    }
}