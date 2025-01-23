using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserController : MonoBehaviour  
{
    [Header("Floating Mechanic")]
    public Transform waterPlane; // Reference to the Water_Plane transform.
    public float buoyancyForce = 15f; // Force pushing the Cruiser upwards to float.
    public float buoyancyHeight = 1.5f; // Height at which the Cruiser starts to float.
    public float buoyancyDamping = 0.8f; // Reduces oscillations in floating.
    public float waterDrag = 1f; // Drag applied to simulate water resistance.

    [Header("Movement")]
    public float maxSpeed = 10f; // Maximum forward/backward speed.
    public float accelerationRate = 5f; // How quickly the Cruiser accelerates.
    public float decelerationRate = 7f; // How quickly the Cruiser decelerates when stopping.
    public float turnSpeed = 50f; // Left/right turning speed.
    public float stopDrag = 2f; // Drag when no input is applied.

    private float currentSpeed = 0f; // Current forward/backward speed.
    private Rigidbody rb;

    [Header("Weapons")]
    public List<Transform> weapons; // List of weapon GameObjects.
    public int activeWeaponIndex = -1; // Active weapon index (-1 means no weapon active).

    public static event Action<Vector3, Vector3> OnEnemyFire;

    // Trigger this event with firing position and direction when the Enemy fires.


    void Start()
    {
        // Cache the Rigidbody component and set water-like physics properties.
        rb = GetComponent<Rigidbody>();
        rb.drag = stopDrag;
        rb.angularDrag = waterDrag;

        // Make all weapons visible but deactivated.
        foreach (Transform weapon in weapons)
        {
            EnableWeaponVisualOnly(weapon);
        }
    }

    void FixedUpdate()
    {
        HandleBuoyancy(); // Apply floating physics in FixedUpdate.
        HandleMovement(); // Process Cruiser movement.
    }

    void Update()
    {
        HandleWeaponActivation(); // Check for weapon activation.
        HandleWeaponRotation();   // Rotate the active weapon.
    }

    // --- BUOYANCY MECHANIC ---
    void HandleBuoyancy()
    {
        if (waterPlane == null) return;

        // Get the current position of the water plane and Cruiser.
        float waterHeight = waterPlane.position.y; // Height of the water plane.
        float cruiserHeight = transform.position.y; // Height of the Cruiser.

        // Calculate the depth of the Cruiser relative to the water plane.
        float depthDifference = waterHeight + buoyancyHeight - cruiserHeight;

        // Apply buoyancy force if the Cruiser is below the water surface.
        if (depthDifference > 0)
        {
            // Add upward buoyancy force proportional to the depthDifference.
            Vector3 buoyancy = Vector3.up * (buoyancyForce * depthDifference);

            // Add damping to stabilize the floating motion.
            buoyancy -= rb.velocity * buoyancyDamping;

            // Apply the buoyancy force to the Rigidbody.
            rb.AddForce(buoyancy, ForceMode.Acceleration);
        }
    }

    // --- MOVEMENT MECHANIC ---
    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical"); // Forward/backward input (W/S or Up/Down).
        float turnInput = Input.GetAxis("Horizontal"); // Left/right input (A/D or Left/Right).

        // Handle acceleration and deceleration for Forward/Backward movement.
        if (moveInput != 0)
        {
            // Gradually increase speed towards maxSpeed in the direction of moveInput.
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * moveInput, accelerationRate * Time.fixedDeltaTime);
        }
        else
        {
            // Gradually reduce speed to 0 when no input is given.
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, decelerationRate * Time.fixedDeltaTime);
        }

        // Apply force for forward/backward movement based on currentSpeed.
        Vector3 forwardMovement = transform.forward * currentSpeed;
        rb.AddForce(forwardMovement, ForceMode.Force);

        // Apply torque for turning.
        rb.AddTorque(Vector3.up * turnInput * turnSpeed, ForceMode.Force);
    }

    // --- WEAPON ACTIVATION ---
    void HandleWeaponActivation()
    {
        // Activate weapons using number keys (1 to 9).
        for (int i = 0; i < weapons.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                ActivateWeapon(i);
            }
        }

        // Deactivate all weapons with Backspace.
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeactivateAllWeapons();
        }
    }

    void ActivateWeapon(int index)
    {
        // Deactivate the previously active weapon.
        if (activeWeaponIndex != -1)
        {
            ResetWeaponRotation(activeWeaponIndex);
            DisableWeaponInteraction(weapons[activeWeaponIndex]);
        }

        // Activate the new weapon.
        activeWeaponIndex = index;
        EnableWeaponInteraction(weapons[activeWeaponIndex]);
    }

    void DeactivateAllWeapons()
    {
        if (activeWeaponIndex != -1)
        {
            ResetWeaponRotation(activeWeaponIndex);
            DisableWeaponInteraction(weapons[activeWeaponIndex]);
            activeWeaponIndex = -1;
        }
    }

    // --- WEAPON INTERACTION ---
    void HandleWeaponRotation()
    {
        if (activeWeaponIndex == -1) return; // No active weapon, skip rotation.

        Transform activeWeapon = weapons[activeWeaponIndex];

        // Rotate weapon on the Y-axis with "Q" and "E".
        if (Input.GetKey(KeyCode.Q))
        {
            activeWeapon.Rotate(Vector3.up, 30f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            activeWeapon.Rotate(Vector3.up, -30f * Time.deltaTime);
        }
    }

    void ResetWeaponRotation(int index)
    {
        weapons[index].localRotation = Quaternion.identity; // Reset to default rotation.
    }

    void EnableWeaponVisualOnly(Transform weapon)
    {
        weapon.gameObject.SetActive(true); // Ensure weapon is visible.
        Collider weaponCollider = weapon.GetComponent<Collider>();
        if (weaponCollider != null) weaponCollider.enabled = false; // Disable interaction.
    }

    void EnableWeaponInteraction(Transform weapon)
    {
        weapon.gameObject.SetActive(true); // Make it visible.
        Collider weaponCollider = weapon.GetComponent<Collider>();
        if (weaponCollider != null) weaponCollider.enabled = true; // Enable interaction.
    }

    void DisableWeaponInteraction(Transform weapon)
    {
        Collider weaponCollider = weapon.GetComponent<Collider>();
        if (weaponCollider != null) weaponCollider.enabled = false; // Disable interaction.
    }


}
