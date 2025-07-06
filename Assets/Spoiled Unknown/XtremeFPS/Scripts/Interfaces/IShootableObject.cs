using UnityEngine;

namespace XtremeFPS.Interfaces
{
    public interface IShootableObject
    {
        void OnHit(RaycastHit hit, float impactForce);
    }
}
