using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private ResourceEvents resourceEvents;
    [SerializeField] private List<ResourceSetup> startingResources;

    private Dictionary<ResourceType, ResourceData> resources = new();

    private void Awake()
    {
        foreach (var resource in startingResources)
        {
            if (resource.hasCap)
            {
                int max = SaveSystem.LoadMaxResource(resource.type, resource.initialValue);
                SaveSystem.SaveMaxResource(resource.type, max);
                resources[resource.type] = new(max, max, 0);
            }
            else
            {
                int current = SaveSystem.LoadCurrentResource(resource.type, resource.initialValue);
                resources[resource.type] = new(current, 0, 0);
            }
        }
    }
    private void Start()
    {
        foreach (var resource in resources)
            resourceEvents.CallResourceChanged(resource.Key, resource.Value);
    }

    public void ChangeResource(ResourceType resourceType, int amount)
    {
        ApplyChange(resourceType, amount, clampToMax: true); // hard clamp or hard cap
    }

    public void AddBonus(ResourceType resourceType, int amount)
    {
        ApplyChange(resourceType, amount, clampToMax: false); // overbuff
    }

    private void ApplyChange(ResourceType type, int amount, bool clampToMax)
    {
        if (resources.TryGetValue(type, out ResourceData data))
        {
            data.current += amount;
            data.current = clampToMax && data.max > 0
                ? Mathf.Clamp(data.current, 0, data.max)
                : Mathf.Max(data.current, 0);
            data.delta = amount;
            resources[type] = data;

            resourceEvents.CallResourceChanged(type, data);
        }
    }
    
    public int GetResourceAmount(ResourceType type)
    {
        if (resources.TryGetValue(type, out ResourceData data)) return data.current;
        return 0;
    }

    [System.Serializable]
    private struct ResourceSetup
    {
        public ResourceType type;
        public int initialValue;
        public bool hasCap;
    }
}
