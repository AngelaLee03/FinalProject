using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PlayerPathFollower : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float reachThreshold = 0.05f;
    public Transform endCam;
    private Coroutine moveRoutine;
    private Vector3 startPosition;
    private Quaternion startRotation;
    public System.Action OnPathComplete;
    public float moveSoundCooldown = 0.25f;
    private float lastMoveSoundTime;

    // Life system
    public LayerMask damageMask;
    public float collisionCheckRadius = 0.45f;
    private PathGameManager gameManager;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        gameManager = FindAnyObjectByType<PathGameManager>();
    }

    public void FollowPath(List<Vector3> path)
    {
        // Stop any previous movement
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        moveRoutine = StartCoroutine(FollowPathCoroutine(path));
    }

    // Coroutine that moves player along the path
    private IEnumerator FollowPathCoroutine(List<Vector3> path)
    {
        // Ensures that path exists
        if (path == null || path.Count == 0)
        {
            yield break;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartPlayerMoveSound();
        }
                
        // Makes sure player starts from starting position
        transform.position = path[0];

        // Looping through all points in path 
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 target = path[i];

            // Move toward current target point
            while (Vector3.Distance(transform.position, target) > reachThreshold)
            {
                // Rotating player in direction of path if needed
                Vector3 direction = (target - transform.position);
                direction.y = 0f; // Ensures the character stays flat to the ground
                direction.Normalize();
                if (direction != Vector3.zero)
                {
                    transform.forward = Vector3.Lerp(
                        transform.forward,
                        direction,
                        10f * Time.deltaTime
                        );
                }

                // Moving player towards target point
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );

                // Checking if player has hit obstacles or enemies
                if (Physics.CheckSphere(transform.position, collisionCheckRadius, damageMask))
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.StopPlayerMoveSound();
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit);
                    }
                    gameManager.LoseLife();
                    yield break;
                }

                // Waiting before continuing movement
                yield return null;
            }
        }
        moveRoutine = null;

        // Rotates player to look at the camera when finishing level
        Quaternion startRot = transform.rotation;
        Vector3 lookDir = endCam.position - transform.position;
        lookDir.y = 0f;
        transform.forward = lookDir.normalized;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopPlayerMoveSound();
        }

        OnPathComplete?.Invoke();
    }

    // Resetting player back to the starting point
    public void ResetToStart(Vector3 startPoint)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopPlayerMoveSound();
        }
        
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        transform.position = startPoint;
        transform.rotation = startRotation;
    }

    // Fall back if start point is null
    public void ResetToStart()
    {
        ResetToStart(startPosition);
    }
}

