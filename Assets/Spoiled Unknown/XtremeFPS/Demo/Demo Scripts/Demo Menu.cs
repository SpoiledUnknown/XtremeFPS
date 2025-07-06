using System;
using TMPro;
using UnityEngine;
using XtremeFPS.Player.Controller;

namespace XtremeFPS.Demo
{
    public class DemoMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private PlayerMovementController personController;

        private void Update()
        {
                stateText.text = $"State: {personController.MovementState}";
        }
    }
}
