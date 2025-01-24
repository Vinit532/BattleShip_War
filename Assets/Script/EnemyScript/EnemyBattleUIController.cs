using UnityEngine;
using TMPro;

public class EnemyBattleUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text firingCountText;
    public TMP_Text hitsText;
    public TMP_Text missesText;
    public TMP_Text powerUsageText;
    public TMP_Text weaponTypeText;

    private int firingCount = 0;
    private int hits = 0;
    private int misses = 0;
    private string currentWeaponType;

    void OnEnable()
    {
        CanonController.OnPlayerFire += UpdateFiringStats;
        ProjectileCollisionHandler.OnPlayerHit += UpdateHits;
        ProjectileCollisionHandler.OnPlayerMiss += UpdateMisses;
    }

    void OnDisable()
    {
        CanonController.OnPlayerFire -= UpdateFiringStats;
        ProjectileCollisionHandler.OnPlayerHit -= UpdateHits;
        ProjectileCollisionHandler.OnPlayerMiss -= UpdateMisses;
    }

    private void UpdateFiringStats(string weaponType, float firingPower, int projectilesFired)
    {
        firingCount += projectilesFired;
        currentWeaponType = weaponType;
        powerUsageText.text = $"Power Usage: {firingPower:F1}";
        RefreshUI();
    }

    private void UpdateHits()
    {
        hits++;
        RefreshUI();
    }

    private void UpdateMisses()
    {
        misses++;
        RefreshUI();
    }

    private void RefreshUI()
    {
        firingCountText.text = $"Firing Count: {firingCount}";
        hitsText.text = $"Hits: {hits}";
        missesText.text = $"Misses: {misses}";
        weaponTypeText.text = $"Weapon Type: {currentWeaponType}";
    }
}
