using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPathFollower : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float reachThreshold = 0.05f;

    private Coroutine moveRoutine;

    public void FollowPath(List<Vector3> path)
    {
        // Stop any previous movement
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        moveRoutine = StartCoroutine(FollowPathCoroutine(path));
    }

    private IEnumerator FollowPathCoroutine(List<Vector3> path)
    {
        if (path == null || path.Count == 0)
        {
            yield break;
        }

        transform.position = path[0];

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 target = path[i];

            while (Vector3.Distance(transform.position, target) > reachThreshold)
            {
                // Rotating player if needed along path
                Vector3 direction = (target - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.forward = Vector3.Lerp(
                        transform.forward,
                        direction,
                        10f * Time.deltaTime
                        );
                }

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }
        }
        OnPathComplete();
    }
    private void OnPathComplete() 
    {
        Debug.Log("Player reached destination");
    }
}

