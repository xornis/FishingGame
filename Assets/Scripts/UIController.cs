using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private RunController runController;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private TextMeshProUGUI fishText;
    [SerializeField] private GameObject endRunPanel;
    [SerializeField] private GameObject endRunResultPanel;
    [SerializeField] private TextMeshProUGUI fishTriesText;
    [SerializeField] private TextMeshProUGUI fishCaughtText;
    [SerializeField] private TextMeshProUGUI stepsWalkedText;

    private void Start()
    {
        ToggleGameObject(endRunPanel, false);
        UpdateStepsText();
        UpdateFishTriesText();
    }

    private void OnEnable()
    {
        gameEvents.OnRunEnded += OnRunEnded;
        gameEvents.OnStepEnded+= UpdateStepsText;
        gameEvents.OnFishingAttempted += UpdateFishTriesText;
    }

    private void OnDisable()
    {
        gameEvents.OnRunEnded -= OnRunEnded;
        gameEvents.OnStepEnded -= UpdateStepsText;
        gameEvents.OnFishingAttempted -= UpdateFishTriesText;
    }

    private void OnRunEnded()
    {
        ToggleGameObject(endRunPanel, true);
        StartCoroutine(FadeAnimation(endRunPanel, 2f));
        StartCoroutine(ScalePingAnimation(endRunResultPanel.transform, 2f, 1.1f));
        SetStepsWalkedText();
        SetFishCaughtText();
        SetFishTriesText();
    }

    private void UpdateStepsText(TileInstance _ = null) => stepsText.text = $"{runController.StepsLeft}/{runController.maxSteps}";
    private void UpdateFishTriesText() => fishText.text = $"{runController.FishingAttemptsLeft}/{runController.maxFishingAttempts}";
    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);
    private void SetStepsWalkedText() => stepsWalkedText.text = $"Steps Walked: {runController.TotalStepsWalked}";
    private void SetFishCaughtText() => fishCaughtText.text = $"Fish Caught: {runController.TotalFishCaught}";
    private void SetFishTriesText() => fishTriesText.text = $"Fishing Tries: {runController.TotalFishingAttempts}";

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
