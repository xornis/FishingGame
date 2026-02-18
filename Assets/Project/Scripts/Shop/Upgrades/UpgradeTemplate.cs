using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeTemplate", menuName = "Scriptable Objects/UpgradeTemplate")]
public class UpgradeTemplate : ScriptableObject
{
    [SerializeField] private UpgradeEffect upgradeEffect;

    [Header("Appearance")]
    [SerializeField] private Sprite resourceIcon;
    
    [Header("Logic")]
    [SerializeField] private int bonusPerUpgradeLevel = 20;
    [SerializeField] private int basePrice = 10;
    [SerializeField] private float priceGrowth = 1.5f;

    private int CurrentUpgradeLevel => SaveSystem.LoadUpgradeLevel(upgradeEffect.GetEffectName());

    private void OnValidate()
    {
        if (upgradeEffect == null)
        {
            Debug.LogWarning($"[{name}] UpgradeEffect is not assigned!");
        }
    }

    public UpgradeOffer GenerateOffer()
    {
        if (upgradeEffect == null)
        {
            Debug.LogError($"[{name}] Cannot generate offer: UpgradeEffect is null!");
            return default;
        }

        int upgradeLevel = CurrentUpgradeLevel;
        int price = Mathf.RoundToInt(basePrice * Mathf.Pow(priceGrowth, upgradeLevel));

        return new UpgradeOffer
        {
            effect = upgradeEffect,
            icon = resourceIcon,
            price = price,
            bonusAmount = bonusPerUpgradeLevel,
            upgradeLevel = upgradeLevel,
        };
    }

    public struct UpgradeOffer
    {
        public UpgradeEffect effect;
        public Sprite icon;
        public int price;
        public int bonusAmount;
        public int upgradeLevel;
    }
}
