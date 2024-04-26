/*Copyright © Spoiled Unknown*/
/*2024*/

using UnityEngine;

namespace XtremeFPS.WeaponSystem
{
    public abstract class ShootableObject : MonoBehaviour
    {
        public abstract void OnHit(RaycastHit hit);
    }
}

