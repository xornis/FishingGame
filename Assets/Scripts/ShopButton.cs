using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("UI Elements")]
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

        resourceTypeText.text = offer.type.ToString();
        priceText.text = "$" + offer.price.ToString();
        bonusText.text = "+" + offer.bonusAmount.ToString();

        purchaseButton.onClick.AddListener(OnPurchaseClick);
    }

    private void OnPurchaseClick()
    {
        if (resourceManager.GetResourceAmount(ResourceType.Coins) >= currentOffer.price)
        {
            resourceManager.ChangeResource(ResourceType.Coins, -currentOffer.price);
            SaveSystem.SaveMaxResource(ResourceType.Coins, resourceManager.GetResourceAmount(ResourceType.Coins));

            int currentMax = SaveSystem.LoadMaxResource(currentOffer.type, 10);
            SaveSystem.SaveMaxResource(currentOffer.type, currentMax + currentOffer.bonusAmount);

            gameObject.SetActive(false);
            print($"Purchased {currentOffer.type} max is now {currentMax + currentOffer.bonusAmount}");
        }
    }
}
