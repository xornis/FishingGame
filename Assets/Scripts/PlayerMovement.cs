using HexDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private IslandManager manager;
    [SerializeField, Range(0f, 0.5f)] private float moveDuration = 0.25f;

    public bool CanMove { get; private set; } = true;
    private bool isMoving;
    private HexCoord? queuedStep;
    private readonly List<TileView> highlightedTiles = new List<TileView>();

    public event System.Action OnStepFinished;

    private PlayerInput playerInput;
    private InputAction action;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        action = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        manager.OnIslandReady += OnIslandReady;
        action.performed += OnClick;
    }

    private void OnDisable()
    {
        manager.OnIslandReady -= OnIslandReady;
        action.performed -= OnClick;
    }

    private void OnIslandReady()
    {
        SpawnPlayer();
        ShowAvailableMoves();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (!TryGetClickedNeighbor(out HexCoord target)) return;
        TryMove(target);
    }

    private bool TryGetClickedNeighbor(out HexCoord target)
    {
        target = default;

        var layout = manager.Layout;

        Vector3 screenMousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, -Camera.main.transform.position.z));

        HexCoord clickedPos = layout.WorldToHex(worldMousePos);
        HexCoord currentPos = layout.WorldToHex(transform.position);

        if (currentPos.Distance(clickedPos) != 1) return false;
        if (!manager.tileByCoord.TryGetValue(clickedPos, out var tile)) return false;
        if (!tile.data.walkable) return false;

        target = clickedPos;
        return true;
    }

    private void TryMove(HexCoord target)
    {
        if (!CanMove) return;

        if (isMoving) { queuedStep = target; return; }

        if (queuedStep.HasValue && queuedStep.Value.Equals(target)) return;

        StartCoroutine(MoveTo(target));
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

        AfterStep();

        if (queuedStep.HasValue)
        {
            var next = queuedStep.Value;
            queuedStep = null;
            StartCoroutine(MoveTo(next));
        }
    }

    private void ShowAvailableMoves()
    {
        HexCoord current = manager.Layout.WorldToHex(transform.position);

        foreach (var dir in HexDirectionExtensions.hexDirections)
        {
            HexCoord neighbor = current.Neighbor(dir);
            if (manager.tileByCoord.TryGetValue(neighbor, out var tile)
                && tile.data.walkable
                && tile.view != null)
            {
                tile.view.HighlightTiles(true);
                highlightedTiles.Add(tile.view);
            }
        }
    }

    private void ClearAvailableMoves()
    { 
        foreach (var tile in highlightedTiles)
            tile.HighlightTiles(false);

        highlightedTiles.Clear();
    }

    private void SpawnPlayer()
    {
        HexCoord randomCoord = manager.GetRandomGroundTileCoord();
        Vector3 worldCoordPos = manager.Layout.HexToWorld(randomCoord);
        worldCoordPos.z = transform.position.z;

        transform.position = worldCoordPos;
    }

    private void AfterStep()
    {
        OnStepFinished?.Invoke();
        ClearAvailableMoves();
        if (CanMove) ShowAvailableMoves();
    }

    public void ClearQueuedStep() => queuedStep = null;
    public void SetMovePermission(bool state) => CanMove = state;
}
