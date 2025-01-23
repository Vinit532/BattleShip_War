using System;
using UnityEngine;
using TMPro;

public class BattleUIController : MonoBehaviour
{
    [Header("Player Data UI")]
    [SerializeField] private TMP_Text playerTotalFiresText; // Total shots fired by the Player
    [SerializeField] private TMP_Text playerWeaponTypeText; // Last weapon type fired
    [SerializeField] private TMP_Text playerHitsText; // Number of Player hits
    [SerializeField] private TMP_Text playerMissesText; // Number of Player misses
    [SerializeField] private TMP_Text playerPowerUsedText; // Power used by the Player

    [Header("Enemy Data UI")]
    [SerializeField] private TMP_Text enemyFiringSideText; // Side from which the Enemy fired
    [SerializeField] private TMP_Text enemyFiringAngleText; // Angle at which the Enemy fired

    [Header("Dependencies")]
    [SerializeField] private Transform playerShip; // Reference to Player's battleship
    [SerializeField] private Transform enemyShip; // Reference to Enemy's battleship

    // Internal tracking variables
    private int playerTotalFires = 0; // Total shots fired by the Player
    private int playerHits = 0; // Total Player hits
    private int playerMisses = 0; // Total Player misses
    private float playerPowerUsed = 0f; // Total power used by the Player
    private string lastPlayerWeaponType = "N/A"; // Last weapon type fired by the Player

    private void Start()
    {
        // Initialize UI with default values
        ResetUI();

        // Subscribe to events from other scripts
        ProjectileCollisionHandler.OnPlayerHit += HandlePlayerHit;
        ProjectileCollisionHandler.OnPlayerMiss += HandlePlayerMiss;
        CanonController.OnPlayerFire += HandlePlayerFire;
        CruiserController.OnEnemyFire += HandleEnemyFire;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        ProjectileCollisionHandler.OnPlayerHit -= HandlePlayerHit;
        ProjectileCollisionHandler.OnPlayerMiss -= HandlePlayerMiss;
        CanonController.OnPlayerFire -= HandlePlayerFire;
        CruiserController.OnEnemyFire -= HandleEnemyFire;
    }

    private void ResetUI()
    {
        // Reset all UI fields to default values
        playerTotalFiresText.text = "Total Fires: 0";
        playerWeaponTypeText.text = "Last Weapon: N/A";
        playerHitsText.text = "Hits: 0";
        playerMissesText.text = "Misses: 0";
        playerPowerUsedText.text = "Power Used: 0.00";

        enemyFiringSideText.text = "Firing Side: N/A";
        enemyFiringAngleText.text = "Firing Angle: N/A";
    }

    // Event handler for Player firing
    private void HandlePlayerFire(string weaponType, float powerUsed)
    {
        playerTotalFires++;
        lastPlayerWeaponType = weaponType;
        playerPowerUsed += powerUsed;

        UpdatePlayerUI();
    }

    // Event handler for Player hits
    private void HandlePlayerHit()
    {
        playerHits++;
        UpdatePlayerUI();
    }

    // Event handler for Player misses
    private void HandlePlayerMiss()
    {
        playerMisses++;
        UpdatePlayerUI();
    }

    // Event handler for Enemy firing
    private void HandleEnemyFire(Vector3 firingPosition, Vector3 firingDirection)
    {
        // Determine firing side relative to the Player
        Vector3 relativePosition = playerShip.InverseTransformPoint(firingPosition);
        string firingSide = DetermineFiringSide(relativePosition);

        // Calculate firing angle
        Vector3 playerToEnemy = enemyShip.position - playerShip.position;
        float angle = Vector3.Angle(playerToEnemy, firingDirection);

        UpdateEnemyUI(firingSide, $"{angle:F2}°");
    }

    private string DetermineFiringSide(Vector3 relativePosition)
    {
        if (relativePosition.x > 0) return "Right";
        if (relativePosition.x < 0) return "Left";
        if (relativePosition.z > 0) return "Front";
        return "Back";
    }

    private void UpdatePlayerUI()
    {
        // Update Player-related UI fields
        playerTotalFiresText.text = $"Total Fires: {playerTotalFires}";
        playerWeaponTypeText.text = $"Last Weapon: {lastPlayerWeaponType}";
        playerHitsText.text = $"Hits: {playerHits}";
        playerMissesText.text = $"Misses: {playerMisses}";
        playerPowerUsedText.text = $"Power Used: {playerPowerUsed:F2}";
    }

    private void UpdateEnemyUI(string firingSide, string firingAngle)
    {
        // Update Enemy-related UI fields
        enemyFiringSideText.text = $"Firing Side: {firingSide}";
        enemyFiringAngleText.text = $"Firing Angle: {firingAngle}";
    }
}
