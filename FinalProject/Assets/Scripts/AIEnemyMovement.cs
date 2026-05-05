using UnityEngine;
using UnityEngine.SceneManagement;

public class AIEnemyMovement : MonoBehaviour
{
    public Transform player;

    // patrolling
    public Transform[] patrolPoints = new Transform[2];
    private int currentPatrolPoint = 0;
    public float patrolSpeed = 1.5f;
    public float stoppingDistance = 0.5f;
    private Vector3 startingPosition;

    // attacking
    public float timeBetweenAttacks = 0.5f;
    bool alreadyAttacked;

    // chasing
    // speed and delay can be edited in the inspector
    public float chaseSpeed = 2.5f;
    public float attackRange = 1f;
    public float chaseStartDelay = 0.5f;
    private bool isChaseable = false;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        startingPosition = transform.position;
    }

    private void OnEnable()
    {
        // reset chase state when scene is loaded
        isChaseable = false;
        alreadyAttacked = false;
        currentPatrolPoint = 0;
        // cancel any pending invokes when resetting
        CancelInvoke();
        transform.position = startingPosition;
    }

    // states: attacking, patrolling, chasing
    private void Update()
    {
        // check if enemy is attacking
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange) // if the player is within attack range
        {
            AttackPlayer();
            return;
        }

        // if can't chase, patroll instead
        if (!isChaseable)
        {
            Patrolling();
        }
        else
        {
            // chase the player
            ChasePlayer();
        }
    }

    public void StartChasing()
    {
        // call this method when the player starts moving
        // ensure we don't queue multiple invokes
        CancelInvoke(nameof(EnableChase));
        Invoke(nameof(EnableChase), chaseStartDelay);
    }

    public void StopChasing()
    {
        // stop any pending chase start and disable chasing immediately
        CancelInvoke(nameof(EnableChase));
        isChaseable = false;
    }

    public void ResetToStart()
    {
        // resets enemy to its starting position and patrol state without reloading scene
        StopChasing();
        alreadyAttacked = false;
        currentPatrolPoint = 0;
        transform.position = startingPosition;
    }

    private void EnableChase()
    {
        isChaseable = true;
    }
    private void Patrolling()
    {
        // check if patrol points are assigned
        if (patrolPoints.Length < 2 || patrolPoints[0] == null || patrolPoints[1] == null)
        {
            Debug.LogWarning("Patrol points not properly assigned on " + gameObject.name);
            return;
        }

        Vector3 targetPoint = patrolPoints[currentPatrolPoint].position;
        Vector3 direction = (targetPoint - transform.position).normalized;
        transform.position += direction * patrolSpeed * Time.deltaTime;

        // check if reached patrol point
        if (Vector3.Distance(transform.position, targetPoint) <= stoppingDistance)
        {
            // switch to next patrol point
            currentPatrolPoint = (currentPatrolPoint + 1) % 2;
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    private void AttackPlayer()
    {
        // attack cooldown
        if (!alreadyAttacked)
        {
            // restart the level
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            alreadyAttacked = true;
        }
    }
}
