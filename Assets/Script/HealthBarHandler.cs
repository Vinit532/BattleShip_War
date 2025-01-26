using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [Header("Health Settings")]
    public Image healthBar; // Reference to the UI health bar image
    public float maxHealth = 100f; // Maximum health value
    private float currentHealth; // Current health value

    [Header("Layer Settings")]
    public LayerMask playerProjectileLayer; // Layer for projectiles that hit the Player
    public LayerMask enemyProjectileLayer; // Layer for projectiles that hit the Enemy

    [Header("Game Over Settings")]
    public GameObject gameOverUI; // Reference to the Game Over UI

    [Header("Victory/Lose UI")]
    public GameObject VictoryImage; // UI Image displayed when Player wins
    public GameObject LoseGameImage; // UI Image displayed when Player loses

    [Header("Object Type")]
    public bool isPlayer; // Is this the player's battleship?

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Ensure health bar is full at the start
        if (healthBar != null)
        {
            healthBar.fillAmount = 1f;
        }

        // Ensure all UI elements are disabled initially
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        if (VictoryImage != null)
        {
            VictoryImage.SetActive(false);
        }

        if (LoseGameImage != null)
        {
            LoseGameImage.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isPlayer)
        {
            if (IsInLayerMask(collision.gameObject, enemyProjectileLayer))
            {
                Debug.Log("Player Battleship hit by Enemy Projectile");
                TakeDamage(5f);
            }
        }
        else
        {
            if (IsInLayerMask(collision.gameObject, playerProjectileLayer))
            {
                Debug.Log("AI Battleship hit by Player Projectile");
                TakeDamage(5f);
            }
        }
    }

    private void TakeDamage(float damage)
    {
        // Decrease health
        currentHealth -= damage;

        // Update health bar UI
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }

        // Log health updates for debugging
        Debug.Log($"{gameObject.name}'s current health: {currentHealth}");

        // Check for health depletion
        if (currentHealth <= 0f)
        {
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        Debug.Log($"{gameObject.name} is destroyed!");

        // Determine whether the Player or AI lost
        if (isPlayer)
        {
            // If the Player loses
            Debug.Log("Game Over: Player Lost!");
            if (LoseGameImage != null)
            {
                LoseGameImage.SetActive(true); // Show Lose UI
            }
        }
        else
        {
            // If the AI loses
            Debug.Log("Game Over: Player Wins!");
            if (VictoryImage != null)
            {
                VictoryImage.SetActive(true); // Show Victory UI
            }
        }

        // Activate Game Over UI (optional, if you need a global UI for game over)
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0f;
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        // Check if the object's layer is in the specified layer mask
        return (layerMask.value & (1 << obj.layer)) > 0;
    }
}
