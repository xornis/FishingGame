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

    private void Start()
    {
        foreach (var stat in initialStats)
            OnStatChanged?.Invoke(stat.type, GetStat(stat.type));
    }

    private float ValidateStat(StatType type, float value)
    {
        return type switch
        {
            StatType.CatchChance => Mathf.Clamp(value, 1f, 100f),

            StatType.MoveSpeed => Mathf.Clamp(value, 10f, 1000f),
            StatType.FishingSpeed => Mathf.Clamp(value, 10f, 1000f),

            StatType.CoinsPerFish => Mathf.Clamp(value, 1f, 100f),
            _ => value
        };
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
    MoveSpeed, CatchChance, FishingSpeed, CoinsPerFish
}
