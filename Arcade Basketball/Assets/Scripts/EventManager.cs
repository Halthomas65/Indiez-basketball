using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;
    public int manager = 1; // 1 for cam1, 2 for cam2

    public void SwitchCamera()
    {
        GetComponent<Animator>().SetBool("isSelecting", true);
    }

    public void ManageCam()
    {
        if (manager == 1)
        {
            Cam_1();
            manager = 2;
        }
        else if (manager == 2)
        {
            Cam_2();
            manager = 1;
        }
    }

    public void Cam_1()
    {
        if (cam1 != null)
        {
            cam1.SetActive(true);
        }
        if (cam2 != null)
        {
            cam2.SetActive(false);
        }
    }

    public void Cam_2()
    {
        if (cam1 != null)
        {
            cam1.SetActive(false);
        }
        if (cam2 != null)
        {
            cam2.SetActive(true);
        }
    }
}
