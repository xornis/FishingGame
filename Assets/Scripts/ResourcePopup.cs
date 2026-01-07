using TMPro;
using UnityEngine;

public class ResourcePopup : MonoBehaviour
{
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float speed = 40f;
    [SerializeField] private float startScaleMultiplier = 1.25f;
    [SerializeField] private float endScaleDivider = 3f;

    private TextMeshProUGUI popupText;
    private float timer;
    private Vector3 direction;

    public void Initialization(int amount, Color color)
    {
        popupText = GetComponent<TextMeshProUGUI>();
        popupText.text = amount > 0 ? $"(+{amount})" : $"({amount.ToString()})";
        popupText.color = color;

        direction = amount > 0 ? Vector3.up : Vector3.down;

        transform.localScale = Vector3.one * startScaleMultiplier;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;

        transform.localPosition += direction * speed * Time.deltaTime;

        transform.localScale = Vector3.Lerp(Vector3.one * startScaleMultiplier, Vector3.one / endScaleDivider, progress);

        if (popupText != null)
        {
            Color color = popupText.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            popupText.color = color;
        }

        if (timer >= duration) Destroy(gameObject);
    }
}
