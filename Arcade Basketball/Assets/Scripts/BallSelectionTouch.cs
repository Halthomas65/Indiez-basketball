using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallSelectionTouch : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private Transform ballsCircle;
    [SerializeField] private float selectionThreshold = 1f;
    [SerializeField] private Camera ballSelectionCamera; // Reference to cameraBallSelect
    // [SerializeField] private Button okButton; // Reference to OK button
    [SerializeField] private Transform ballsParent; // Reference to BallsParent containing the 5 balls

    private Vector2 touchStart;
    private bool isDragging = false;
    private Ball selectedBall;
    private float touchStartTime;
    private const float CLICK_TIME_THRESHOLD = 0.2f;
    private GameObject selectionCircle; // Visual indicator for selection

    void Start()
    {
        if (ballSelectionCamera == null)
        {
            ballSelectionCamera = GameObject.Find("cameraBallSelect").GetComponent<Camera>();
        }
        
        // Ensure camera is properly configured
        if (ballSelectionCamera != null)
        {
            ballSelectionCamera.clearFlags = CameraClearFlags.SolidColor;
            ballSelectionCamera.rect = new Rect(0, 0, 1, 1);
            ballSelectionCamera.targetDisplay = 0; // Main display
            ballSelectionCamera.enabled = true;
            
            // If using a render texture, ensure it's created and assigned
            if (ballSelectionCamera.targetTexture != null)
            {
                RenderTexture rt = new RenderTexture(1024, 1024, 24);
                rt.antiAliasing = 1; // Disable MSAA for the render texture
                ballSelectionCamera.targetTexture = rt;
            }
            else
            {
                // If not using render texture, clear it
                ballSelectionCamera.targetTexture = null;
            }
        }

        // // Setup OK button listener
        // if (okButton != null)
        // {
        //     okButton.onClick.AddListener(ApplySelectedBallMesh);
        // }

        // Create selection circle (you can also use a prefab)
        CreateSelectionCircle();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
            touchStartTime = Time.time;
            isDragging = false; // Don't start dragging immediately
        }
        else if (Input.GetMouseButton(0))
        {
            // Only start dragging if we've held the button and moved
            if (!isDragging && Vector2.Distance(touchStart, Input.mousePosition) > 10f)
            {
                isDragging = true;
            }

            // Only rotate if we're actually dragging
            if (isDragging)
            {
                float deltaX = Input.mousePosition.x - touchStart.x;
                ballsCircle.Rotate(Vector3.up, -deltaX * rotationSpeed * Time.deltaTime);
                touchStart = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Consider it a click if we didn't drag and released within time threshold
            if (!isDragging && (Time.time - touchStartTime) < CLICK_TIME_THRESHOLD)
            {
                TrySelectBall();
            }
            isDragging = false;
        }
    }

    private void CreateSelectionCircle()
    {
        selectionCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        selectionCircle.transform.localScale = new Vector3(1.2f, 0.05f, 1.2f);
        
        // Make it yellow and semi-transparent
        Material circleMaterial = new Material(Shader.Find("Standard"));
        circleMaterial.color = new Color(1f, 1f, 0f, 0.5f);
        selectionCircle.GetComponent<Renderer>().material = circleMaterial;
        
        selectionCircle.SetActive(false);
    }

    private void TrySelectBall()
    {
        // Use ballSelectionCamera instead of Camera.main
        Ray ray = ballSelectionCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Ball ball = hit.collider.GetComponent<Ball>();
            if (ball != null)
            {
                SelectBall(ball);
            }
        }
    }

    private void SelectBall(Ball ball)
    {
        if (selectedBall != null)
        {
            // Reset previous ball's color
            selectedBall.GetComponent<Renderer>().material.color = Color.white;
        }

        selectedBall = ball;
        
        // Position selection circle around the ball
        selectionCircle.SetActive(true);
        selectionCircle.transform.position = ball.transform.position;
        selectionCircle.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Lay flat
    }

    public void ApplySelectedBallMesh()
    {
        if (selectedBall == null || ballsParent == null) return;

        // Find the mesh object by name instead of index
        Transform selectedMesh = selectedBall.transform.Find("mesh");
        if (selectedMesh == null) 
        {
            Debug.LogError("Selected ball's mesh not found!");
            return;
        }

        foreach (Transform child in ballsParent)
        {
            Ball ballScript = child.GetComponent<Ball>();
            if (ballScript != null)
            {
                // Find mesh by name instead of index
                Transform ballMesh = child.Find("mesh");
                if (ballMesh != null)
                {
                    MeshFilter targetMeshFilter = ballMesh.GetComponent<MeshFilter>();
                    MeshRenderer targetMeshRenderer = ballMesh.GetComponent<MeshRenderer>();
                    
                    MeshFilter sourceMeshFilter = selectedMesh.GetComponent<MeshFilter>();
                    MeshRenderer sourceMeshRenderer = selectedMesh.GetComponent<MeshRenderer>();

                    if (targetMeshFilter != null && sourceMeshFilter != null)
                    {
                        Debug.Log($"Copying mesh from {selectedBall.name} to {child.name}");
                        targetMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
                    }
                    
                    if (targetMeshRenderer != null && sourceMeshRenderer != null)
                    {
                        targetMeshRenderer.sharedMaterials = sourceMeshRenderer.sharedMaterials;
                    }
                }
                else
                {
                    Debug.LogError($"Mesh not found in ball: {child.name}");
                }
            }
        }

        GetComponent<EventManager>().ManageCam();
    }

    void OnDisable()
    {
        // Hide selection circle when switching views
        if (selectionCircle != null)
        {
            selectionCircle.SetActive(false);
        }
    }
}
