using UnityEngine;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour
{
    private LineRenderer line;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    public void DrawPath(List<Vector3> points)
    {
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
    public void Clear()
    {
        line.positionCount = 0;
    }
}
