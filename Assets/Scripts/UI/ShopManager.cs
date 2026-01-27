using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<UpgradeTemplate> availableTemplates;
    [SerializeField] private GameObject shopButtonPrefab;
    [SerializeField] private Transform shopButtonsContainer;
    [SerializeField] private int initialCardsToSpawnNumber = 2;

    public void GenerateShopButtons(ResourceManager resourceManager)
    {
        foreach (Transform child in shopButtonsContainer) Destroy(child.gameObject);

        int day = SaveSystem.LoadDay();

        int cardsToSpawn = Mathf.Clamp(initialCardsToSpawnNumber + (day - 1) / 3, initialCardsToSpawnNumber, 6);

        List<UpgradeTemplate> selected = GetRandomTemplates(availableTemplates, cardsToSpawn);

        foreach (var template in selected)
        {
            GameObject button = Instantiate(shopButtonPrefab, shopButtonsContainer);
            ShopButton shopButton = button.GetComponent<ShopButton>();

            shopButton.Initialize(template.GenerateOffer(), resourceManager);
        }
    }

    private List<UpgradeTemplate> GetRandomTemplates(List<UpgradeTemplate> upgradeTemplates, int count)
    {
        List<UpgradeTemplate> result = new();
        List<UpgradeTemplate> copy = new(upgradeTemplates);

        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, copy.Count);
            result.Add(copy[randomIndex]);
        }

        return result;
    }
}
