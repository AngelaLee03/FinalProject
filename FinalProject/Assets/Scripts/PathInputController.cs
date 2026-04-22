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
        drawPlane = new Plane(Vector3.up, Vector3.zero);
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (!drawPlane.Raycast(ray, out float enter))
            {
                worldPosition = lastPoint;
            }
            else
            {
                worldPosition = ray.GetPoint(enter);
                worldPosition.y += 0.02f;
            }
            
            if (touch.phase == TouchPhase.Began)
            {
                pathPoints.Clear();
                Vector3 clamped = ClampToBounds(worldPosition);
                if (Vector3.Distance(worldPosition, startPoint.position) < snapThreshold)
                {
                    worldPosition = startPoint.position;
                }
                pathPoints.Add(worldPosition);
                lastPoint = worldPosition;

                OnPathUpdated?.Invoke(pathPoints);
            }

            else if (touch.phase == TouchPhase.Moved)
            {
                if (Vector3.Distance(worldPosition, lastPoint) > 0.05f)
                {
                    pathPoints.Add(worldPosition);
                    lastPoint = worldPosition;

                    OnPathUpdated?.Invoke(pathPoints);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector3 lastPoint = pathPoints[pathPoints.Count - 1];
                if (Vector3.Distance(lastPoint, endPoint.position) < snapThreshold)
                {
                    pathPoints[pathPoints.Count - 1] = endPoint.position;
                }
                OnPathFinished?.Invoke(pathPoints);
            }
        }
    }
    Vector3 ClampToBounds(Vector3 point)
    {
        Bounds b = stageBounds.bounds;
        float x = Mathf.Clamp(point.x, b.min.x, b.max.x);
        float z = Mathf.Clamp(point.z, b.min.z, b.max.z);

        return new Vector3(x, point.y, z);
    }
}
