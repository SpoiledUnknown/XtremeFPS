using System;
using TMPro;
using UnityEngine;
using XtremeFPS.FPSController;
using XtremeFPS.WeaponSystem;

namespace XtremeFPS.Demo
{
    public class DemoMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI surfaceText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI bulletText;
        [SerializeField] private FirstPersonController personController;
        [SerializeField] private UniversalWeaponSystem weaponSystem;

        private void Update()
        {
            stateText.text = $"State: {personController.MovementState}";
            surfaceText.text = $"Surface: {personController.SurfaceType}";
            speedText.text = $"Speed: {personController.targetSpeed}";
            CalculateBulletShellsAndSetTheText();
        }

        private void CalculateBulletShellsAndSetTheText()
        {
            if (bulletText == null || weaponSystem == null || !weaponSystem.enabled) return;
            bulletText.text = $"{weaponSystem.BulletsLeft / weaponSystem.bulletsPerTap} / {weaponSystem.totalBullets / weaponSystem.bulletsPerTap}";
        }
    }
}
