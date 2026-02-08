using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeTemplate", menuName = "Scriptable Objects/UpgradeTemplate")]
public class UpgradeTemplate : ScriptableObject
{
    [Header("Logic")]
    [SerializeField] private UpgradeEffect upgradeEffect;
    [SerializeField] private int minBasePrice;
    [SerializeField] private int maxBasePrice;
    [SerializeField] private int minBonusAmount;
    [SerializeField] private int maxBonusAmount;

    [Header("Appearance")]
    [SerializeField] private Sprite resourceIcon;

    public UpgradeOffer GenerateOffer()
    {
        int bonus = Random.Range(minBonusAmount, maxBonusAmount + 1);

        float quality = (maxBonusAmount == minBonusAmount) ? 1f :
            (float)(bonus - minBonusAmount) / (maxBonusAmount - minBonusAmount);

        int price = Mathf.RoundToInt(Mathf.Lerp(minBasePrice, maxBasePrice, quality));

        return new UpgradeOffer
        {
            effect = upgradeEffect,
            price = price,
            bonusAmount = bonus,
            icon = resourceIcon
        };
    }

    public struct UpgradeOffer
    {
        public UpgradeEffect effect;
        public int price;
        public int bonusAmount;
        public Sprite icon;
    }
}
