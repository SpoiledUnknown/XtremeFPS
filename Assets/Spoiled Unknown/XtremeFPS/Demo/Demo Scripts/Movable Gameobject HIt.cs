/*Copyright � Spoiled Unknown*/
/*2024*/

using UnityEngine;
using XtremeFPS.WeaponSystem;

namespace XtremeFPS.Demo
{
    public class MovableGameobjectHit : ShootableObject
    {
        [SerializeField] private GameObject particlesPrefab;
        [SerializeField] private float impactForce;

        public override void OnHit(RaycastHit hit)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * impactForce, hit.point);
        }
    }
}
