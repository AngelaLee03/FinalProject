using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPathFollower : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float reachThreshold = 0.05f;

    private Coroutine moveRoutine;
    private Vector3 startPosition;
    private Quaternion startRotation;

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
                Vector3 direction = (target - transform.position).normalized;
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
        OnPathComplete();
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
    }

    public void ResetToStart()
    {
        ResetToStart(startPosition);
    }

    // Called when player successfully completes path
    private void OnPathComplete() 
    {
        Debug.Log("Player reached destination");
    }
}

