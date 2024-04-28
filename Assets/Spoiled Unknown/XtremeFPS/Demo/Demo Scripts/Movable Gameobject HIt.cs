/*Copyright � Spoiled Unknown*/
/*2024*/

using UnityEngine;
//using XtremeFPS.Common.PoolingSystem;
using XtremeFPS.WeaponSystem;

namespace XtremeFPS.Demo
{
    public class MovableGameobjectHit : ShootableObject
    {
        public GameObject particlesPrefab;
        public float impactForce;

        public override void OnHit(RaycastHit hit)
        {
            //PoolManager.Spawn(particlesPrefab.transform, hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal));

            GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * impactForce, hit.point);
        }
    }
}


