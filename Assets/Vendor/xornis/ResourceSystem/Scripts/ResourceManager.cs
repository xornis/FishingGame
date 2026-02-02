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
            int savedMax = SaveSystem.LoadMaxResource(resource.type, resource.maxValue);
            resources[resource.type] = new(savedMax, savedMax, 0);
        }
    }
    private void Start()
    {
        foreach (var resource in resources)
            resourceEvents.CallResourceChanged(resource.Key, resource.Value);
    }

    public void ChangeResource(ResourceType type, int amount)
    {
        if (resources.TryGetValue(type, out ResourceData data))
        {
            data.current += amount;
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
        public int maxValue;
    }
}
