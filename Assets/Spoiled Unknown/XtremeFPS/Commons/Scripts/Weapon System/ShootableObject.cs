/*Copyright © Spoiled Unknown*/
/*2024*/

using UnityEngine;

namespace XtremeFPS.Common.WeaponSystem
{
    public abstract class ShootableObject : MonoBehaviour
    {
        public abstract void OnHit(RaycastHit hit);
    }
}

