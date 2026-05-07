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
    public PathInputController input;
    public PathRenderer pathRenderer;
    public PathValidator validator;
    public PlayerPathFollower player;

    // Checkpoint system
    public List<LevelPart> levelParts;
    private int currentPartIndex = 0;

    // Life system
    public int maxLives = 5;
    public int currentLives;
    public Image[] heartIcons;
    private bool isGameOver;

    public float lifeLossCooldown = 1f;
    private float lastLifeLossTime = -999f;

    // Path system
    private List<Vector3> currentPath;
    public System.Action OnPathFinished;

    void SwitchToBeginningCam()
    {
        beginCam.Priority = 5;
        followCam.Priority = 0;
        endCam.Priority = 0;
    }
    void SwitchToFollowCam()
    { 
        beginCam.Priority = 0;
        followCam.Priority = 10;
        endCam.Priority = 0;
    }
    void SetActiveLevelPart(int index)
    {
        beginCam.Priority = 0;
        followCam.Priority = 0;
        endCam.Priority = 0;
        for (int i = 0; i < levelParts.Count; i++)
        {
            // Activating the drawing camera for the currently active level part based on index passed
            levelParts[i].levelCam.Priority = (i == index) ? 10 : 0;
        }
    }
    void SwitchToEndCam()
    {
        beginCam.Priority = 0;
        followCam.Follow = null;
        followCam.enabled = false;
        endCam.Priority = 10;
    }

    private void Start()
    {
        currentPartIndex = 0;
        SetActiveLevelPart(currentPartIndex); // setting the active camera to first part of level

        input.OnPathUpdated += HandlePathUpdated;
        input.OnPathFinished += HandlePathFinished;
        player.OnPathComplete += HandlePathComplete;

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
        LevelPart part = levelParts[currentPartIndex];
        bool valid = validator.Validate(path, part);

        if (!valid)
        {
            input.ResetPath();
            pathRenderer.Clear();
            Debug.Log("Invalid Path");
            return;
        }
        // otherwise player will follow path
        player.FollowPath(path);
        SwitchToFollowCam();
        OnPathFinished?.Invoke();
    }

    // Advances to next level part and clears path to prepare for next part of the level
    private void HandlePathComplete()
    {
        AdvanceNextPart();
        pathRenderer.Clear();
    }
    
    void AdvanceNextPart()
    {
        currentPartIndex++; // advancing level part index

        if (currentPartIndex >= levelParts.Count) // if we've complete the last part of the level, level is completed
        {
            SwitchToEndCam();
            Debug.Log("LEVEL COMPLETE");
            return;
        }

        SetActiveLevelPart(currentPartIndex);
    }

    // Resets player to the last reached checkpoint
    public void ResetPlayerToCheckPoint()
    {
        if (player == null) return;

        LevelPart part = levelParts[currentPartIndex];
        if (part.startPoint != null)
        {
            Collider startCollider = part.startPoint.GetComponent<Collider>();
            Vector3 resetPos = part.startPoint.position;
            if (startCollider != null)
            {
                // resetting player on top of starting point
                resetPos.y = startCollider.bounds.max.y + 0.5f;
            }
            else
            {
                resetPos.y += 1f;
            }
            player.ResetToStart(resetPos);
            SetActiveLevelPart(currentPartIndex);
        }
        else
        {
            // fallback option
            player.ResetToStart();
        }
        input?.ResetPath();
        pathRenderer?.Clear();
    }

    // Life system
    public void LoseLife()
    {
        if (isGameOver) return;

        // if player's lost a life faster than cooldown, player will not lose a life
        if (Time.time - lastLifeLossTime < lifeLossCooldown)
        {
            return;
        }

        lastLifeLossTime = Time.time;

        currentLives--;
        UpdateHeartsUI();

        Debug.Log("Player lost a life. Lives left: " + currentLives);

        if (currentLives <= 0)
        {
            ResetToBeginning();
            return;
        }

        ResetPlayerToCheckPoint();
    }
    
    // Displaying hearts for current lives
    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].enabled = i < currentLives;
        }
    }

    // Resetting player to the first part of the level
    private void ResetToBeginning()
    {
        currentLives = maxLives;
        UpdateHeartsUI();

        currentPartIndex = 0;

        if (levelParts.Count > 0 && levelParts[0].startPoint != null)
        {
            Collider startCollider = levelParts[0].startPoint.GetComponent<Collider>();

            Vector3 resetPos = levelParts[0].startPoint.position;

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

        input?.ResetPath();
        pathRenderer?.Clear();

        SetActiveLevelPart(currentPartIndex);

        Debug.Log("Out of lives. Reset to beginning.");
    }

    private void GameOver()
    {
        isGameOver = true;
        currentPartIndex = 0;
        Debug.Log("Game Over");

        SetActiveLevelPart(currentPartIndex);
    }
    
    // helper function for getting level part data
    public LevelPart GetLevelPart()
    {
        return levelParts[currentPartIndex];
    }
}
