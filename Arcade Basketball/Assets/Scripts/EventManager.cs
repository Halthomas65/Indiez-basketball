using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;
    public int manager = 2; // 1 for cam1, 2 for cam2

    public GameObject mainUI; 
    public GameObject ballSelectUI;

    private TouchManager touchManager;
    private ScoreManager scoreManager;
    private BallSelectionTouch ballSelectionTouch;

    void Start()
    {
        touchManager = GetComponent<TouchManager>();
        scoreManager = GetComponent<ScoreManager>();
        ballSelectionTouch = GetComponent<BallSelectionTouch>();
        
        // Initial setup
        ballSelectionTouch.enabled = false;
    }

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
            touchManager.enabled = true;
            scoreManager.enabled = true;
            mainUI.SetActive(true);
            ballSelectUI.SetActive(false);
            ballSelectionTouch.enabled = false;
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
            touchManager.enabled = false;
            scoreManager.enabled = false;
            mainUI.SetActive(false);
            ballSelectUI.SetActive(true);
            ballSelectionTouch.enabled = true;
        }
        if (cam2 != null)
        {
            cam2.SetActive(true);
            // Ensure the camera component is enabled
            Camera ballSelectCam = cam2.GetComponent<Camera>();
            if (ballSelectCam != null)
            {
                ballSelectCam.enabled = true;
            }
        }
    }
}
