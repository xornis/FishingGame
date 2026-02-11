using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeTemplate", menuName = "Scriptable Objects/UpgradeTemplate")]
public class UpgradeTemplate : ScriptableObject
{
    [SerializeField] private UpgradeEffect upgradeEffect;

    [Header("Appearance")]
    [SerializeField] private Sprite resourceIcon;
    
    [Header("Logic")]
    [SerializeField] private int bonusPerLevel = 20;
    [SerializeField] private int basePrice = 10;
    [SerializeField] private float priceGrowth = 1.5f;
    [SerializeField] private int maxLevel = 20;

    private int CurrentLevel => SaveSystem.LoadUpgradeLevel(upgradeEffect.GetEffectName());
    public bool IsMaxLevel => CurrentLevel >= maxLevel;

    public UpgradeOffer GenerateOffer()
    {
        int level = CurrentLevel;
        int price = Mathf.RoundToInt(basePrice * Mathf.Pow(priceGrowth, level));

        return new UpgradeOffer
        {
            effect = upgradeEffect,
            icon = resourceIcon,
            price = price,
            bonusAmount = bonusPerLevel,
            level = level,
            maxLevel = maxLevel,
        };
    }

    public struct UpgradeOffer
    {
        public UpgradeEffect effect;
        public Sprite icon;
        public int price;
        public int bonusAmount;
        public int level;
        public int maxLevel;
    }
}
