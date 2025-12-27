using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private TextMeshProUGUI fishText;
    [SerializeField] private GameObject endRunPanel;
    [SerializeField] private GameObject endRunResultPanel;
    [SerializeField] private TextMeshProUGUI totalFishingAttemptsText;
    [SerializeField] private TextMeshProUGUI totalFishCapturedText;
    [SerializeField] private TextMeshProUGUI totalStepsWalkedText;

    private int localStepsLeft;
    private int localMaxSteps;

    private int localFishingAttemptsLeft;
    private int localMaxFishingAttempts;

    private int localTotalStepsWalked;
    private int localTotalFishCaptured;
    private int localTotalFishingAttempts;

    private void Start()
    {
        ToggleGameObject(endRunPanel, false);
        UpdateStepsUIText();
        UpdateFishUIText();
    }

    private void OnEnable()
    {
        gameEvents.OnRunEnded += ShowEndRunPanel;
        gameEvents.OnStepEnded += UpdateStepsUIText;
        gameEvents.OnFishingAttempted += UpdateFishUIText;

        gameEvents.OnStepsLeftChanged += (value) => { localStepsLeft = value; UpdateStepsUIText(); }; 
        gameEvents.OnMaxStepsChanged += (value) => { localMaxSteps = value; UpdateStepsUIText(); };
        gameEvents.OnFishingAttemptsLeftChanged += (value) => { localFishingAttemptsLeft = value; UpdateFishUIText(); };
        gameEvents.OnMaxFishingAttemptsChanged += (value) => { localMaxFishingAttempts = value; UpdateFishUIText(); };

        gameEvents.OnTotalStepsWalkedChanged += (value) => { localTotalStepsWalked = value; SetTotalStepsWalkedText(); };
        gameEvents.OnTotalFishCapturedChanged += (value) => { localTotalFishCaptured = value; SetTotalFishCapturedText(); };
        gameEvents.OnTotalFishingAttemptsChanged += (value) => { localTotalFishingAttempts = value; SetTotalFishingAttemptsText(); };
    }

    private void OnDisable()
    {
        gameEvents.OnRunEnded -= ShowEndRunPanel;
        gameEvents.OnStepEnded -= UpdateStepsUIText;
        gameEvents.OnFishingAttempted -= UpdateFishUIText;

        gameEvents.OnStepsLeftChanged -= (value) => { localStepsLeft = value; UpdateStepsUIText(); };
        gameEvents.OnMaxStepsChanged -= (value) => { localMaxSteps = value; UpdateStepsUIText(); };
        gameEvents.OnFishingAttemptsLeftChanged -= (value) => { localFishingAttemptsLeft = value; UpdateFishUIText(); };
        gameEvents.OnMaxFishingAttemptsChanged -= (value) => { localMaxFishingAttempts = value; UpdateFishUIText(); };

        gameEvents.OnTotalStepsWalkedChanged -= (value) => { localTotalStepsWalked = value; SetTotalStepsWalkedText(); };
        gameEvents.OnTotalFishCapturedChanged -= (value) => { localTotalFishCaptured = value; SetTotalFishCapturedText(); };
        gameEvents.OnTotalFishingAttemptsChanged -= (value) => { localTotalFishingAttempts = value; SetTotalFishingAttemptsText(); };
    }

    private void ShowEndRunPanel()
    {
        TurnOnEndRunPanel();
        SetResultsText();
    }

    private void UpdateStepsUIText(TileInstance _ = null) => stepsText.text = $"{localStepsLeft}/{localMaxSteps}";
    private void UpdateFishUIText() => fishText.text = $"{localFishingAttemptsLeft}/{localMaxFishingAttempts}";

    private void SetTotalStepsWalkedText() => totalStepsWalkedText.text = $"Total Steps Walked: {localTotalStepsWalked}";
    private void SetTotalFishCapturedText() => totalFishCapturedText.text = $"Total Fish Captured: {localTotalFishCaptured}";
    private void SetTotalFishingAttemptsText() => totalFishingAttemptsText.text = $"Total Fishing Attempts: {localTotalFishingAttempts}";

    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);

    private void TurnOnEndRunPanel()
    {
        ToggleGameObject(endRunPanel, true);

        StartCoroutine(FadeAnimation(endRunPanel, 2f));
        StartCoroutine(ScalePingAnimation(endRunResultPanel.transform, 2f, 1.1f));
    }
    private void SetResultsText()
    {
        SetTotalStepsWalkedText();
        SetTotalFishCapturedText();
        SetTotalFishingAttemptsText();
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
