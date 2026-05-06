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

            // Checking if the ray intersects the drawing plane
            if (!drawPlane.Raycast(ray, out float enter))
            {
                // If does not intersect, fallback to the last valid point to avoid gaps in path
                worldPosition = lastPoint;
            }
            else
            {   
                // Converting hit distance to actual world position
                worldPosition = ray.GetPoint(enter);
                worldPosition.y += 0.02f;
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
                if (Vector3.Distance(worldPosition, lastPoint) > 0.05f)
                {
                    pathPoints.Add(worldPosition);
                    lastPoint = worldPosition;

                    OnPathUpdated?.Invoke(pathPoints);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Gets final point in the path
                Vector3 lastPoint = pathPoints[pathPoints.Count - 1];

                // If the user ends the path close enough to the end point, snap ending position precisely onto end point
                if (Vector3.Distance(lastPoint, endPoint.position) < snapThreshold)
                {
                    pathPoints[pathPoints.Count - 1] = endPoint.position;
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

        return new Vector3(x, point.y, z);
    }
}
