using UnityEngine;
using XtremeFPS.InputHandling;

namespace XtremeFPS.WeaponSystem.WeaponHolder
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Weapon Holder")]
    public class WeaponHolder : MonoBehaviour
    {
        private int selectedWeapon;
        private XtremeFPSInputHandler inputHandler;

        private void Start()
        {
            inputHandler = XtremeFPSInputHandler.Instance;
            SelectWeapon();
        }

        private void Update()
        {
            int delta = inputHandler.WeaponCycleDelta;
            if (delta == 0)
                return;

            int previousWeapon = selectedWeapon;

            selectedWeapon = (selectedWeapon + delta) % GetWeaponCount();
            if (selectedWeapon < 0)
                selectedWeapon += GetWeaponCount();

            if (previousWeapon != selectedWeapon)
                SelectWeapon();

            inputHandler.ResetWeaponCycle();
        }


        public void SelectWeapon()
        {
            int i = 0;
            foreach (Transform weapons in transform)
            {
                weapons.gameObject.SetActive(i == selectedWeapon);
                i++;
            }
        }

        public int GetWeaponCount()
        {
            return transform.childCount;
        }
    }
}
