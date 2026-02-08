using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceUpgradeEffect", menuName = "Scriptable Objects/Shop/Effects/Resource")]
public class ResourceUpgradeEffect : UpgradeEffect
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int baseValue = 10;

    public override string GetEffectName() => resourceType.ToString();

    public override void ApplyEffect(int amount)
    {
        int currentMax = SaveSystem.LoadMaxResource(resourceType, baseValue);
        SaveSystem.SaveMaxResource(resourceType, currentMax + amount);
    }
}
