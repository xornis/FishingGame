using HexDungeon;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private IslandManager shaper;

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
        var layout = shaper.Layout;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        HexCoord clickedPos = layout.WorldToHex(worldPos);
        HexCoord currentPos = layout.WorldToHex(transform.position);

        if (currentPos.Distance(clickedPos) != 1) return;
        if (!shaper.tiles.Contains(clickedPos)) return;

        Vector3 targetWorldPos = shaper.Layout.HexToWorld(clickedPos);
        targetWorldPos.z = transform.position.z;
        transform.position = targetWorldPos;

        Vector3 cameraPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = cameraPos;
    }
}
