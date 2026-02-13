using System.Collections.Generic;
using UnityEngine;

public class ResourcePopupSpawner : MonoBehaviour
{
    [SerializeField] private ResourceEvents resourceEvents;
    [SerializeField] private PopupSettings popupSettings;
    [SerializeField] private List<PopupAnchor> anchors;

    private void OnEnable() => resourceEvents.OnResourceChanged += HandleResourceChange;
    private void OnDisable() => resourceEvents.OnResourceChanged -= HandleResourceChange;

    private void HandleResourceChange(ResourceType type, ResourceData data)
    {
        if (data.delta == 0) return;

        foreach (var anchor in anchors)
            if (anchor.type == type)
            {
                SpawnPopup(anchor.transform, data.delta, type);
                break;
            }
    }

    private void SpawnPopup(Transform parent, int amount, ResourceType type)
    {
        if (popupSettings.PopupPrefab == null) return;

        var go = Instantiate(popupSettings.PopupPrefab, parent);
        go.transform.localPosition = RandomOffset(popupSettings.SpawnRadius);
        go.transform.localRotation = RandomTurn(popupSettings.Turn);

        var colors = popupSettings.GetColorData(type);
        Color finalColor = (amount > 0) ? colors.positiveColor : colors.negativeColor;

        go.GetComponent<ResourcePopup>().Initialization(amount, finalColor);
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
    private struct PopupAnchor
    {
        public ResourceType type;
        public Transform transform;
    }
}
