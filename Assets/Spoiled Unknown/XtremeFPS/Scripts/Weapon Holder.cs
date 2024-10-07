using System;
using UnityEngine;
using XtremeFPS.InputHandling;

namespace XtremeFPS.WeaponSystem.Holder
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Weapon Holder")]
    public class WeaponHolder : MonoBehaviour
    {
        public static WeaponHolder Instance { get; private set; }

        private int selectedWeapon;
        private XtremeFPSInputHandler inputHandler;

        private void Start()
        {
            if (Instance == null) Instance = this;
            else Destroy(Instance);

            inputHandler = XtremeFPSInputHandler.Instance;
            SelectWeapon();
        }

        private void Update()
        {
            int previouslySelectedWeapon = selectedWeapon;
            if (inputHandler.MouseScroll > 0)
            {
                if (selectedWeapon >= GetWeaponCount() - 1) selectedWeapon = 0;
                else selectedWeapon++;
            }

            if (inputHandler.MouseScroll < 0)
            {
                if (selectedWeapon <= 0) selectedWeapon = GetWeaponCount() - 1;
                else selectedWeapon--;
            }

            if (inputHandler.isTryingToInteractAlternate) Invoke(nameof(SelectWeapon), 0.25f);
            if (inputHandler.isTryingToInteract) SelectWeapon();
            if (previouslySelectedWeapon != selectedWeapon) SelectWeapon();
        }

        public void SelectWeapon()
        {
            int i = 0;
            foreach (Transform weapons in transform)
            {
                if (i == selectedWeapon) weapons.gameObject.SetActive(true);
                else weapons.gameObject.SetActive(false);

                i++;
            }
        }

        public int GetWeaponCount()
        {
            return transform.childCount;
        }
    }
}
