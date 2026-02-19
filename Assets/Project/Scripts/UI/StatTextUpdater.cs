using TMPro;
using UnityEngine;

public class StatTextUpdater : MonoBehaviour
{
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private StatType statType;

    [Header("Display Settings")]
    [SerializeField] private string label = "stat";
    [SerializeField] private string suffix = "%";
    [SerializeField] private string format = "F2";

    private TextMeshProUGUI text;

    private void Awake() => text = GetComponent<TextMeshProUGUI>();

    private void OnEnable() => statsManager.OnStatChanged += HandleStatChanged;
    private void OnDisable() => statsManager.OnStatChanged -= HandleStatChanged;

    private void HandleStatChanged(StatType type, float value)
    {
        if (type != statType) return;

        UpdateText(value);
    }

    private void UpdateText(float value)
    {
        string formattedValue = value.ToString(format);

        if (!string.IsNullOrEmpty(label))
            text.text = label + ": " + formattedValue + suffix;
        else
            text.text = formattedValue + suffix;
    }
}
