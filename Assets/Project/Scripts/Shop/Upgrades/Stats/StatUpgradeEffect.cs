using UnityEngine;

[CreateAssetMenu(fileName = "NewStatUpgradeEffect", menuName = "Scriptable Objects/Shop/Effects/Stat")]
public class StatUpgradeEffect : UpgradeEffect
{
    [SerializeField] private StatType statType;

    public override string GetEffectName() => statType.ToString();

    public override void ApplyEffect(int amount)
    {
        float currentMax = SaveSystem.LoadMaxStat(statType, 0);
        float newValue = currentMax + amount;

        SaveSystem.SaveMaxStat(statType, newValue);
    }

    public override string EffectFormat(int amount)
    {
        return statType == StatType.CatchChance
            ? $"+{amount}%"
            : $"+{amount / 10f}%";
    }
}
