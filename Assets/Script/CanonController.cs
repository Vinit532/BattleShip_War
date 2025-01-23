using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float maxRotationAngle = 50f;
    public float minRotationAngle = 0f;
    public float rotationSpeed = 10f; 

    [Header("Firing Settings")]
    public GameObject cannonBallPrefab; // Cannon Ball prefab.
    public GameObject missilePrefab; // Missile prefab.
    public GameObject blastEffectPrefab; // Blast particle effect prefab.
    public float firingForce = 500f; // Cannon Ball firing force.
    public float missileSpeed = 100f; // Missile speed.
    public Transform[] firePoints; // Array of fire points.
    public bool canFireMissiles = false; // Determines if the weapon can fire Missiles.

    [Header("Dependencies")]
    public CruiserController cruiserController;
    private bool isParentWeaponActive;
    private Transform parentWeapon;


    public static event Action<string, float> OnPlayerFire;

    // Trigger this event with weapon type and power usage whenever the Player fires.

    void Start()
    {
        parentWeapon = transform.parent;

        if (cruiserController == null)
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

        HandleRotation();

        if (Input.GetMouseButtonDown(0)) // Left mouse button.
        {
            FireProjectile();
        }
    }

    // --- ROTATION LOGIC ---
    private void HandleRotation()
    {
        float mouseInput = Input.GetAxis("Mouse Y");

        float currentAngle = transform.localEulerAngles.x;
        currentAngle = (currentAngle > 180f) ? currentAngle - 360f : currentAngle;

        float targetAngle = Mathf.Clamp(currentAngle - mouseInput * rotationSpeed * Time.deltaTime, minRotationAngle, maxRotationAngle);

        transform.localEulerAngles = new Vector3(targetAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    // --- FIRING LOGIC ---
    private void FireProjectile()
    {
        if (canFireMissiles && missilePrefab != null)
        {
            FireMissile();
        }
        else if (cannonBallPrefab != null)
        {
            FireCannonBalls();
        }
        else
        {
            Debug.LogWarning("No projectile prefab assigned for firing!");
        }
    }

    private void FireCannonBalls()
    {
        if (firePoints.Length == 0)
        {
            Debug.LogWarning("No fire points assigned for cannon balls!");
            return;
        }

        foreach (Transform firePoint in firePoints)
        {
            GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, firePoint.rotation);

            // Assign the blast effect to the projectile's collision handler.
            var collisionHandler = cannonBall.GetComponent<ProjectileCollisionHandler>();
            if (collisionHandler != null)
            {
                collisionHandler.blastEffectPrefab = blastEffectPrefab;
            }

            Rigidbody cannonBallRb = cannonBall.GetComponent<Rigidbody>();
            if (cannonBallRb != null)
            {
                cannonBallRb.AddForce(firePoint.forward * firingForce);
            }
        }

        // Invoke OnPlayerFire event for firing a cannonball.
        OnPlayerFire?.Invoke("Cannonball", firingForce);
    }

    private void FireMissile()
    {
        if (firePoints.Length == 0)
        {
            Debug.LogWarning("No fire points assigned for missiles!");
            return;
        }

        Transform firePoint = firePoints[0];
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(90f, 0f, 0f));

        // Assign the blast effect to the projectile's collision handler.
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

        // Invoke OnPlayerFire event for firing a missile.
        OnPlayerFire?.Invoke("Missile", missileSpeed);
    }

}
