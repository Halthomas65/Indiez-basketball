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
        }
    }

    // Call this in Update while sticking
    public void StickToWall()
    {
        if (!isSticking) return;
        transform.position = Vector3.Lerp(transform.position, stickTargetPos, Time.deltaTime * 50f);
    }

    // Called when player releases
    public void Launch(Vector3 releasePoint) // Replace with your force logic
    {
        isThrown = true;
        isSticking = false;
        rb.isKinematic = false;

        // Calculate launch direction based on movement
        Vector3 throwDirection = (releasePoint - transform.position).normalized;
        float upwardForce = 0.5f; // Adjust this value to control upward angle
        
        // Ensure the throw direction has a minimum positive Z component
        float minZComponent = 0.3f; // Adjust this value to control minimum forward motion
        throwDirection.z = Mathf.Max(throwDirection.z, minZComponent);
        
        Vector3 launchDir = (throwDirection + Vector3.up * upwardForce).normalized;
        rb.AddForce(launchDir * 1000f); // Adjust force multiplier as needed
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
}
