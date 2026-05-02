using UnityEngine;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour
{
    public Material pathMaterial;
    public float textureLength = 1.2f;

    private LineRenderer line;

    private void Awake()
    {
        // Getting component to render our path
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;

        line.textureMode = LineTextureMode.Tile;
        line.startColor = Color.white;
        line.endColor = Color.white;
        line.widthMultiplier = 0.65f;

        if (pathMaterial != null)
        {
            line.material = pathMaterial;
        }
    }

    // Function for drawing our path
    public void DrawPath(List<Vector3> points)
    {
        Vector3[] adjusted = new Vector3[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            adjusted[i] = points[i] + Vector3.up * 0.05f;
        }

        line.positionCount = adjusted.Length;
        line.SetPositions(adjusted);

    }

    // Clears path from the screen
    public void Clear()
    {
        line.positionCount = 0;
    }
}