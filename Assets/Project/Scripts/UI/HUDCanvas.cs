using System.Collections;
using TMPro;
using UnityEngine;

public class HUDCanvas : UICanvas
{
    [Header("Game Menu Hint Text")]
    [SerializeField] private TextMeshProUGUI gameMenuHintText;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color targetColor = Color.gray;
    [SerializeField] private float bounceDuration = 0.75f;

    private void Start() => StartCoroutine(TextIdleColorBounce(startColor, targetColor, bounceDuration));

    private IEnumerator TextIdleColorBounce(Color startColor, Color targetColor, float duration)
    {
        while (true)
        {
            yield return StartCoroutine(LerpColor(startColor, targetColor, duration));
            yield return StartCoroutine(LerpColor(targetColor, startColor, duration));
        }
    }

    private IEnumerator LerpColor(Color startColor, Color targetColor, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            gameMenuHintText.color = Color.Lerp(startColor, targetColor, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        gameMenuHintText.color = startColor;
    }

    public void CloseGameMenuHint()
    {
        if (gameMenuHintText.IsActive()) gameMenuHintText.gameObject.SetActive(false);
    }
}
