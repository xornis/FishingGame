using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private GameObject endRunPanel;
    [SerializeField] private TextMeshProUGUI fishCaughtText;
    [SerializeField] private TextMeshProUGUI stepsWalkedText;

    [SerializeField] private RunController runController;
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        ToggleGameObject(endRunPanel, false);
        UpdateStepsText();
    }

    private void OnEnable()
    {
        runController.OnRunEnded += OnRunEnded;
        playerMovement.OnStepFinished += UpdateStepsText;
    }

    private void OnDisable()
    {
        runController.OnRunEnded -= OnRunEnded;
        playerMovement.OnStepFinished -= UpdateStepsText;
    }

    private void OnRunEnded()
    {
        ToggleGameObject(endRunPanel, true);
        StartCoroutine(FadeAnimation(endRunPanel, 2f));
        StartCoroutine(ScalePingAnimation(endRunPanel.transform, 2f, 1.1f));
        SetStepsWalkedText();
        SetFishCaughtText();
    }

    private void UpdateStepsText() => stepsText.text = $"{runController.StepsLeft}/{runController.MaxSteps}";
    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);
    private void SetStepsWalkedText() => stepsWalkedText.text = $"Steps Walked: {runController.StepsWalked}";
    private void SetFishCaughtText() => fishCaughtText.text = $"Fish Caught: {runController.FishCaught}";

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
