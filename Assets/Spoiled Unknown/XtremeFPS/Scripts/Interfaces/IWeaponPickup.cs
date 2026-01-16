namespace XtremeFPS.Interfaces
{
    public interface IWeaponPickup
    {
        void Equip();
        void Drop();
        bool IsEquiped();
        bool IsActive();
    }
}