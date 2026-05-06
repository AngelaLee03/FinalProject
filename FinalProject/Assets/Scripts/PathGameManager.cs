using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PathGameManager : MonoBehaviour
{
    public CinemachineCamera drawCam;
    public CinemachineCamera followCam;
    public CinemachineCamera endCam;
    public Transform startPoint;
    public PathInputController input;
    public PathRenderer pathRenderer;
    public PathValidator validator;
    public PlayerPathFollower player;

    private List<Vector3> currentPath;
    public System.Action OnPathFinished;

    void SwitchToFollowCam()
    {
        followCam.Priority = 10;
        drawCam.Priority = 0;
        endCam.Priority = 0;
    }
    void SwitchToDrawCam()
    {
        drawCam.Priority = 10;
        followCam.Priority = 0;
        endCam.Priority = 0;
    }
    void SwitchToEndCam()
    {
        drawCam.Priority = 0;
        followCam.Priority = 0;
        endCam.Priority = 10;
    }
    private void Start()
    {
        input.OnPathUpdated += HandlePathUpdated;
        input.OnPathFinished += HandlePathFinished;
        player.OnPathComplete += HandlePathComplete;

        SwitchToDrawCam();
    }

    private void OnDestroy()
    {
        if (input != null)
        {
            input.OnPathUpdated -= HandlePathUpdated;
            input.OnPathFinished -= HandlePathFinished;
        }
    }

    // Rendering the path
    private void HandlePathUpdated(List<Vector3> path)
    {
        currentPath = path;
        pathRenderer.DrawPath(path);
    }

    // Checks if path is valid
    private void HandlePathFinished(List<Vector3> path)
    {
        bool valid = validator.Validate(path);

        if (!valid)
        {
            pathRenderer.Clear();
            SwitchToDrawCam();
            Debug.Log("Invalid Path");
            return;
        }
        player.FollowPath(path);
        SwitchToFollowCam();
        OnPathFinished?.Invoke();
    }

    // Clearing the path once we reach the end point
    private void HandlePathComplete()
    {
        SwitchToEndCam();
        pathRenderer.Clear();
    }

    public void ResetPlayerToStart()
    {
        if (player == null)
        {
            return;
        }

        if (startPoint != null)
        {
            player.ResetToStart(startPoint.position);
        }
        else
        {
            player.ResetToStart();
        }

        if (input != null)
        {
            input.ResetPath();
        }

        if (pathRenderer != null)
        {
            pathRenderer.Clear();
        }

        SwitchToDrawCam();
        Debug.Log("Player was reset to the start point");
    }
}
