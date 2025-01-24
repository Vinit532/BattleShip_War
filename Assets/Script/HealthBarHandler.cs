using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [Header("Health Settings")]
    public Image healthBar; // Reference to the UI health bar image
    public float maxHealth = 100f; // Max health value
    private float currentHealth; // Current health of the battleship

    [Header("Tag Settings")]
    public bool isPlayer; // Is this the player's battleship?
    public string enemyMissileTag = "EnemyMissile"; // Tag for enemy missiles
    public string enemyCannonBallTag = "EnemyCannonBall"; // Tag for enemy cannonballs
    public string playerMissileTag = "PlayerMissile"; // Tag for player missiles
    public string playerCannonBallTag = "PlayerCannonBall"; // Tag for player cannonballs

    [Header("Game Over Settings")]
    public GameObject gameOverUI; // Reference to the Game Over UI

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Ensure health bar is full at the start
        if (healthBar != null)
        {
            healthBar.fillAmount = 1f;
        }

        // Ensure Game Over UI is disabled initially
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if this is the player's battleship
        if (isPlayer)
        {
            // If hit by enemy projectiles
            if (other.CompareTag(enemyMissileTag) || other.CompareTag(enemyCannonBallTag))
            {
                Debug.Log("Player Battleship hit by " + other.tag);
                TakeDamage(5f);
            }
        }
        else
        {
            // If hit by player projectiles
            if (other.CompareTag(playerMissileTag) || other.CompareTag(playerCannonBallTag))
            {
                Debug.Log("AI Battleship hit by " + other.tag);
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

        // Check for health depletion
        if (currentHealth <= 0f)
        {
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        Debug.Log($"{gameObject.name} is destroyed!");

        // Activate Game Over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0f;
    }
}
