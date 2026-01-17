using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PopupSettings", menuName = "Scriptable Objects/UnityUIResourceSystem/PopupSettings")]
public class PopupSettings : ScriptableObject
{
    [Header("Popup Prefab")]
    [SerializeField] private GameObject popupPrefab;

    [Header("Animation")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float speed = 40f;
    [SerializeField] private float startScaleMultiplier = 1.25f;
    [SerializeField] private float endScaleDivider = 3f;

    [Header("Randomness")]
    [SerializeField] private float spawnRadius = 30f;
    [SerializeField] private float turn = 15f;

    [Header("Colors")]
    [SerializeField] private List<ResourceColorData> resourceColorDatas;

    public GameObject PopupPrefab => popupPrefab;

    public float Duration => duration;
    public float Speed => speed;
    public float StartScaleMultiplier => startScaleMultiplier;
    public float EndScaleDivider => endScaleDivider;

    public float SpawnRadius => spawnRadius;
    public float Turn => turn;
    
    public ResourceColorData GetColorData(ResourceType type)
    {
        foreach (var colorData in resourceColorDatas)
            if (colorData.type == type)
                return colorData;
        return default;
    }

    [System.Serializable]
    public struct ResourceColorData
    {
        public ResourceType type;
        public Color positiveColor;
        public Color negativeColor;
    }
}
