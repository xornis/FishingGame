using HexDungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private IslandManager manager;
    [SerializeField, Range(0f, 0.5f)] private float moveDuration = 0.25f;

    private bool isMoving;
    private HexCoord? queuedStep;

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
        var layout = manager.Layout;
        
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        HexCoord clickedPos = layout.WorldToHex(worldPos);
        HexCoord currentPos = layout.WorldToHex(transform.position);

        if (currentPos.Distance(clickedPos) != 1) return;
        if (!manager.tileByCoord.TryGetValue(clickedPos, out var tile)) return;
        if (!tile.data.walkable) return;

        if (isMoving) { queuedStep = clickedPos; return; }
        if (queuedStep.HasValue && queuedStep.Value.Equals(clickedPos)) return;

        StartCoroutine(MoveTo(clickedPos));
    }

    private IEnumerator MoveTo(HexCoord target)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = manager.Layout.HexToWorld(target);
        end.z = start.z;

        float timer = 0f;
        float duration = moveDuration;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, timer / duration);
            yield return null;
        }

        transform.position = end;
        isMoving = false;

        if (queuedStep.HasValue)
        {
            StartCoroutine(MoveTo(queuedStep.Value));
            queuedStep = null;
        }
    }
}
