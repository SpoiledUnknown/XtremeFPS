using UnityEngine;

namespace XtremeFPS.Interfaces
{
    public interface IWeaponPickup
    {
        void PickUp();
        void Drop();
        bool IsEquiped();
        bool IsActive();
    }
}