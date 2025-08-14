using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public List<Ball> balls = new List<Ball>();
    private Ball currentBall;

    void Start()
    {
        // Find all balls in the scene under BallsParent and add to list
        Transform ballsParent = GameObject.Find("BallsParent").transform;
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
            }
        }

        // Mouse/finger release
        if (Input.GetMouseButtonUp(0) && currentBall != null)
        {
            currentBall.Launch();
            currentBall = null;
        }
    }
}
