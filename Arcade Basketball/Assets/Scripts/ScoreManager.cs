using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro highScoreText;    // Changed from TextMeshProUGUI
    [SerializeField] private TextMeshPro timeText;         // Changed from TextMeshProUGUI
    [SerializeField] private TextMeshPro scoreText;        // Changed from TextMeshProUGUI
    [SerializeField] private GameObject perfectDunkObject;
    [SerializeField] private Canvas worldSpaceCanvas;

    [SerializeField] int timeRemain = 30; // Time limit in seconds
    int score = 0;
    int highScore = 0;
    bool isPerfectDunk = false;

    [SerializeField] private Transform topTrigger;    // Add reference to top trigger
    [SerializeField] private Transform bottomTrigger; // Add reference to bottom trigger
    private Dictionary<int, bool> ballPassedTop = new Dictionary<int, bool>();
    private Dictionary<int, bool> ballTouchedRing = new Dictionary<int, bool>();

    private const string HIGHSCORE_FILE = "highscore.dat";
    private string highScoreFilePath;

    private bool isGameActive = false;
    private float currentTime;

    [Header("Dunk Settings")]
    [SerializeField] private float quickDunkThreshold = 0.07f; // Time threshold for quick dunks
    private Dictionary<int, float> ballTopTriggerTime = new Dictionary<int, float>();

    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
        if (perfectDunkObject != null)
            perfectDunkObject.SetActive(false); // Ensure hidden at start
    }

    void Awake()
    {
        // Set the file path in persistent data path
        highScoreFilePath = Path.Combine(Application.persistentDataPath, HIGHSCORE_FILE);
        LoadHighScore();
    }

    private void LoadHighScore()
    {
        try
        {
            if (File.Exists(highScoreFilePath))
            {
                string scoreStr = File.ReadAllText(highScoreFilePath);
                highScore = int.Parse(scoreStr);
                if (highScoreText != null)
                    highScoreText.text = highScore.ToString();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading high score: {e.Message}");
        }
    }

    private void SaveHighScore()
    {
        try
        {
            File.WriteAllText(highScoreFilePath, highScore.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving high score: {e.Message}");
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
        if (isGameActive)
        {
            currentTime -= Time.deltaTime;
            
            if (currentTime <= 0)
            {
                EndGame();
            }
            else
            {
                UpdateTimer();
            }
        }
    }

    private void ResetGame()
    {
        score = 0;
        UpdateScore(score);
        currentTime = timeRemain;
        UpdateTimer();
        isGameActive = false;
    }

    private void UpdateTimer()
    {
        if (timeText != null)
        {
            timeText.text = Mathf.CeilToInt(currentTime).ToString();
        }
    }

    private void EndGame()
    {
        isGameActive = false;
        currentTime = 0;
        UpdateTimer();
        
        // Check and update high score when game ends
        if (score > highScore)
        {
            highScore = score;
            if (highScoreText != null)
                highScoreText.text = highScore.ToString();
            SaveHighScore();
        }
    }

    // Call this method when player picks up first ball
    public void StartGame()
    {
        if (!isGameActive)
        {
            ResetGame();
            isGameActive = true;
        }
    }

    public void OnBallEnterTopTrigger(GameObject ball)
    {
        int ballId = ball.GetInstanceID();
        ballPassedTop[ballId] = true;
        ballTopTriggerTime[ballId] = Time.time; // Record time when ball passes top trigger
    }

    public void OnBallTouchRing(GameObject ball)
    {
        int ballId = ball.GetInstanceID();
        ballTouchedRing[ballId] = true;
    }

    public void OnBallEnterBottomTrigger(GameObject ball)
    {
        if (!isGameActive) return; // Don't score if game isn't active
        
        int ballId = ball.GetInstanceID();
        
        if (ballPassedTop.ContainsKey(ballId) && ballPassedTop[ballId])
        {
            bool isPerfect = !ballTouchedRing.ContainsKey(ballId) || !ballTouchedRing[ballId];
            bool isQuickDunk = false;

            // Check for quick dunk condition
            if (!isPerfect && ballTopTriggerTime.ContainsKey(ballId))
            {
                float timeThroughHoop = Time.time - ballTopTriggerTime[ballId];
                isQuickDunk = timeThroughHoop <= quickDunkThreshold;
            }

            // Award points based on dunk type
            int points = (isPerfect || isQuickDunk) ? 2 : 1;
            score += points;
            UpdateScore(score);

            // Show perfect dunk text for either type of dunk
            if (isPerfect || isQuickDunk)
            {
                isPerfectDunk = true;
                if (perfectDunkObject != null)
                    perfectDunkObject.SetActive(true);
                StartCoroutine(HidePerfectText());
            }

            // Clean up tracking for this ball
            ballPassedTop.Remove(ballId);
            ballTouchedRing.Remove(ballId);
            ballTopTriggerTime.Remove(ballId);
        }
    }

    private IEnumerator HidePerfectText()
    {
        yield return new WaitForSeconds(1.5f);
        if (perfectDunkObject != null)
            perfectDunkObject.SetActive(false);
        isPerfectDunk = false;
    }
}