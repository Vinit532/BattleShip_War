using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    public string avoidCollisionWithTag;  // Tag to avoid collision with.
    public LayerMask avoidCollisionWithLayer;  // Layer to avoid collision with.

    public GameObject blastEffectPrefab;  // Reference to the blast effect prefab.

    public static event Action OnPlayerHit;
    public static event Action OnPlayerMiss;

    // Trigger these events where collisions with EnemyShip or other objects occur.


    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag or layer we want to avoid.
        if (collision.gameObject.CompareTag(avoidCollisionWithTag) || ((1 << collision.gameObject.layer) & avoidCollisionWithLayer) != 0)
        {
            // If the collision is with an object we should avoid, do nothing.
            return;
        }

        // Instantiate the blast effect at the collision point.
        if (blastEffectPrefab != null)
        {
            InstantiateBlastEffect(collision.contacts[0].point, collision.contacts[0].normal);
        }

        // Destroy the projectile on collision.
        Destroy(gameObject);
    }

    private void InstantiateBlastEffect(Vector3 position, Vector3 normal)
    {
        // Instantiate the blast effect prefab at the collision point with the correct rotation.
        GameObject blastEffect = Instantiate(blastEffectPrefab, position, Quaternion.LookRotation(normal));

        // Destroy the particle effect after its duration (1 second).
        Destroy(blastEffect, 1f);
    }
}
