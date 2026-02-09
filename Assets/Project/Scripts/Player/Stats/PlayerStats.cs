using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private bool deletePlayerSaves;
    [SerializeField] private List<StatValue> initialStats;

    private Dictionary<StatType, float> runtimeStats = new();

    public event Action<StatType, float> OnStatChanged;

    // Stats Initialization from SO
    private void Awake()
    {
        if (deletePlayerSaves) PlayerPrefs.DeleteAll();

        foreach (var stat in initialStats)
        {
            float savedValue = SaveSystem.LoadMaxStat(stat.type, stat.value);
            SaveSystem.SaveMaxStat(stat.type, savedValue);
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

[Serializable]
public struct StatValue
{
    public StatType type;
    public float value;
}

public enum StatType
{
    MoveSpeed, CatchChance, FishingSpeed
}
