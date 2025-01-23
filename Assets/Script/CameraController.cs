using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cruiser; // Reference to the Cruiser object.
    public float orbitSpeed = 150f; // Speed of orbiting around the Cruiser.
    public float zoomSpeed = 20f; // Speed of zooming in/out.
    public float minZoomDistance = 10f; // Minimum zoom distance from the Cruiser.
    public float maxZoomDistance = 50f; // Maximum zoom distance from the Cruiser.
    public float smoothTime = 0.2f; // Smoothing duration for camera transitions.

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayers; // Layers considered obstacles to avoid clipping.
    public float obstacleOffset = 0.5f; // Offset to maintain distance from obstacles.

    [Header("Cruiser Mesh Settings")]
    public MeshFilter cruiserMeshFilter; // Reference to the Cruiser's MeshFilter component.
    public float meshAvoidanceOffset = 2f; // Offset to avoid getting too close to the mesh boundaries.

    private Vector3 currentVelocity; // Used for smoothing.
    private float currentZoomDistance; // Current zoom distance.
    private bool isCameraActive = true; // Determines if the camera is active.

    private CruiserController cruiserController; // Reference to CruiserController.
    private Bounds cruiserBounds; // Cached bounds of the Cruiser's mesh.

    void Start()
    {
        // Initialize zoom distance and cache the CruiserController component.
        currentZoomDistance = (maxZoomDistance + minZoomDistance) / 2;
        if (cruiser != null)
        {
            cruiserController = cruiser.GetComponent<CruiserController>();

            // Cache the mesh bounds if a MeshFilter is provided.
            if (cruiserMeshFilter != null)
            {
                cruiserBounds = cruiserMeshFilter.mesh.bounds;
            }
            else
            {
                Debug.LogError("Cruiser MeshFilter is missing. Please assign it in the Inspector.");
            }
        }
        else
        {
            Debug.LogError("Cruiser reference is missing. Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        if (cruiserController == null) return;

        // Check weapon states from CruiserController to determine camera activity.
        isCameraActive = cruiserController.activeWeaponIndex == -1;

        if (isCameraActive)
        {
            HandleCameraOrbit();
            HandleCameraZoom();
        }
    }

    void HandleCameraOrbit()
    {
        if (Input.GetMouseButton(1)) // Right mouse button for orbiting.
        {
            float horizontal = Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * orbitSpeed * Time.deltaTime;

            // Rotate the camera around the Cruiser.
            transform.RotateAround(cruiser.position, Vector3.up, horizontal);
            transform.RotateAround(cruiser.position, transform.right, vertical);
        }

        // Maintain smooth position and prevent the camera from entering the Cruiser's mesh.
        AdjustCameraDistance();
    }

    void HandleCameraZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            // Adjust zoom distance based on input.
            currentZoomDistance -= scrollInput * zoomSpeed;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);
        }

        // Update the camera position with smooth transitions.
        AdjustCameraDistance();
    }

    void AdjustCameraDistance()
    {
        if (cruiser == null) return;

        // Desired camera position based on zoom distance.
        Vector3 direction = (transform.position - cruiser.position).normalized;
        Vector3 targetPosition = cruiser.position + direction * currentZoomDistance;

        // Use raycasting to check for obstacles.
        if (Physics.Raycast(cruiser.position, direction, out RaycastHit hit, currentZoomDistance, obstacleLayers))
        {
            targetPosition = hit.point - direction * obstacleOffset;
        }

        // Ensure the camera stays outside the bounds of the Cruiser's mesh.
        if (cruiserMeshFilter != null)
        {
            Vector3 localPoint = cruiser.InverseTransformPoint(targetPosition); // Convert to local space.
            if (cruiserBounds.Contains(localPoint))
            {
                // Push the camera outside the bounds.
                Vector3 closestPoint = cruiserBounds.ClosestPoint(localPoint);
                targetPosition = cruiser.TransformPoint(closestPoint + direction * meshAvoidanceOffset);
            }
        }

        // Smoothly move the camera to the target position.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // Ensure the camera looks at the Cruiser at all times.
        transform.LookAt(cruiser);
    }
}
