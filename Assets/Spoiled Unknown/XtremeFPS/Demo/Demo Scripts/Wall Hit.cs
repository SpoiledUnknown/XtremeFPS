/*Copyright © Spoiled Unknown*/
/*2024*/

using UnityEngine;
using XtremeFPS.WeaponSystem;

namespace XtremeFPS.Demo
{
    public class WallHit : ShootableObject
    {
        public GameObject particlesPrefab;

        public override void OnHit(RaycastHit hit)
        {
            //Spawn(particlesPrefab.transform, hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal));
        }
    }
}

