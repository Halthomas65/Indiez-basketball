using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Switch camera from main to cam2
    public void SwitchCamera()
    {
        Camera mainCam = Camera.main;
        Camera cam2 = GameObject.Find("cameraBallSelect").GetComponent<Camera>();

        if (mainCam != null && cam2 != null)
        {
            mainCam.enabled = !mainCam.enabled;
            cam2.enabled = !cam2.enabled;
        }
    }
    // Switch camera from cam2 to main
    public void SwitchCameraBack()
    {
        Camera mainCam = Camera.main;
        Camera cam2 = GameObject.Find("cameraBallSelect").GetComponent<Camera>();
        if (mainCam != null && cam2 != null)
        {
            mainCam.enabled = !mainCam.enabled;
            cam2.enabled = !cam2.enabled;
        }
    }
}
