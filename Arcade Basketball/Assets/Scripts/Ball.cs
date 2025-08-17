using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isThrown = false;
    public bool isSticking = false; // Add this field
    private Rigidbody rb;
    private Vector3 stickTargetPos;
    private Collider wallCollider; // Cache the wall collider

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Cache the wall collider
        wallCollider = GameObject.Find("WallCollider").GetComponent<Collider>();
    }

    // Called when player presses
    public void PrepareForThrow(Vector3 screenPos)
    {
        isThrown = false;
        isSticking = true;
        rb.isKinematic = true; // Make kinematic while sticking

        UpdateStickPosition(screenPos);
    }

    public void UpdateStickPosition(Vector3 screenPos)
    {
        if (!isSticking) return;

        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider == wallCollider)
            {
                // Calculate the offset based on ball's radius (half of scale)
                float ballRadius = transform.localScale.z * 0.5f;
                
                // Always place the ball on the positive Z side of the collider
                Vector3 targetPos;
                if (hit.normal.z < 0) // If hitting from front (positive Z)
                {
                    targetPos = hit.point - hit.normal * ballRadius;
                }
                else // If hitting from back (negative Z), force to front
                {
                    targetPos = hit.point + Vector3.forward * ballRadius;
                }

                // Ensure minimum Z position is always slightly in front of the wall
                float wallZ = wallCollider.bounds.center.z;
                if (targetPos.z <= wallZ)
                {
                    targetPos.z = wallZ + ballRadius;
                }

                stickTargetPos = targetPos;
                
                // Smooth follow
                transform.position = Vector3.Lerp(transform.position, stickTargetPos, Time.deltaTime * 50f);
            }
        }    }

    // Called when player releases
    public void Launch(Vector3 releasePoint, float force, float upwardAngle = 0.5f)
    {
        isThrown = true;
        isSticking = false;
        rb.isKinematic = false;

        // Convert screen point to world point for direction calculation
        Camera cam = Camera.main;
        Vector3 worldReleasePoint = cam.ScreenToWorldPoint(new Vector3(releasePoint.x, releasePoint.y, transform.position.z));
        
        // Get only the horizontal direction (ignore Y component)
        Vector3 horizontalDir = (transform.position - worldReleasePoint);
        horizontalDir.y = 0;
        horizontalDir = horizontalDir.normalized;
        
        // Create rotation that tilts the horizontal direction upward by desired angle
        float angleInRadians = 50f * Mathf.Deg2Rad; // 50 degree fixed angle
        Vector3 launchDir = new Vector3(
            horizontalDir.x,
            Mathf.Sin(angleInRadians),
            horizontalDir.z * Mathf.Cos(angleInRadians)
        ).normalized;
        
        // Ensure minimum forward motion
        launchDir.z = Mathf.Max(launchDir.z, 0.3f);
        launchDir = launchDir.normalized;

        // Apply force impulse instead of continuous force
        rb.AddForce(launchDir * force, ForceMode.Impulse);
        
        // Add some torque for rotation
        rb.AddTorque(Random.insideUnitSphere * force * 0.1f);
    }

    // Add this to prevent backward movement through the wall
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tray"))
        {
            isThrown = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else if (collision.gameObject.name == "WallCollider")
        {
            // If trying to go backward through the wall, stop the ball
            if (rb.velocity.z < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
        }
    }
}
