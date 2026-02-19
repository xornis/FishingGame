using UnityEngine;

public abstract class UICanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (canvas == null)
            canvas = GetComponent<Canvas>();
    }

    public bool IsVisible => canvasGroup.alpha > 0;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvas.enabled = true;
    }

    public virtual void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvas.enabled = false;
    }
}
