/*Copyright � Spoiled Unknown*/
/*2024*/

using UnityEngine;
using XtremeFPS.WeaponSystem;

namespace XtremeFPS.Demo
{
    public class MovableGameobjectHit : ShootableObject
    {
        public GameObject particlesPrefab;
        public float impactForce;

        public override void OnHit(RaycastHit hit)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * impactForce, hit.point);
        }
    }
}
