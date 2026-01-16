using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XtremeFPS.InputHandling;
using XtremeFPS.Player;
using XtremeFPS.Player.Controller;

namespace XtremeFPS.Demo
{
    public class DemoMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private PlayerMovementController personController;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject staticMenu;
        [SerializeField] private GameObject touchControlMenu;
        [SerializeField] private Slider staminaBar;

        private bool paused;
        bool isCursorLocked;
        private void Start()
        {
            if (playerManager.isCursorLocked)
            {
                this.isCursorLocked = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void Update()
        {
            stateText.text = $"State: {personController.MovementState}";

            float sprintRemainingPercent = personController.SprintRemaining / personController.sprintDuration;
            staminaBar.value = sprintRemainingPercent;

            if (XtremeFPSInputHandler.Instance.Escape)
            {
                if (!paused) Invoke(nameof(Pause), 0.1f);
                else Resume();
            }
        }



        public void Pause()
        {
            pauseMenu.SetActive(true);
            staticMenu.SetActive(false);
            touchControlMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            paused = true;
            Time.timeScale = 0f;
        }

        public void Resume()
        {             
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            staticMenu.SetActive(true);
            if (isCursorLocked) Cursor.lockState = CursorLockMode.Locked;
            Invoke(nameof(IsPaused), 0.1f);
#if IOS || UNITY_ANDROID
            touchControlMenu.SetActive(true);
#endif
        }

        private void IsPaused()
        {
            paused = false;
        }

        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
