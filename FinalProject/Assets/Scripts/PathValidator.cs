using UnityEngine;
using System.Collections.Generic;

public class PathValidator : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float threshold = 0.5f;
    public LayerMask obstacleMask;
    public Collider stageBounds;

    public bool Validate(List<Vector3> path)
    {
        // Checking if path is too short
        if (path == null || path.Count < 2)
        {
            Debug.Log("Path is too short!");
            return false;
        }

        // Checking if path is starting from valid start position
        if (Vector3.Distance(path[0], startPoint.position) > threshold) 
        {
            Debug.Log(Vector3.Distance(path[0], startPoint.position));
            Debug.Log("Path needs to start from the start point!");
            return false;
        }
        
        // Checking if path is ending at valid end position
        if (Vector3.Distance(path[^1], endPoint.position) > threshold)
        {
            Debug.Log("Path needs to end at the end point");
            return false;
        }

        for (int i = 0; i < path.Count - 1; i++) {
            // Looping over all points to see if path is within bounds
            if (!stageBounds.bounds.Contains(path[i]))
            {
                Debug.Log("Out of bounds");
                return false;
            }

            // Looping over all points to see if path has overlapped with any obstacles
            if (Physics.Linecast(path[i], path[i + 1], obstacleMask)) 
            {
                Debug.Log("Path overlaps obstacles");
                return false;
            }
            
        }
        return true;
    }
}
