using System.Collections;
using TMPro;
using UnityEngine;

public class RunSummaryUI : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    [Header("References")]
    [SerializeField] private RunController runController;
    [SerializeField] private ResourceManager resourceManager;

    [Header("Settings")]
    [SerializeField] private GameObject endRunPanel;
    [SerializeField] private GameObject endRunResultPanel;
    [SerializeField] private TextMeshProUGUI totalFishCapturedText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;

    [SerializeField] private TextMeshProUGUI totalFishingAttemptsText;
    [SerializeField] private TextMeshProUGUI totalStepsWalkedText;

    private void Start()
    {
        ToggleGameObject(endRunPanel, false);

        UpdateTotalStepsWalkedText(0);
        UpdateTotalFishCapturedText(0);
        UpdateTotalFishingAttemptsText(0);
    }

    private void OnEnable()
    {
        gameEvents.OnRunEnded += TurnOnEndRunPanel;

        gameEvents.OnTotalStepsWalkedChanged += UpdateTotalStepsWalkedText;
        gameEvents.OnTotalFishCapturedChanged += UpdateTotalFishCapturedText;
        gameEvents.OnTotalFishingAttemptsChanged += UpdateTotalFishingAttemptsText;
    }

    private void OnDisable()
    {
        gameEvents.OnRunEnded -= TurnOnEndRunPanel;

        gameEvents.OnTotalStepsWalkedChanged -= UpdateTotalStepsWalkedText;
        gameEvents.OnTotalFishCapturedChanged -= UpdateTotalFishCapturedText;
        gameEvents.OnTotalFishingAttemptsChanged -= UpdateTotalFishingAttemptsText;
    }

    private void UpdateTotalStepsWalkedText(int value) => totalStepsWalkedText.text = $"Total Steps Walked: {value}";
    private void UpdateTotalFishCapturedText(int value) => totalFishCapturedText.text = $"Total Fish Captured: {value}";
    private void UpdateTotalFishingAttemptsText(int value) => totalFishingAttemptsText.text = $"Total Fishing Attempts: {value}";

    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);

    private void TurnOnEndRunPanel()
    {
        ToggleGameObject(endRunPanel, true);
        InitializeCoins();

        StartCoroutine(FadeAnimation(endRunPanel, 2f));
        StartCoroutine(ScalePingAnimation(endRunResultPanel.transform, 2f, 1.1f));
    }

    private void InitializeCoins()
    {
        int earnedCoins = runController.TotalFishCaptured * 10;
        resourceManager.ChangeResource(ResourceType.Coins, earnedCoins);
        int totalCoins = resourceManager.GetResourceAmount(ResourceType.Coins);
        SaveSystem.SaveMaxResource(ResourceType.Coins, totalCoins);
        totalCoinsText.text = totalCoins.ToString();
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
