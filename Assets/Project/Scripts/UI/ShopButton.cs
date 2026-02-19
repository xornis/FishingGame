using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private Button purchaseButton;

    [Header("Upgrade Type")]
    [SerializeField] private Image upgradeTypeIcon;
    [SerializeField] private TextMeshProUGUI upgradeTypeText;

    [Header("Upgrade Data")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI bonusText;

    private UpgradeTemplate.UpgradeOffer currentOffer;
    private ResourceManager resourceManager;

    private void Awake()
    {
        if (purchaseButton == null)
            purchaseButton = GetComponent<Button>();
    }

    public void Initialize(UpgradeTemplate.UpgradeOffer offer, ResourceManager resourceManager)
    {
        currentOffer = offer;
        this.resourceManager = resourceManager;

        UpgradeTypeInitialize(offer);

        priceText.text = "$" + offer.price.ToString();
        bonusText.text = offer.effect.EffectFormat(offer.bonusAmount);

        purchaseButton.onClick.AddListener(OnPurchaseClick);
    }

    private void OnPurchaseClick()
    {
        if (resourceManager.GetResourceAmount(ResourceType.Coins) < currentOffer.price) return;

        resourceManager.ChangeResource(ResourceType.Coins, -currentOffer.price);
        SaveSystem.SaveCurrentResource(ResourceType.Coins, resourceManager.GetResourceAmount(ResourceType.Coins));

        currentOffer.effect.ApplyEffect(currentOffer.bonusAmount);

        string id = currentOffer.effect.GetEffectName();
        SaveSystem.SaveUpgradeLevel(id, currentOffer.upgradeLevel + 1);

        gameObject.SetActive(false);
    }

    private void UpgradeTypeInitialize(UpgradeTemplate.UpgradeOffer offer)
    {
        if (offer.icon != null)
        {
            upgradeTypeIcon.sprite = offer.icon;
            upgradeTypeIcon.gameObject.SetActive(true);
            upgradeTypeText.gameObject.SetActive(false);
        }
        else
        {
            upgradeTypeText.text = offer.effect.GetEffectName();
            upgradeTypeText.gameObject.SetActive(true);
            upgradeTypeIcon.gameObject.SetActive(false);
        }
    }
}
