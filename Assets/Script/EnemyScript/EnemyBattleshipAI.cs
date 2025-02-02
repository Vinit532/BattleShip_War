using UnityEngine;
using UnityEngine.AI;

public class EnemyBattleshipAI : MonoBehaviour
{
    [Header("Navigation Settings")]
    public Transform playerCruiser;  // Reference to the player's Cruiser
    public float attackRange = 200f; // Firing range
    public float minSafeDistance = 50f; // Minimum distance to keep from the player
    public float maxSpeed = 10f; // Maximum speed of the AI battleship
    public float turnSpeed = 50f; // Turning speed of the AI battleship
    public float stoppingDistance = 10f; // Distance to start slowing down

    [Header("Firing Settings")]
    public float firingInterval = 3f; // Time between consecutive attacks
    private float firingCooldown;
    public float missileRange = 150f; // Preferred range for firing missiles
    public float cannonballRange = 100f; // Preferred range for firing cannonballs
    public float minCannonballForce = 300f; // Minimum force for cannonballs
    public float maxCannonballForce = 800f; // Maximum force for cannonballs

    [Header("Weapons")]
    public Transform weaponsParent; // Reference to the "Weapons" parent object
    private CanonController[] weaponControllers; // Array to hold all weapon controllers

    [Header("Dependencies")]
    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component

    private bool fireMissiles = false; // Toggle between missiles and cannonballs

    void Start()
    {
        if (playerCruiser == null)
        {
            Debug.LogError("Player Cruiser is not assigned!");
            return;
        }

        if (weaponsParent == null)
        {
            Debug.LogError("Weapons parent is not assigned!");
            return;
        }

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned!");
            return;
        }

        // Initialize firing cooldown
        firingCooldown = firingInterval;

        // Get all CanonController components from the child weapons
        weaponControllers = weaponsParent.GetComponentsInChildren<CanonController>();

        if (weaponControllers == null || weaponControllers.Length == 0)
        {
            Debug.LogError("No weapon controllers found!");
        }

        // Set NavMeshAgent speed and angular speed
        navMeshAgent.speed = maxSpeed;
        navMeshAgent.angularSpeed = turnSpeed;
        navMeshAgent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (playerCruiser == null) return;

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerCruiser.position);

        // Maintain minimum safe distance
        if (distanceToPlayer < minSafeDistance)
        {
            // Smoothly decelerate as we approach the safe distance
            navMeshAgent.speed = Mathf.Lerp(navMeshAgent.speed, 0, Time.deltaTime * 2f);
        }
        else
        {
            // Move towards the player at full speed
            navMeshAgent.speed = maxSpeed;
            navMeshAgent.SetDestination(playerCruiser.position);
        }

        // Check attack range
        if (distanceToPlayer <= attackRange)
        {
            // Fire at the player if within range and cooldown is ready
            firingCooldown -= Time.deltaTime;
            if (firingCooldown <= 0f)
            {
                DecideAndFire(distanceToPlayer);
                firingCooldown = firingInterval; // Reset cooldown
            }
        }
    }

    private void DecideAndFire(float distanceToPlayer)
    {
        if (weaponControllers == null || weaponControllers.Length == 0) return;

        // Decide whether to fire missiles or cannonballs based on distance
        if (distanceToPlayer >= missileRange)
        {
            fireMissiles = true; // Prefer missiles at longer range
        }
        else if (distanceToPlayer <= cannonballRange)
        {
            fireMissiles = false; // Prefer cannonballs at closer range
        }

        // Find the best weapon to fire based on the player's position
        CanonController bestWeapon = GetBestWeaponForFiring();
        if (bestWeapon != null)
        {
            if (fireMissiles && bestWeapon.canFireMissiles)
            {
                bestWeapon.FireMissile();
            }
            else
            {
                // Calculate dynamic force for cannonballs based on distance
                float force = Mathf.Lerp(minCannonballForce, maxCannonballForce, distanceToPlayer / attackRange);

                // Temporarily modify the firingForce in CanonController
                float originalFiringForce = bestWeapon.firingForce;
                bestWeapon.firingForce = force;

                // Fire the cannonball
                bestWeapon.FireCannonBall();

                // Restore the original firingForce
                bestWeapon.firingForce = originalFiringForce;
            }
        }
    }

    private CanonController GetBestWeaponForFiring()
    {
        CanonController bestWeapon = null;
        float bestAngle = float.MaxValue;

        foreach (var weapon in weaponControllers)
        {
            if (weapon == null) continue;

            // Calculate the angle between the weapon's forward direction and the player's position
            Vector3 directionToPlayer = (playerCruiser.position - weapon.transform.position).normalized;
            float angle = Vector3.Angle(weapon.transform.forward, directionToPlayer);

            // Choose the weapon with the smallest angle to the player
            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestWeapon = weapon;
            }
        }

        return bestWeapon;
    }
}