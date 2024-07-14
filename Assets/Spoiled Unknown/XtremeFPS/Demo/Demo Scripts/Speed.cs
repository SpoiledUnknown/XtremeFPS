using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XtremeFPS.FirstPersonController;

public class Speed : MonoBehaviour
{
    public TextMeshProUGUI speedCount;
    public FirstPersonController firstPersonController;

    // Update is called once per frame
    void Update()
    {
        speedCount.text = firstPersonController.targetSpeed.ToString();
    }
}
