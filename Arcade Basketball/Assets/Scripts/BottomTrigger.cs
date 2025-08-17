using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomTrigger : MonoBehaviour
{
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            scoreManager.OnBallEnterBottomTrigger(other.gameObject);
        }
    }
}
