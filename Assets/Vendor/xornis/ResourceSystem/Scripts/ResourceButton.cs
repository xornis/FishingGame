using UnityEngine;

public class ResourceButton : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private ResourceType type;
    [SerializeField] private int amount;

    public void ExecuteChange()
    {
        if (resourceManager != null)
            resourceManager.ChangeResource(type, amount);
    }
}
