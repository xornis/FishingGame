using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image resourceTypeIcon;
    [SerializeField] private TextMeshProUGUI resourceTypeText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private Button purchaseButton;

    private UpgradeTemplate.UpgradeOffer currentOffer;
    private ResourceManager resourceManager;

    public void Initialize(UpgradeTemplate.UpgradeOffer offer, ResourceManager manager)
    {
        currentOffer = offer;
        resourceManager = manager;

        ResourceTypeInitialize(offer);

        priceText.text = "$" + offer.price.ToString();
        bonusText.text = offer.effect is ResourceUpgradeEffect 
            ? "+" + offer.bonusAmount.ToString() 
            : "+" + (offer.bonusAmount / 10f).ToString() + "%";

        purchaseButton.onClick.AddListener(OnPurchaseClick);
    }

    private void OnPurchaseClick()
    {
        if (resourceManager.GetResourceAmount(ResourceType.Coins) < currentOffer.price) return;

        resourceManager.ChangeResource(ResourceType.Coins, -currentOffer.price);
        SaveSystem.SaveMaxResource(ResourceType.Coins, resourceManager.GetResourceAmount(ResourceType.Coins));

        currentOffer.effect.ApplyEffect(currentOffer.bonusAmount);

        gameObject.SetActive(false);
    }

    private void ResourceTypeInitialize(UpgradeTemplate.UpgradeOffer offer)
    {
        if (offer.icon != null)
        {
            resourceTypeIcon.sprite = offer.icon;
            resourceTypeIcon.gameObject.SetActive(true);
            resourceTypeText.gameObject.SetActive(false);
        }
        else
        {
            resourceTypeText.text = offer.effect.GetEffectName();
            resourceTypeText.gameObject.SetActive(true);
            resourceTypeIcon.gameObject.SetActive(false);
        }
    }
}
