using UnityEngine;

namespace XtremeFPS.InputHandling.TouchControls
{
    public class TouchControlActivator : MonoBehaviour
    {
        void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(true);
#endif
            gameObject.SetActive(false);
        }
    }
}
