using System;
using UnityEngine;
using TMPro;

public class BattleUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text firingCountText;
    public TMP_Text hitsText;
    public TMP_Text missesText;
    public TMP_Text powerUsageText;

    private int firingCount = 0;
    private int hits = 0;
    private int misses = 0;
    private float totalPowerUsed = 0f;

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

    private void UpdateFiringStats(string weaponType, float powerUsed)
    {
        firingCount++;
        totalPowerUsed += powerUsed;
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
        powerUsageText.text = $"Power Usage: {totalPowerUsed:F1}";
    }
}
