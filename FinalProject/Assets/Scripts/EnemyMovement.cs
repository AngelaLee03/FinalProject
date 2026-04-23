using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    public PathGameManager gameManager;
    public Transform player;
    public float distance = 2f;
    public float moveSpeed = 1.5f;
    public float hitCooldown = 0.75f;

    private Vector3 startPosition;
    private float hitTimer = -999f;
    private Rigidbody enemyRigidbody;
    private Collider enemyCollider;

    private void Awake()
    {
        // ensure enemy has a start position, a rigidbody, and a collider
        startPosition = transform.position;
        enemyRigidbody = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();

        // collision is based on enemy as the trigger
        enemyRigidbody.isKinematic = true;
        enemyRigidbody.useGravity = false;
        enemyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        enemyCollider.isTrigger = true;

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<PathGameManager>();
        }

        if (player == null && gameManager != null && gameManager.player != null)
        {
            player = gameManager.player.transform;
        }
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    // enemy moves a certain distance back and forth
    private void MoveEnemy()
    {
        Vector3 offset = new Vector3(Mathf.Sin(Time.time * moveSpeed) * distance, 0f, 0f);
        enemyRigidbody.MovePosition(startPosition + offset);
    }

    // if enemy collides with player
    private void OnTriggerEnter(Collider other)
    {
        TryResetFromCollider(other.transform); // reset back to start point
    }

    // as long as something is being triggered, reset back to start
    private void OnTriggerStay(Collider other)
    {
        TryResetFromCollider(other.transform); 
    }

    private void TryResetFromCollider(Transform other)
    {
        // incase references are missing don't do anything
        if (player == null || gameManager == null) 
        {
            return;
        }

        // check if we are in collision cooldown
        if (Time.time - hitTimer < hitCooldown)
        {
            return;
        }

        // check if the enemy hit an obstacle instead of a player (only player counts as collison)
        if (other.root != player.root)
        {
            return;
        }

        hitTimer = Time.time;
        Debug.Log("Player was hit by enemy and sent back to start");
        gameManager.ResetPlayerToStart(); // reset enemy to starting point
    }
}
