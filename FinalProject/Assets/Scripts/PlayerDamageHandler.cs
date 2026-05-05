using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    public PathGameManager gameManager;
    public LayerMask damageMask;
    public float hitCooldown = 0.75f;

    private float lastHitTime = -999f;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<PathGameManager>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryTakeDamage(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryTakeDamage(other);
    }

    private void TryTakeDamage(Collider other)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

        bool isDamageObject = (damageMask.value & (1 << other.gameObject.layer)) != 0;

        if (!isDamageObject) return;

        lastHitTime = Time.time;
        gameManager.LoseLife();
    }
}
