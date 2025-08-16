using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private float throwForce = 80f;
    [SerializeField] private float throwAngle = 0.5f;

    public List<Ball> balls = new List<Ball>();
    private Ball currentBall;

    public Transform ballsParent;

    private Vector3 lastFingerPosition;

    void Start()
    {
        // Find all balls in the scene under BallsParent and add to list
        ballsParent = GameObject.Find("BallsParent").transform;
        foreach (Transform child in ballsParent)
        {
            Ball ballScript = child.GetComponent<Ball>();
            if (ballScript != null)
                balls.Add(ballScript);
        }
    }

    // Get the next available ball (not thrown)
    public Ball GetBall()
    {
        foreach (Ball ball in balls)
        {
            if (!ball.isThrown)
            {
                currentBall = ball;
                return ball;
            }
        }
        return null;
    }

    void Update()
    {
        // Mouse/finger press
        if (Input.GetMouseButtonDown(0))
        {
            Ball ball = GetBall();
            if (ball != null)
            {
                ball.PrepareForThrow(Input.mousePosition);
                currentBall = ball;
                lastFingerPosition = Input.mousePosition;
            }
        }

        // While finger is holding
        if (Input.GetMouseButton(0) && currentBall != null)
        {
            currentBall.UpdateStickPosition(Input.mousePosition);
            lastFingerPosition = Input.mousePosition;
        }

        // Mouse/finger release
        if (Input.GetMouseButtonUp(0) && currentBall != null)
        {
            currentBall.Launch(lastFingerPosition, throwForce, throwAngle);
            currentBall = null;
        }
    }
}
