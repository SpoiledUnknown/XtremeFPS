using UnityEngine;
using XtremeFPS.InputHandling;

public class CameraTest : MonoBehaviour
{
    public GameObject Eye;

    // Update is called once per frame
    void Update()
    {
        if (!XtremeFPSInputHandler.Instance.IsSwitchingCamera) Eye.SetActive(false);
        else Eye.SetActive(true);
    }
}
