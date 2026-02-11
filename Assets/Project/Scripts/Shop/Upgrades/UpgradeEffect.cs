using UnityEngine;

public abstract class UpgradeEffect : ScriptableObject
{
    public abstract string GetEffectName();
    public abstract void ApplyEffect(float amount);
    public abstract string EffectFormat(float amount);
}
