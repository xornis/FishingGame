using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RunSummaryUI : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    [Header("References")]
    [SerializeField] private DayController dayController;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private ShopManager shopManager;

    [Header("Settings")]
    [SerializeField] private GameObject endRunPanel;
    [SerializeField] private GameObject endRunResultPanel;
    [SerializeField] private TextMeshProUGUI totalFishCapturedText;
    [SerializeField] private TextMeshProUGUI dayOverText;

    private void Start()
    {
        ToggleGameObject(endRunPanel, false);
        UpdateTotalFishCapturedText(0);
    }

    private void OnEnable()
    {
        gameEvents.OnDayEnded += HandleDayEnded;

        gameEvents.OnTotalFishCapturedChanged += UpdateTotalFishCapturedText;
    }

    private void OnDisable()
    {
        gameEvents.OnDayEnded -= HandleDayEnded;

        gameEvents.OnTotalFishCapturedChanged -= UpdateTotalFishCapturedText;
    }

    private void HandleDayEnded()
    {
        TurnOnEndDayPanel();
        UpdateDayOverText();

        InitializeCoins();
        shopManager.GenerateShopButtons(resourceManager);
    }

    private void UpdateTotalFishCapturedText(int value) => totalFishCapturedText.text = $"Total Fish Captured: {value}";

    private void UpdateDayOverText() => dayOverText.text = $"Day {SaveSystem.LoadDay() - 1} is over...";

    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);

    private void TurnOnEndDayPanel()
    {
        ToggleGameObject(endRunPanel, true);

        StartCoroutine(FadeAnimation(endRunPanel, 2f));
        StartCoroutine(ScalePingAnimation(endRunResultPanel.transform, 2f, 1.1f));
    }

    private void InitializeCoins()
    {
        int earnedCoins = dayController.TotalFishCaptured * 10;
        resourceManager.ChangeResource(ResourceType.Coins, earnedCoins);
        int totalCoins = resourceManager.GetResourceAmount(ResourceType.Coins);
        SaveSystem.SaveMaxResource(ResourceType.Coins, totalCoins);
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
