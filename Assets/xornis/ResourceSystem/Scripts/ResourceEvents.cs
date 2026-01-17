using UnityEngine;

[CreateAssetMenu(fileName = "ResourceEvents", menuName = "Scriptable Objects/UnityUIResourceSystem/ResourceEvents")]
public class ResourceEvents : ScriptableObject
{
    public event System.Action<ResourceType, ResourceData> OnResourceChanged;

    public void CallResourceChanged(ResourceType type, ResourceData data) => OnResourceChanged?.Invoke(type, data);
}