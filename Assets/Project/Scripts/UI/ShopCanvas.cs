using TMPro;
using UnityEngine;

public class ShopCanvas : UICanvas
{
    [SerializeField] private TextMeshProUGUI dayOverText;
    [SerializeField] private TextMeshProUGUI totalFishCapturedText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Transform shopButtonsContainer;
    [SerializeField] private GameObject shopResultPanel;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float scalePingDuration = 2f;
    [SerializeField] private float scalePingStrength = 1.1f;

    private void PlayShowAnimations()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartCoroutine(UIAnimations.Fade(canvasGroup, 0f, 1f, fadeDuration));

        if (shopResultPanel != null)
        {
            StartCoroutine(UIAnimations.ScalePulse(
                shopResultPanel.transform,
                scalePingDuration,
                scalePingStrength
            ));
        }
    }

    public void SetupShop(int dayNumber, int fishCaptured, int coinsEarned)
    {
        dayOverText.text = $"Day {dayNumber} is over...";
        totalFishCapturedText.text = $"Total Fish Captured: {fishCaptured}";
    }

    public void UpdateFishCount(int fishCount)
    {
        totalFishCapturedText.text = $"Total Fish Captured: {fishCount}";
    }

    public Transform GetShopButtonsContainer() => shopButtonsContainer;

    public override void Show()
    {
        base.Show();
        PlayShowAnimations();
    }

    public void ClearShopButtons()
    {
        foreach (Transform child in shopButtonsContainer)
            Destroy(child.gameObject);
    }
}
