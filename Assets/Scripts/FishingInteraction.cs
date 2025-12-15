using HexDungeon;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingInteraction : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float catchChance = 0.25f;

    [SerializeField] private IslandManager manager;

    private PlayerInput playerInput;
    private InputAction action;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        action = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        action.performed += OnClick;
    }

    private void OnDisable()
    {
        action.performed -= OnClick;
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        TryCatch();
    }

    private void TryCatch()
    {
        var layout = manager.Layout;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        HexCoord clickedPos = layout.WorldToHex(worldPos);
        HexCoord currentPos = layout.WorldToHex(transform.position);

        if (currentPos.Distance(clickedPos) != 1) return;
        if (!manager.tileByCoord.TryGetValue(clickedPos, out var tile)) return;

        if (tile.fishable)
            Debug.Log((Random.value < catchChance) ? "Caught!" : "Got Away..");
    }
}
