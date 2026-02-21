using System.Collections;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float hoverScaleMultiplier = 1.15f;
    [SerializeField] private float hoverScaleDuration = 0.1f;
    [SerializeField] private float rejectedClickScaleMultiplier = 0.85f;
    [SerializeField] private float rejectedClickScaleDuration = 0.25f;

    protected SpriteRenderer sr;
    
    private Coroutine scaleCoroutine;
    private Coroutine errorCoroutine;
    private Coroutine shakeCoroutine;

    private Vector3 baseScale;
    private Color baseColor;
    private float baseAngleRotation;
    private int baseSortingOrder;

    private bool initialized;
    private bool pulsing;
    private bool shaking;

    private bool hoveredState;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        baseSortingOrder = sr.sortingOrder;
        baseAngleRotation = transform.rotation.eulerAngles.z;
    }

    private void Start() => EnsureInitialized();

    private void EnsureInitialized()
    {
        if (initialized && baseScale != Vector3.zero) return;
        baseScale = transform.localScale;
        if (baseScale == Vector3.zero) baseScale = Vector3.one;
        initialized = true;
    }

    private void PlayScaleAnimate(Vector3 target, float duration)
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

    public void SetHover(bool hovered)
    {
        hoveredState = hovered;

        if (!pulsing && errorCoroutine == null)
        {
            PlayScaleAnimate(GetTargetScale(), hoverScaleDuration);
            sr.sortingOrder = hovered ? baseSortingOrder + 10 : baseSortingOrder;
        }
    }

    private Vector3 GetTargetScale() => hoveredState ? baseScale * hoverScaleMultiplier : baseScale;

    public void PlayShake(float speed, float angle, float duration)
    {
        EnsureInitialized();
        if (shakeCoroutine != null && !shaking) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(speed, angle, duration));
    }

    private IEnumerator ShakeRoutine(float speed, float angle, float duration)
    {
        shaking = true;
        Quaternion startRotation = transform.localRotation;
        float timer = 0f;

        while (timer < duration)
        {
            float randomAngle = Random.Range(-angle, angle);
            Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, randomAngle);

            float moveTime = 0f;
            float segmentDuration = 0.1f;

            while (moveTime < segmentDuration)
            {
                moveTime += Time.deltaTime;
                timer += Time.deltaTime;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, moveTime / segmentDuration);
                yield return null;
            }
        }

        float resetTime = 0;
        while (resetTime < 1f)
        {
            resetTime += Time.deltaTime * 10f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, startRotation, resetTime);
            yield return null;
        }
        shaking = false;
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

        yield return ScaleAnimate(baseScale * strength, duration / 2f);
        yield return ScaleAnimate(GetTargetScale(), duration / 2f);

        pulsing = false;

        SetHover(hoveredState);
    }

    public void PlayErrorEffect()
    {
        EnsureInitialized();
        if (errorCoroutine != null) StopCoroutine(errorCoroutine);
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        errorCoroutine = StartCoroutine(ErrorRoutine());
    }

    private IEnumerator ErrorRoutine()
    {
        Color errorColor = Color.Lerp(Color.white, Color.red, 0.3f);
        Vector3 startScale = transform.localScale;

        float intensity = 1f - rejectedClickScaleMultiplier;
        float tiltAngle = intensity * 100f;
        float randomTilt = Random.Range(-tiltAngle, tiltAngle);

        Vector3 rejectedClickScale = baseScale * rejectedClickScaleMultiplier;

        Quaternion startRotation = Quaternion.Euler(0f, 0f, baseAngleRotation);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, baseAngleRotation + randomTilt);

        float time = 0f;
        while (time < rejectedClickScaleDuration)
        {
            float t = time / rejectedClickScaleDuration;
            float pingPong = Mathf.PingPong(t * 2, 1f);

            sr.color = Color.Lerp(baseColor, errorColor, pingPong);
            transform.localScale = Vector3.Lerp(startScale, rejectedClickScale, pingPong);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, pingPong);

            time += Time.deltaTime;
            yield return null;
        }

        sr.color = baseColor;
        transform.rotation = startRotation;
        errorCoroutine = null;

        PlayScaleAnimate(GetTargetScale(), hoverScaleDuration);
    }

    public void HighlightTiles(bool value)
    {
        if (sr != null)
            sr.color = value ? Color.Lerp(baseColor, Color.black, 0.2f) : baseColor;
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;
        sr.color = color;
    }
} 
