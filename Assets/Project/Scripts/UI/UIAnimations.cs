using UnityEngine;
using System.Collections;

public static class UIAnimations
{
    public static IEnumerator Fade(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    public static IEnumerator ScalePulse(Transform target, float duration, float strength)
    {
        float timer = 0f;
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * strength;

        while (timer < duration)
        {
            float t = Mathf.PingPong(timer / duration * 2f, 1f);
            target.localScale = Vector3.Lerp(originalScale, targetScale, t);

            timer += Time.deltaTime;
            yield return null;
        }

        target.localScale = originalScale;
    }

    public static IEnumerator SmoothScale(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            target.localScale = Vector3.Lerp(startScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        target.localScale = targetScale;
    }

    public static IEnumerator Bounce(Transform target, float height, float duration)
    {
        Vector3 startPos = target.localPosition;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            float yOffset = Mathf.Sin(t * Mathf.PI) * height;

            target.localPosition = startPos + Vector3.up * yOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        target.localPosition = startPos;
    }
}
