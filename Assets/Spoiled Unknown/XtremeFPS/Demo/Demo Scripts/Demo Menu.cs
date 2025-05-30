using System;
using TMPro;
using UnityEngine;
using XtremeFPS.Controller;
using XtremeFPS.InputHandling;
using XtremeFPS.WeaponSystem;

namespace XtremeFPS.Demo
{
    public class DemoMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI surfaceText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private MovementController personController;

        private void Update()
        {
                stateText.text = $"State: {personController.MovementState}";
                surfaceText.text = $"Surface: {personController.SurfaceType}";
                speedText.text = $"Speed: {personController.targetSpeed}";
        }
    }
}
