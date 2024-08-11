using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace XtremeFPS.Demo
{
    public class DemoMenu : MonoBehaviour
    {
        public void ResetLevel()
        {
            SceneManager.LoadScene(0);
        }
    }
}
