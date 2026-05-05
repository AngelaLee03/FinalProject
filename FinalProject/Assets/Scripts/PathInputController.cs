using UnityEngine;
using System.Collections.Generic;

public class PathInputController: MonoBehaviour
{
    private Plane drawPlane;
    private Vector3 lastPoint;
    private Vector3 worldPosition;
    public Transform startPoint;
    public Transform endPoint;
    public Collider stageBounds;
    public float snapThreshold = 1.5f;
    public LayerMask groundMask;
    public float surfaceOffset = 0.05f;

    private List<Vector3> pathPoints = new List<Vector3> ();

    public System.Action<List<Vector3>> OnPathUpdated;
    public System.Action<List<Vector3>> OnPathFinished;

    private void Awake()
    {
        // Plane for drawing to be visible
        drawPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void ResetPath()
    {
        pathPoints.Clear();
        lastPoint = Vector3.zero;
        worldPosition = Vector3.zero;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Converting the screen touch position into a ray from the main camera
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            RaycastHit hit;

            // Checks if player is drawing on valid ground
            if (Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                worldPosition = hit.point;

                // Updating path position
                worldPosition += hit.normal * surfaceOffset;
            }
            else
            {
                // Use last valid point as a fall back option
                worldPosition = lastPoint;
            }

            if (touch.phase == TouchPhase.Began)
            {
                // Starting a new path
                pathPoints.Clear();
                
                // Clamping the touch position so we stay within stage bounds
                Vector3 clamped = ClampToBounds(worldPosition);
                worldPosition = clamped;

                // Snaps starting position precisely onto the start point if player starts close enough
                if (Vector3.Distance(worldPosition, startPoint.position) < snapThreshold)
                {
                    worldPosition = startPoint.position;
                }
                pathPoints.Add(worldPosition);

                // Storing current position as the last valid point
                lastPoint = worldPosition;

                OnPathUpdated?.Invoke(pathPoints);
            }

            else if (touch.phase == TouchPhase.Moved)
            {
                // Only adds a new point if we've moved far enough from previous point 
                if (Vector3.Distance(worldPosition, lastPoint) > 0.025f)
                {
                    pathPoints.Add(worldPosition);
                    lastPoint = worldPosition;

                    OnPathUpdated?.Invoke(pathPoints);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Collider endCol = endPoint.GetComponent<Collider>();
                Vector3 topPoint = endPoint.position;
                topPoint.y = endCol.bounds.max.y + surfaceOffset;

                // Gets final point in the path
                Vector3 finalPoint = pathPoints[pathPoints.Count - 1];

                // If the user ends the path close enough to the end point, snap ending position precisely onto end point
                if (Vector3.Distance(finalPoint, endPoint.position) < snapThreshold)
                {
                    // Player ends on top of the box
                    pathPoints[pathPoints.Count - 1] = topPoint;
                }
                OnPathFinished?.Invoke(pathPoints);
            }
        }
    }
    // Function for clamping our path to stay within stage bounds
    Vector3 ClampToBounds(Vector3 point)
    {
        Bounds b = stageBounds.bounds;
        float x = Mathf.Clamp(point.x, b.min.x, b.max.x);
        float z = Mathf.Clamp(point.z, b.min.z, b.max.z);

        Ray downRay = new Ray(new Vector3(x, point.y + 5f, z), Vector3.down);

        if (Physics.Raycast(downRay, out RaycastHit hit, 10f, groundMask))
        {
            return hit.point + hit.normal * surfaceOffset;
        }

        return point;
    }
}
