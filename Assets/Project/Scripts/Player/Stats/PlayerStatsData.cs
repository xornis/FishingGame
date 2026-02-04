using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPlayerStatsData", menuName = "Scriptable Objects/PlayerStatsData")]
public class PlayerStatsData : ScriptableObject
{
    public List<StatValue> stats;
}

[System.Serializable]
public struct StatValue
{
    public StatType type;
    public float value;
}

public enum StatType
{
    MoveSpeed, MaxHealth, HealthRegen, PickUpRange
}
