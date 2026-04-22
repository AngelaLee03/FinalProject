using UnityEngine;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour
{
    private LineRenderer line;
    private void Awake()
    {
        // Getting component to render our path
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    // Function for drawing our path
    public void DrawPath(List<Vector3> points)
    {
        // Update LineRenderer to match the number of points in the path
        line.positionCount = points.Count;

        // Redraw the path in one call
        line.SetPositions(points.ToArray());
    }
    // Clears path from the screen
    public void Clear()
    {
        line.positionCount = 0;
    }
}
