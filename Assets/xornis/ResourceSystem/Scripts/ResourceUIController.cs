using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ResourceUIController : MonoBehaviour
{
    [SerializeField] private ResourceEvents resourceEvents;
    [SerializeField] private PopupSettings popupSettings;
    [SerializeField] private List<ResourceUIElement> uIElements;

    private void OnEnable() => resourceEvents.OnResourceChanged += HandleResourceChange;
    private void OnDisable() => resourceEvents.OnResourceChanged -= HandleResourceChange;

    private void HandleResourceChange(ResourceType type, ResourceData data)
    {
        foreach (var element in uIElements)
            if (element.type == type)
            {
                UpdateResourceUI(element.text, data, type);
                break;
            }
    }

    private void UpdateResourceUI(TextMeshProUGUI text, ResourceData data, ResourceType type)
    {
        text.text = $"{data.current}/{data.max}";
        if (data.delta != 0) SpawnPopup(text.transform, data.delta, type);
    }

    private void SpawnPopup(Transform parent, int amount, ResourceType type)
    {
        if (popupSettings.PopupPrefab == null) return;

        var go = Instantiate(popupSettings.PopupPrefab, parent);
        go.transform.localPosition = RandomOffset(popupSettings.SpawnRadius);
        go.transform.localRotation = RandomTurn(popupSettings.Turn);

        var popup = go.GetComponent<ResourcePopup>();
        var colors = popupSettings.GetColorData(type);
        Color finalColor = (amount > 0) ? colors.positiveColor : colors.negativeColor;

        popup.Initialization(amount, finalColor);
    }

    private Vector3 RandomOffset(float offsetRange)
    {
        float randomOffset = Random.Range(-offsetRange, offsetRange);
        return new Vector3(randomOffset, randomOffset, 0f);
    }
    private Quaternion RandomTurn(float turnRange)
    {
        float randomTurn = Random.Range(-turnRange, turnRange);
        return Quaternion.Euler(0f, 0f, randomTurn);
    }
    
    [System.Serializable]
    private struct ResourceUIElement
    {
        public ResourceType type;
        public TextMeshProUGUI text;
    }
}
