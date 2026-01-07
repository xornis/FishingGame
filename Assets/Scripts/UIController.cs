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

    [Header("Popup Settings")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Color positiveColor = new Color(0.35f, 1f, 0.3f);
    [SerializeField] private Color negativeLight = new Color(1f, 0.4f, 0.4f);
    [SerializeField] private Color negativeDark = new Color(0.5f, 0f, 0f);

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

        gameEvents.OnStepsChanged += UpdateStepsUIText;
        gameEvents.OnFishingAttemptsChanged += UpdateFishUIText;

        gameEvents.OnTotalStepsWalkedChanged += UpdateTotalStepsWalkedText;
        gameEvents.OnTotalFishCapturedChanged += UpdateTotalFishCapturedText;
        gameEvents.OnTotalFishingAttemptsChanged += UpdateTotalFishingAttemptsText;
    }

    private void OnDisable()
    {
        gameEvents.OnRunEnded -= TurnOnEndRunPanel;

        gameEvents.OnStepsChanged -= UpdateStepsUIText;
        gameEvents.OnFishingAttemptsChanged -= UpdateFishUIText;

        gameEvents.OnTotalStepsWalkedChanged -= UpdateTotalStepsWalkedText;
        gameEvents.OnTotalFishCapturedChanged -= UpdateTotalFishCapturedText;
        gameEvents.OnTotalFishingAttemptsChanged -= UpdateTotalFishingAttemptsText;
    }

    private void SpawnPopup(Transform parent, int amount)
    {
        if (popupPrefab == null) return;

        GameObject go = Instantiate(popupPrefab, parent);
        go.transform.localPosition = new Vector3(-45f, 0f, 0f);

        var popup = go.GetComponent<ResourcePopup>();

        Color finalColor;
        if (amount > 0) finalColor = positiveColor;
        else
        {
            float intensity = Mathf.InverseLerp(0, 5, Mathf.Abs(amount));
            finalColor = Color.Lerp(negativeLight, negativeDark, intensity);
        }

        popup.Initialization(amount, finalColor);
    }

    private void UpdateResourceUI(TextMeshProUGUI text, ResourceData data)
    {
        text.text = $"{data.current}/{data.max}";

        if (data.delta != 0) SpawnPopup(text.transform, data.delta);
    }

    private void UpdateStepsUIText(ResourceData data) => UpdateResourceUI(stepsText, data);
    private void UpdateFishUIText(ResourceData data) => UpdateResourceUI(fishText, data);

    private void UpdateTotalStepsWalkedText(int value) => totalStepsWalkedText.text = $"Total Steps Walked: {value}";
    private void UpdateTotalFishCapturedText(int value) => totalFishCapturedText.text = $"Total Fish Captured: {value}";
    private void UpdateTotalFishingAttemptsText(int value) => totalFishingAttemptsText.text = $"Total Fishing Attempts: {value}";

    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);

    private void TurnOnEndRunPanel()
    {
        ToggleGameObject(endRunPanel, true);

        StartCoroutine(FadeAnimation(endRunPanel, 2f));
        StartCoroutine(ScalePingAnimation(endRunResultPanel.transform, 2f, 1.1f));
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
