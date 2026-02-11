using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceUpgradeEffect", menuName = "Scriptable Objects/Shop/Effects/Resource")]
public class ResourceUpgradeEffect : UpgradeEffect
{
    [SerializeField] private ResourceType resourceType;

    public override string GetEffectName() => resourceType.ToString();

    public override void ApplyEffect(float amount)
    {
        int currentMax = SaveSystem.LoadMaxResource(resourceType, 0);
        SaveSystem.SaveMaxResource(resourceType, currentMax + Mathf.RoundToInt(amount));
    }

    public override string EffectFormat(float amount) => $"+{amount}";
}