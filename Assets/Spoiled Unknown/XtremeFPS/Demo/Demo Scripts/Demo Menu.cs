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

        bool paused;

        private void Update()
        {
            stateText.text = $"State: {personController.MovementState}";

            float sprintRemainingPercent = personController.sprintRemaining / personController.sprintDuration;
            staminaBar.value = sprintRemainingPercent;

            if (XtremeFPSInputHandler.Instance.escape)
            {
                if (!pauseMenu.activeSelf && !paused)
                {
                    Pause();
                    Invoke(nameof(IsPaused), 0.1f);
                    return;
                }
                else
                {
                    Resume();
                    Invoke(nameof(IsPaused), 0.1f);
                }
            }

            Cursor.lockState = (playerManager.isCursorLocked && !paused) ? CursorLockMode.Locked : CursorLockMode.None;
        }

        void IsPaused()
        {
            paused = !paused;
        }

        public void Pause()
        {
            pauseMenu.SetActive(true);
            staticMenu.SetActive(false);
            touchControlMenu.SetActive(false);
            Time.timeScale = 0f;
        }

        public void Resume()
        {             
            pauseMenu.SetActive(false);
            staticMenu.SetActive(true);
#if IOS || UNITY_ANDROID
            touchControlMenu.SetActive(true);
#endif
            Time.timeScale = 1f;
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
