using UnityEngine;
using System.Collections.Generic;

public class PathGameManager : MonoBehaviour
{
    public PathInputController input;
    public PathRenderer pathRenderer;
    public PathValidator validator;
    public PlayerPathFollower player;

    private List<Vector3> currentPath;
    public System.Action OnPathFinished;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        input.OnPathUpdated += HandlePathUpdated;
        input.OnPathFinished += HandlePathFinished;
    }

    private void HandlePathUpdated(List<Vector3> path)
    {
        currentPath = path;
        pathRenderer.DrawPath(path);
    }

    private void HandlePathFinished(List<Vector3> path)
    {
        bool valid = validator.Validate(path);

        if (!valid)
        {
            pathRenderer.Clear();
            Debug.Log("Invalid Path");
            return;
        }
        player.FollowPath(path);
        OnPathFinished?.Invoke();
    }
}
