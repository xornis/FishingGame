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
    [SerializeField] private int maxUpgradeLevel = 20;

    private int CurrentUpgradeLevel => SaveSystem.LoadUpgradeLevel(upgradeEffect.GetEffectName());
    public bool IsMaxUpgradeLevel => CurrentUpgradeLevel >= maxUpgradeLevel;

    public UpgradeOffer GenerateOffer()
    {
        int upgradeLevel = CurrentUpgradeLevel;
        int price = Mathf.RoundToInt(basePrice * Mathf.Pow(priceGrowth, upgradeLevel));

        return new UpgradeOffer
        {
            effect = upgradeEffect,
            icon = resourceIcon,
            price = price,
            bonusAmount = bonusPerUpgradeLevel,
            upgradeLevel = upgradeLevel,
            maxUpgradeLevel = maxUpgradeLevel,
        };
    }

    public struct UpgradeOffer
    {
        public UpgradeEffect effect;
        public Sprite icon;
        public int price;
        public int bonusAmount;
        public int upgradeLevel;
        public int maxUpgradeLevel;
    }
}
