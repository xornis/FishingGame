using System;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Stat Texts")]
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI fishingSpeedText;
    [SerializeField] private TextMeshProUGUI catchChanceText;
    [SerializeField] private TextMeshProUGUI coinsPerFishText;

    private void Start() => InitializeStats();

    private void OnEnable() => playerStats.OnStatChanged += HandleStatChanged;
    private void OnDisable() => playerStats.OnStatChanged -= HandleStatChanged;

    private void InitializeStats()
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            UpdateStatText(statType, playerStats.GetStat(statType));
    }

    private void HandleStatChanged(StatType type, float value) => UpdateStatText(type, value);

    private void UpdateStatText(StatType type, float value)
    {
        switch (type)
        {
            case StatType.MoveSpeed:
                moveSpeedText.text = $"Move Speed: {value:F2}s";
                break;
            case StatType.FishingSpeed:
                fishingSpeedText.text = $"Fishing Speed: {value:F2}s";
                break;
            case StatType.CatchChance:
                catchChanceText.text = $"Catch Chance: {value:F0}%";
                break;
            case StatType.CoinsPerFish:
                coinsPerFishText.text = $"Coins Per Fish: {Mathf.RoundToInt(value)}";
                break;
        }
    }
}
