/*Copyright � Spoiled Unknown*/
/*2024*/

using UnityEngine;
using XtremeFPS.Interfaces;

namespace XtremeFPS.Demo
{
    public class MovableGameobjectHit : MonoBehaviour, IShootableObject
    {
        public void OnHit(RaycastHit hit, float impactForce)
        {
            this.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * (impactForce * 10f), hit.point);
        }
    }
}
