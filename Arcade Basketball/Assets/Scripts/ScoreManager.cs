using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Canvas worldSpaceCanvas;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SetupCanvas();
    }

    private void SetupCanvas()
    {
        if (worldSpaceCanvas == null)
            worldSpaceCanvas = GetComponentInParent<Canvas>();

        if (worldSpaceCanvas != null)
        {
            worldSpaceCanvas.worldCamera = mainCamera;
            worldSpaceCanvas.planeDistance = 100f;

            // // Position the canvas in front of the camera
            // RectTransform rectTransform = worldSpaceCanvas.GetComponent<RectTransform>();
            // if (rectTransform != null)
            // {
            //     // Adjust these values based on your scene
            //     rectTransform.localPosition = new Vector3(0, 0, 47);
            //     rectTransform.localRotation = Quaternion.Euler(0, 180, 0);
            //     rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            // }
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
