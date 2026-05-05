using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PathGameManager : MonoBehaviour
{
    public CinemachineCamera beginCam;
    public CinemachineCamera followCam;
    public CinemachineCamera endCam;
    public CinemachineCamera drawCam;
    public Transform startPoint;
    public PathInputController input;
    public PathRenderer pathRenderer;
    public PathValidator validator;
    public PlayerPathFollower player;

    // Life system
    public int maxLives = 3;
    public int currentLives;
    public Image[] heartIcons;
    private bool isGameOver;

    private List<Vector3> currentPath;
    public System.Action OnPathFinished;

    void SwitchToFollowCam()
    {
        followCam.Priority = 10;
        beginCam.Priority = 0;
        endCam.Priority = 0;
        drawCam.Priority = 0;
    }
    void SwitchToBeginningCam()
    {
        beginCam.Priority = 10;
        followCam.Priority = 0;
        endCam.Priority = 0;
        drawCam.Priority = 0;
    }
    void SwitchToDrawCam()
    {
        drawCam.Priority = 10;
        followCam.Priority = 0;
        endCam.Priority = 0;
        beginCam.Priority = 0;
       
    }
    void SwitchToEndCam()
    {
        drawCam.Priority = 0;
        followCam.Priority = 0;
        endCam.Priority = 10;
        beginCam.Priority = 0;
    }
    private void Start()
    {
        input.OnPathUpdated += HandlePathUpdated;
        input.OnPathFinished += HandlePathFinished;
        player.OnPathComplete += HandlePathComplete;

        SwitchToBeginningCam();
        SwitchToDrawCam();

        // Life system
        currentLives = maxLives;
        UpdateHeartsUI();
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
            input.ResetPath();
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
        if (player == null) return;

        if (startPoint != null)
        {
            Collider startCollider = startPoint.GetComponent<Collider>();

            Vector3 resetPos = startPoint.position;

            if (startCollider != null)
            {
                resetPos.y = startCollider.bounds.max.y + 0.5f;
            }
            else
            {
                resetPos.y += 1f;
            }

            player.ResetToStart(resetPos);
        }
        else
        {
            player.ResetToStart();
        }

        input?.ResetPath();
        pathRenderer?.Clear();

        SwitchToDrawCam();
    }

    // Life system
        public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        UpdateHeartsUI();

        Debug.Log("Player lost a life. Lives left: " + currentLives);

        if (currentLives <= 0)
        {
            GameOver();
            return;
        }

        ResetPlayerToStart();
    }

        private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].enabled = i < currentLives;
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");

        // Simple restart for now
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
