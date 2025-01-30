using System;
using UnityEngine;
using UnityEngine.UI;

public class CanonController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float maxRotationAngle = 50f;
    public float minRotationAngle = 0f;
    public float rotationSpeed = 10f;

    [Header("Firing Settings")]
    public GameObject cannonBallPrefab;
    public GameObject missilePrefab;
    public GameObject blastEffectPrefab;
    public float firingForce = 500f;
    public float missileSpeed = 100f;
    public Transform[] firePoints;
    public bool canFireMissiles = false;

    [Header("UI Settings")]
    public Image powerChargeUI;  // UI image to show cannon ball charge
    public float maxPower = 1f;  // Max charge power
    private float currentPower = 0f;  // Current charge power

    [Header("Dependencies")]
    public CruiserController cruiserController;
    private bool isParentWeaponActive;
    private Transform parentWeapon;

    public static event Action<string, float, int> OnPlayerFire;


    public static event Action<string, float, int> OnEnemyFire;

    [Header("AI Settings")]
    public bool isAIControlled = false; // Determines if this is AI-controlled

    void Start()
    {
        parentWeapon = transform.parent;
        if (cruiserController == null && !isAIControlled)
        {
            Debug.LogError("CruiserController reference is missing!");
        }
    }

    void Update()
    {
        if (cruiserController != null && cruiserController.weapons.Contains(parentWeapon))
        {
            int weaponIndex = cruiserController.weapons.IndexOf(parentWeapon);
            isParentWeaponActive = (weaponIndex == cruiserController.activeWeaponIndex);
        }

        if (!isParentWeaponActive) return;

        //HandleRotation();

        // Handle firing input
        /*if (Input.GetMouseButton(0))  // Left mouse button is held down
        {
            ChargeCannonBall();
        }
        else if (Input.GetMouseButtonUp(0))  // Left mouse button is released
        {
            FireProjectile();
        } */
        if (!isAIControlled)
        {
            HandleRotation();

            if (canFireMissiles && missilePrefab != null) 
            { 
                if (Input.GetMouseButtonUp(0)) 
                {
                    FireMissile();
                }
                
            }
            else
            {
                // Player input for firing
                if (Input.GetMouseButton(0))
                {
                    ChargeCannonBall();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    FireCannonBall();
                }
            } 
        }
    }

    private void HandleRotation()
    {
        float mouseInput = Input.GetAxis("Mouse Y");
        float currentAngle = transform.localEulerAngles.x;
        currentAngle = (currentAngle > 180f) ? currentAngle - 360f : currentAngle;
        float targetAngle = Mathf.Clamp(currentAngle - mouseInput * rotationSpeed * Time.deltaTime, minRotationAngle, maxRotationAngle);
        transform.localEulerAngles = new Vector3(targetAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    // Increase power as long as the mouse button is held
    private void ChargeCannonBall()
    {
        currentPower = Mathf.Min(currentPower + Time.deltaTime * 0.5f, maxPower);  // Gradually increase power up to maxPower
        powerChargeUI.fillAmount = currentPower;  // Update UI image fill
    }

    // Fire the appropriate projectile
    private void FireProjectile()
    {
        if (canFireMissiles && missilePrefab != null)
        {
            FireMissile();
        }
        else if (cannonBallPrefab != null)
        {
            FireCannonBall();
        }
        else
        {
            Debug.LogWarning("No projectile prefab assigned for firing!");
        }
    }

    // Fire cannon balls with the current power
    public void FireCannonBall()
    {
        int projectilesFired = firePoints.Length;
        foreach (Transform firePoint in firePoints)
        {
            GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, firePoint.rotation);
            var collisionHandler = cannonBall.GetComponent<ProjectileCollisionHandler>();
            if (collisionHandler != null)
            {
                collisionHandler.blastEffectPrefab = blastEffectPrefab;
            }

            Rigidbody cannonBallRb = cannonBall.GetComponent<Rigidbody>();
            if (cannonBallRb != null)
            {
                cannonBallRb.AddForce(firePoint.forward * firingForce * currentPower);  // Apply force based on power
            }
        }

        // Reset power and update UI
        if (!isAIControlled)
        {
            OnPlayerFire?.Invoke("Cannonball", firingForce * currentPower, projectilesFired);
        }
        else
        {
            OnEnemyFire?.Invoke("Cannonball", firingForce * currentPower, projectilesFired);
        }
        currentPower = 0f;
        powerChargeUI.fillAmount = 0f;
    }

    // Fire missile as before
    public void FireMissile()
    {
        int projectilesFired = 1;
        Transform firePoint = firePoints[0];
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(90f, 0f, 0f));
        var collisionHandler = missile.GetComponent<ProjectileCollisionHandler>();
        if (collisionHandler != null)
        {
            collisionHandler.blastEffectPrefab = blastEffectPrefab;
        }

        Rigidbody missileRb = missile.GetComponent<Rigidbody>();
        if (missileRb != null)
        {
            missileRb.velocity = firePoint.forward * missileSpeed;
        }

        if (!isAIControlled)
        {
            OnPlayerFire?.Invoke("Missile", missileSpeed, projectilesFired); // Notify firing stats
        }
        else
        {
            OnEnemyFire?.Invoke("Missile", missileSpeed, projectilesFired);
        }
        
        powerChargeUI.fillAmount = 0f;
    }
}
