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
            int previouslySelectedWeapon = selectedWeapon;
            if (inputHandler.mouseScrollDirection > 0)
            {
                if (selectedWeapon >= GetWeaponCount() - 1) selectedWeapon = 0;
                else selectedWeapon++;
            }

            if (inputHandler.mouseScrollDirection < 0)
            {
                if (selectedWeapon <= 0) selectedWeapon = GetWeaponCount() - 1;
                else selectedWeapon--;
            }

            if (previouslySelectedWeapon != selectedWeapon) SelectWeapon();
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
