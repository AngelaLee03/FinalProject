using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PlayerPathFollower : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float reachThreshold = 0.05f;
    public Transform endCam;
    // Optional: assign the single smart enemy that should chase this player
    public AIEnemyMovement smartEnemy;
    private Coroutine moveRoutine;
    private Vector3 startPosition;
    private Quaternion startRotation;
    public System.Action OnPathComplete;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void FollowPath(List<Vector3> path)
    {
        // Stop any previous movement
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        // If a smart enemy is assigned, tell it to start chasing after its delay
        if (smartEnemy != null)
        {
            smartEnemy.StartChasing();
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

                // Waiting before continuing movement
                yield return null;
            }
        }
        moveRoutine = null;
        // Rotates player to look at the camera when finishing
        Vector3 lookDir = endCam.position - transform.position;
        lookDir.y = 0f;
        transform.forward = lookDir.normalized;
        OnPathComplete?.Invoke();

        // Stop the smart enemy from chasing when path finishes
        if (smartEnemy != null)
        {
            smartEnemy.StopChasing();
        }
    }

    public void ResetToStart(Vector3 startPoint)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        transform.position = startPoint;
        transform.rotation = startRotation;

        // Reset assigned smart enemy back to its start
        if (smartEnemy != null)
        {
            smartEnemy.ResetToStart();
        }
    }

    public void ResetToStart()
    {
        ResetToStart(startPosition);
    }
}

