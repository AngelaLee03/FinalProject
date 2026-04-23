using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PathGameManager : MonoBehaviour
{
    public CinemachineCamera drawCam;
    public CinemachineCamera followCam;
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
    }
    void SwitchToDrawCam()
    {
        drawCam.Priority = 10;
        followCam.Priority = 0;
    }
    private void Start()
    {
        input.OnPathUpdated += HandlePathUpdated;
        input.OnPathFinished += HandlePathFinished;

        SwitchToDrawCam();
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
}
