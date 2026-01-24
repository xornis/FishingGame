using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class RunSummaryUI : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private ResourceEvents resourceEvents;

    [Header("References")]
    [SerializeField] private RunController runController;
    [SerializeField] private ResourceManager resourceManager;

    [Header("Settings")]
    [SerializeField] private GameObject endRunPanel;
    [SerializeField] private GameObject endRunResultPanel;
    [SerializeField] private TextMeshProUGUI totalFishCapturedText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;

    [Header("Shop Settings")]
    [SerializeField] private List<UpgradeTemplate> availableTemplates;
    [SerializeField] private GameObject shopButtonPrefab;
    [SerializeField] private Transform shopButtonsContainer;

    private void Start()
    {
        ToggleGameObject(endRunPanel, false);
        
        UpdateTotalFishCapturedText(0);
    }

    private void OnEnable()
    {
        gameEvents.OnRunEnded += TurnOnEndRunPanel;

        gameEvents.OnTotalFishCapturedChanged += UpdateTotalFishCapturedText;

        resourceEvents.OnResourceChanged += HandleResourceUpdate;
    }

    private void OnDisable()
    {
        gameEvents.OnRunEnded -= TurnOnEndRunPanel;

        gameEvents.OnTotalFishCapturedChanged -= UpdateTotalFishCapturedText;

        resourceEvents.OnResourceChanged -= HandleResourceUpdate;
    }

    private void HandleResourceUpdate(ResourceType type, ResourceData data)
    {
        if (type == ResourceType.Coins) totalCoinsText.text = "$" + data.current.ToString();
    }

    private void UpdateTotalFishCapturedText(int value) => totalFishCapturedText.text = $"Total Fish Captured: {value}";

    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);

    private void TurnOnEndRunPanel()
    {
        ToggleGameObject(endRunPanel, true);
        InitializeCoins();
        GenerateShopButtons();

        StartCoroutine(FadeAnimation(endRunPanel, 2f));
        StartCoroutine(ScalePingAnimation(endRunResultPanel.transform, 2f, 1.1f));
    }

    private void InitializeCoins()
    {
        int earnedCoins = runController.TotalFishCaptured * 10;
        resourceManager.ChangeResource(ResourceType.Coins, earnedCoins);
        int totalCoins = resourceManager.GetResourceAmount(ResourceType.Coins);
        SaveSystem.SaveMaxResource(ResourceType.Coins, totalCoins);
        totalCoinsText.text = "$" + totalCoins.ToString();
    }

    private void GenerateShopButtons()
    {
        foreach (Transform child in shopButtonsContainer) Destroy(child.gameObject);

        List<UpgradeTemplate> selected = GetRandomTemplates(availableTemplates, 6);

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
    
    private IEnumerator ScalePingAnimation(Transform targetTransform, float duration, float animationStrength)
    {
        float timer = 0f;
        Vector3 originalScale = targetTransform.localScale;
        Vector3 targetScale = originalScale * animationStrength;
        targetTransform.localScale = originalScale;

        while (timer < duration)
        {
            float t = Mathf.PingPong(timer / duration * 2f, 1f);
            targetTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            timer += Time.deltaTime;
            yield return null;
        }
        targetTransform.localScale = originalScale;
    }
    private IEnumerator FadeAnimation(GameObject gameObject, float duration)
    {
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        float timer = 0f;

        float startAlpha = 0f;
        float endAlpha = 1f;

        while (timer < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);

            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}
