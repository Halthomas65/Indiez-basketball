using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCollider : MonoBehaviour
{
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            scoreManager.OnBallTouchRing(collision.gameObject);
        }
    }
}
