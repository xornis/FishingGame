using UnityEngine;

[CreateAssetMenu(fileName = "NewStatUpgradeEffect", menuName = "Scriptable Objects/Shop/Effects/Stat")]
public class StatUpgradeEffect : UpgradeEffect
{
    [SerializeField] private StatType statType;

    public override string GetEffectName() => statType.ToString();

    public override void ApplyEffect(float amount)
    {
        float currentMax = SaveSystem.LoadMaxStat(statType, 0);

        float bonus = statType == StatType.CatchChance
            ? amount 
            : currentMax / (-amount);

        SaveSystem.SaveMaxStat(statType, currentMax + bonus);
    }

    public override string EffectFormat(float amount)
    {
        if (statType == StatType.FishingSpeed || statType == StatType.MoveSpeed)
            return $"-{amount}%";
        else
            return $"+{amount}%";
    }    
}
