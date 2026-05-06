using UnityEngine;
using Unity.Cinemachine;

// Creating a shared level part class to hold each level part's starting point, ending point, and camera settings
[System.Serializable]
public class LevelPart
{
    public Transform startPoint;
    public Transform endPoint;
    public Collider levelBounds;

    [Header("Camera")]
    public Transform camTarget; // target for drawing cam to look at (will be in center of level part)
    public Unity.Cinemachine.CinemachineCamera levelCam;
}
