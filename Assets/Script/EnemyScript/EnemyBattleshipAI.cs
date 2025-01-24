using UnityEngine;
using UnityEngine.AI;

public class EnemyBattleshipAI : MonoBehaviour
{
    [Header("Navigation Settings")]
    public Transform playerCruiser;  // Reference to the player's Cruiser
    public float attackRange = 200f; // Firing range

    [Header("Firing Settings")]
    public float firingInterval = 3f; // Time between consecutive attacks
    private float firingCooldown;

    [Header("Dependencies")]
    public CanonController canonController; // Reference to the firing logic script
    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component

    void Start()
    {
        if (playerCruiser == null)
        {
            Debug.LogError("Player Cruiser is not assigned!");
            return;
        }

        if (canonController == null)
        {
            Debug.LogError("CanonController is not assigned!");
            return;
        }

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned!");
            return;
        }

        // Initialize firing cooldown
        firingCooldown = firingInterval;
    }

    void Update()
    {
        if (playerCruiser == null) return;

        // Navigate towards the player's Cruiser
        navMeshAgent.SetDestination(playerCruiser.position);

        // Check attack range
        float distanceToPlayer = Vector3.Distance(transform.position, playerCruiser.position);
        if (distanceToPlayer <= attackRange)
        {
            // Fire at the player if within range and cooldown is ready
            firingCooldown -= Time.deltaTime;
            if (firingCooldown <= 0f)
            {
                FireAtPlayer();
                firingCooldown = firingInterval; // Reset cooldown
            }
        }
    }

    private void FireAtPlayer()
    {
        if (canonController != null)
        {
            if (canonController.canFireMissiles)
            {
                canonController.FireMissile();
            }
            else
            {
                canonController.FireCannonBall();
            }
        }
    }
}
