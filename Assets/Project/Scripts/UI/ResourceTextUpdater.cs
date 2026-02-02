using TMPro;
using UnityEngine;

public class ResourceTextUpdater : MonoBehaviour
{
    [SerializeField] private ResourceEvents resourceEvents;
    [SerializeField] private ResourceType resourceType;

    [Header("Format Settings")]
    [SerializeField] private string prefix = "";
    [SerializeField] private bool showMax;

    private TextMeshProUGUI text;

    private void Awake() => text = GetComponent<TextMeshProUGUI>();

    private void OnEnable() => resourceEvents.OnResourceChanged += UpdateResourceText;
    private void OnDisable() => resourceEvents.OnResourceChanged -= UpdateResourceText;

    private void UpdateResourceText(ResourceType type, ResourceData data)
    {
        if (type != resourceType) return;

        if (showMax) text.text = prefix + data.current + "/" + data.max;
        else text.text = prefix + data.current;
    }
}
