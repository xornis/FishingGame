using UnityEngine;

public class SimpleResourceRegenerator : MonoBehaviour
{
    [SerializeField] private ResourceManager manager;
    [SerializeField] private ResourceType type;

    private void Start() => InvokeRepeating(nameof(RegenerateResource), 1f, 1f);

    private void RegenerateResource() => manager.ChangeResource(type, 1);
}
