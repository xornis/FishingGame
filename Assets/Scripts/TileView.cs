using System.Collections;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float clickScaleMultiplier = 1.25f;
    [SerializeField] private float hoverScaleMultiplier = 1.15f;
    [SerializeField] private float hoverScaleDuration = 0.1f;

    protected SpriteRenderer sr;
    private Coroutine scaleCoroutine;
    private Vector3 baseScale;
    private Color baseColor;
    private int baseSortingOrder;

    private bool initialized;
    private bool pulsing;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        baseSortingOrder = sr.sortingOrder;
    }

    private void Start()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (initialized && baseScale != Vector3.zero) return;
        baseScale = transform.localScale;
        if (baseScale == Vector3.zero) baseScale = Vector3.one;
        initialized = true;
    }

    public void SetHover(bool isHovered)
    {
        if (pulsing) return;

        EnsureInitialized();

        sr.sortingOrder = isHovered ? baseSortingOrder + 10 : baseSortingOrder;

        Vector3 target = isHovered ? baseScale * hoverScaleMultiplier : baseScale;
        StartScaleAnimation(target, hoverScaleDuration);
    }

    public void PlayPulse(float strength, float duration)
    {
        EnsureInitialized();
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(PulseRoutine(strength, duration));
    }

    private IEnumerator PulseRoutine(float strength, float duration)
    {
        pulsing = true;
        sr.sortingOrder = baseSortingOrder + 15;

        Vector3 target = baseScale * strength;
        yield return StartCoroutine(ScaleAnimate(target, duration / 2f));
        yield return StartCoroutine(ScaleAnimate(baseScale, duration / 2f));

        sr.sortingOrder = baseSortingOrder;
        pulsing = false;
    }

    public void SetClickPulse()
    {
        if (baseScale == Vector3.zero) baseScale = transform.localScale;
        StartScaleAnimation(baseScale * clickScaleMultiplier, hoverScaleDuration);
    }

    private void StartScaleAnimation(Vector3 target, float duration)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleAnimate(target, duration));
    }

    private IEnumerator ScaleAnimate(Vector3 targetScale, float duration)
    {
        float time = 0f;
        Vector3 startScale = transform.localScale;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    public void HighlightTiles(bool value)
    {
        sr.color = value
            ? Color.Lerp(baseColor, Color.black, 0.2f)
            : baseColor;
    }
}
