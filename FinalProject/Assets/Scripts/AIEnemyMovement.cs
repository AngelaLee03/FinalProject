using UnityEngine;
using UnityEngine.AI;

public class AIEnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform player;

    // patrolling
    public float stoppingDistance = 0.5f;
    public float patrolRadius = 50f; // radius to sample random points within

    // attacking
    public float timeBetweenAttacks = 0.5f;
    bool alreadyAttacked;

    public float attackRange = 1f;

    private Vector3 currentPatrolTarget;
    public PathGameManager gameManager;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<PathGameManager>();
        }
    }

    private void Start()
    {
        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            PickNewPatrolTarget();
        }
    }

    // states: attacking, patrolling
    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
            return;
        }

        Patrolling();
    }

    private void Patrolling()
    {
        if (agent == null)
        {
            return;
        }

        if (agent.pathPending)
        {
            return;
        }

        // if reached current patrol target, pick a new random one
        if (agent.remainingDistance <= stoppingDistance)
        {
            PickNewPatrolTarget();
        }
    }

    private void PickNewPatrolTarget()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            currentPatrolTarget = hit.position;
            agent.SetDestination(currentPatrolTarget);
        }
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit);
            
            gameManager.LoseLife();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
