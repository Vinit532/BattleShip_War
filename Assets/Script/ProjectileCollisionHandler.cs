using System;
using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    public string avoidCollisionWithTag;  // Tag to avoid collision with.
    public LayerMask avoidCollisionWithLayer;  // Layer to avoid collision with.

    public GameObject blastEffectPrefab;  // Reference to the blast effect prefab.

    public static event Action OnPlayerHit;
    public static event Action OnPlayerMiss;

    public static event Action OnEnemyHit;
    public static event Action OnEnemyMiss;

    public AudioSource weaponSound;

    [SerializeField] bool isPlayerAttack;
   
    // On collision, check the type of object and update stats accordingly
    void OnCollisionEnter(Collision collision)
    {
        weaponSound.Stop();
        // Ensure the object is not one we are avoiding
        if (collision.gameObject.CompareTag(avoidCollisionWithTag) || ((1 << collision.gameObject.layer) & avoidCollisionWithLayer) != 0)
        {
            return;  // Do nothing if the collision is with an unwanted object
        }
        if (isPlayerAttack)
        {
            if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.layer == LayerMask.NameToLayer("EnemyShip"))
            {
                OnPlayerHit?.Invoke();  // Increment hits counter in BattleUIController
            }
            else
            {
                OnPlayerMiss?.Invoke();  // Increment misses counter in BattleUIController
            }
            // If the projectile collides with an object tagged "EnemyShip" (on Enemy layer)
        }
        else 
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.layer == LayerMask.NameToLayer("PlayerShip"))
            {
                OnEnemyHit?.Invoke();  // Increment hits counter in BattleUIController
            }
            else
            {
                OnEnemyMiss?.Invoke();  // Increment misses counter in BattleUIController
            }
        }
        
       

        // Instantiate blast effect at the collision point
        if (blastEffectPrefab != null)
        {
            if (collision.gameObject.CompareTag("SeaBottom"))
            {
                Debug.Log("Weapon crashed to Sea Bottom");
            }
            else
            {
                InstantiateBlastEffect(collision.contacts[0].point, collision.contacts[0].normal);
            }
        }

       
        
        Destroy(gameObject);
    }

    private void InstantiateBlastEffect(Vector3 position, Vector3 normal)
    {
        // Instantiate the blast effect prefab at the collision point with correct rotation
        GameObject blastEffect = Instantiate(blastEffectPrefab, position, Quaternion.LookRotation(normal));
        Destroy(blastEffect, 1f);  // Destroy the blast effect after 1 second
    }
}
