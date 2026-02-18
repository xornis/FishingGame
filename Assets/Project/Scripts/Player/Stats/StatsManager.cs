using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private bool deletePlayerSaves;
    [SerializeField] private List<StatValue> initialStats;

    private Dictionary<StatType, float> runtimeStats = new();

    public event Action<StatType, float> OnStatChanged;

    private void OnValidate()
    {
        if (deletePlayerSaves) PlayerPrefs.DeleteAll();
    }

    // Stats Initialization from SO
    private void Awake()
    {
        foreach (var stat in initialStats)
        {
            float savedValue = SaveSystem.LoadMaxStat(stat.type, stat.value);
            SaveSystem.SaveMaxStat(stat.type, savedValue);
            runtimeStats[stat.type] = ValidateStat(stat.type, savedValue);
        }
    }

    private void Start()
    {
        RefreshAllStats();
    }

    public void RefreshAllStats()
    {
        foreach (var stat in runtimeStats)
            OnStatChanged?.Invoke(stat.Key, stat.Value);
    }

    private float ValidateStat(StatType type, float value)
    {
        return type switch
        {
            StatType.MoveSpeed => Mathf.Clamp(value, 0.05f, 10f),
            StatType.FishingSpeed => Mathf.Clamp(value, 0.05f, 10f),

            StatType.CatchChance => Mathf.Clamp(value, 1f, 100f),
            StatType.CoinsPerFish => Mathf.Clamp(value, 1f, 1000f),

            _ => value
        };
    }

    public void ChangeStat(StatType type, float value)
    {
        if (!runtimeStats.ContainsKey(type)) return;

        float valueByKey = runtimeStats[type];
        float newValue = valueByKey + value;
        float validatedValue = ValidateStat(type, newValue);

        runtimeStats[type] = validatedValue;

        SaveSystem.SaveMaxStat(type, validatedValue);
        OnStatChanged?.Invoke(type, validatedValue);
    }

    public float GetStat(StatType type) => runtimeStats.GetValueOrDefault(type, 1);
}

[Serializable]
public struct StatValue
{
    public StatType type;
    public float value;
}

public enum StatType
{
    MoveSpeed, FishingSpeed, CatchChance, CoinsPerFish
}
