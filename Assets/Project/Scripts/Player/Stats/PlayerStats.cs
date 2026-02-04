using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatsData baseData;

    private Dictionary<StatType, float> runtimeStats = new();

    public event Action<StatType, float> OnStatChanged;

    // Stats Initialization from SO
    private void Awake()
    {
        foreach (var stat in baseData.stats)
            runtimeStats[stat.type] = stat.value;
    }

    public float GetStat(StatType type) => runtimeStats.GetValueOrDefault(type, 0);

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
            StatType.MaxHealth => Mathf.Max(1, value),
            StatType.MoveSpeed => Mathf.Max(0, value),
            _ => value
        };
    }
}
