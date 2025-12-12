using HexDungeon;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction action;
    private HexLayout hexLayout;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        action = playerInput.actions["Attack"];
        hexLayout = new HexLayout(HexOrientation.PointyTop, 1);
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
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        HexCoord clickedPos = hexLayout.WorldToHex(worldPos);
        HexCoord currentPos = hexLayout.WorldToHex(transform.position);

        if (currentPos.Distance(clickedPos) != 1) return;

        Vector3 targetWorldPos = hexLayout.HexToWorld(clickedPos);
        targetWorldPos.z = transform.position.z;
        transform.position = targetWorldPos;

        Vector3 cameraPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = cameraPos;
    }
}
