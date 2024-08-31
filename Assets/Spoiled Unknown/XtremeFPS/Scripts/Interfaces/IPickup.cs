using UnityEngine;

namespace XtremeFPS.Interfaces
{
    public interface IPickup
    {
        void PickUp();
        void Drop();
        bool IsEquiped();
        Transform GetTransform();
    }
}