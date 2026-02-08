using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatsData baseData;
    [SerializeField] private bool deletePlayerSaves;

    private Dictionary<StatType, float> runtimeStats = new();

    public event Action<StatType, float> OnStatChanged;

    // Stats Initialization from SO
    private void Awake()
    {
        if (deletePlayerSaves) PlayerPrefs.DeleteAll();

        foreach (var stat in baseData.initialStats)
        {
            float savedValue = SaveSystem.LoadMaxStat(stat.type, stat.value);
            runtimeStats[stat.type] = ValidateStat(stat.type, savedValue);
        }
    }

    public float GetStat(StatType type) => runtimeStats.GetValueOrDefault(type, 1);

    public void UpdateStat(StatType type, float value)
    {
        if (runtimeStats.ContainsKey(type))
        {
            float valueByKey = runtimeStats[type];
            float newValue = valueByKey + value;
            float validatedValue = ValidateStat(type, newValue);

            runtimeStats[type] = validatedValue;
            OnStatChanged?.Invoke(type, validatedValue);
        }
    }

    private float ValidateStat(StatType type, float value)
    {
        return type switch
        {
            StatType.CatchChance => Mathf.Clamp(value, 1f, 100f),

            StatType.MoveSpeed => Mathf.Clamp(value, 10f, 9999f),
            StatType.FishingSpeed => Mathf.Clamp(value, 10f, 9999f),
            _ => value
        };
    }
}
