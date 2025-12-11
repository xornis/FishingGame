using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction action;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        action = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        action.performed += context => OnClick();
    }

    private void OnDisable()
    {
        action.canceled -= context => OnClick();
    }

    private void OnClick()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
            print(hit.collider.name + ", " + worldPos);

        transform.position = hit.collider.transform.position;

        Vector3 cameraPos = new Vector3(transform.position.x, transform.position.y, -10);
        Camera.main.transform.position = cameraPos;
    }
}
