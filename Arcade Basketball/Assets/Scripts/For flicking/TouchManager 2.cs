using System.Collections.Generic;
using UnityEngine;

public class TouchManager2 : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private float minThrowForce = 40f;
    [SerializeField] private float maxThrowForce = 120f;
    [SerializeField] private float minThrowAngle = 30f;
    [SerializeField] private float maxThrowAngle = 70f;
    [SerializeField] private float maxFlickSpeed = 3000f; // pixels per second
    [SerializeField] private float maxFlickLength = 300f; // pixels

    public List<Ball> balls = new List<Ball>();
    private Ball currentBall;
    public Transform ballsParent;

    private Vector3 fingerDownPosition;
    private Vector3 lastFingerPosition;
    private float fingerDownTime;

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
                fingerDownPosition = Input.mousePosition;
                lastFingerPosition = fingerDownPosition;
                fingerDownTime = Time.time;
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
            // Calculate flick parameters
            Vector3 flickVector = lastFingerPosition - fingerDownPosition;
            float flickLength = flickVector.magnitude;
            float flickDuration = Time.time - fingerDownTime;
            float flickSpeed = flickLength / flickDuration;

            // Calculate force based on flick speed
            float normalizedSpeed = Mathf.Clamp01(flickSpeed / maxFlickSpeed);
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, normalizedSpeed);

            // Calculate angle based on flick length
            float normalizedLength = Mathf.Clamp01(flickLength / maxFlickLength);
            float angle = Mathf.Lerp(minThrowAngle, maxThrowAngle, normalizedLength);

            // Launch the ball
            currentBall.Launch(lastFingerPosition, force, angle * Mathf.Deg2Rad);
            currentBall = null;
        }
    }
}
