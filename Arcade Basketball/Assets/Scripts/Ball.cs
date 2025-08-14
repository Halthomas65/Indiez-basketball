using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isThrown = false;
    public bool isSticking = false;
    private Rigidbody rb;
    private Vector3 stickTargetPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called when player presses
    public void PrepareForThrow(Vector3 screenPos)
    {
        isThrown = false;
        isSticking = true;

        // Get collider world position at finger Y
        // Assume the collider is named "WallCollider"
        Collider wall = GameObject.Find("WallCollider").GetComponent<Collider>();
        Camera cam = Camera.main;
        float zOnWall = wall.bounds.center.z - wall.bounds.extents.z; // front face of collider

        Vector3 fingerWorld = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cam.transform.position.z - zOnWall));
        // Clamp fingerWorld.y within collider bounds
        float minY = wall.bounds.min.y;
        float maxY = wall.bounds.max.y;
        float clampedY = Mathf.Clamp(fingerWorld.y, minY, maxY);

        stickTargetPos = new Vector3(wall.bounds.center.x, clampedY, zOnWall);

        StickToWall();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tray"))
        {
            isThrown = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private IEnumerator MoveToFinger(Vector3 targetPos)
    {
        float speed = 100f;
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
    }

    // Call this in Update while sticking
    public void StickToWall()
    {
        if (!isSticking) return;

        // Calculate direction to target
        Vector3 dir = (stickTargetPos - transform.position);
        float forceMag = 10f * Physics.gravity.magnitude * rb.mass;
        rb.AddForce(dir.normalized * forceMag, ForceMode.Acceleration);
    }

    // Called when player releases
    public void Launch(float force = 50f) // Replace with your force logic
    {
        isThrown = true;
        isSticking = false;
        rb.isKinematic = false;

        // Launch toward positive z direction with a 45 degree angle
        Vector3 launchDir = Quaternion.Euler(-45, 0, 0) * Vector3.forward;
        rb.AddForce(transform.TransformDirection(launchDir) * 100f, ForceMode.Impulse); // force set to 100
    }
}
