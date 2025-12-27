using HexDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private IslandManager manager;
    [SerializeField, Range(0f, 0.5f)] private float baseMoveDuration = 0.25f;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    public bool CanMove { get; private set; } = true;
    private readonly List<TileView> highlightedTiles = new List<TileView>();

    private PlayerInput playerInput;
    private InputAction action;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        action = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        gameEvents.OnIslandReady += OnIslandReady;
        action.performed += OnClick;

        gameEvents.OnSetMovementPermission += SetMovementPermission;
    }

    private void OnDisable()
    {
        gameEvents.OnIslandReady -= OnIslandReady;
        action.performed -= OnClick;

        gameEvents.OnSetMovementPermission -= SetMovementPermission;
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

        StartCoroutine(MoveTo(target));
    }

    private IEnumerator MoveTo(HexCoord target)
    {
        var tile = manager.tileByCoord[target];

        StartMove();
        yield return AnimateMoveTo(target, tile);
        EndMove(tile);
    }

    private void StartMove() => CanMove = false;
    private void EndMove(TileInstance tile) { CanMove = true; AfterStep(tile); }

    private IEnumerator AnimateMoveTo(HexCoord target, TileInstance tile)
    {
        Vector3 start = transform.position;
        Vector3 end = manager.Layout.HexToWorld(target);
        end.z = start.z;

        float timer = 0f;
        float duration = baseMoveDuration * tile.data.moveDurationScale;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, timer / duration);
            yield return null;
        }

        transform.position = end;
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

    private void UpdateAvailableMoves()
    {
        ClearAvailableMoves();
        if (CanMove) ShowAvailableMoves();
    }

    private void SpawnPlayer()
    {
        HexCoord randomCoord = manager.GetRandomGroundTileCoord();
        Vector3 worldCoordPos = manager.Layout.HexToWorld(randomCoord);
        worldCoordPos.z = transform.position.z;

        transform.position = worldCoordPos;
    }

    private void AfterStep(TileInstance tile)
    {
        gameEvents.CallStepEnded(tile);
        gameEvents.CallPlayerMoved(tile.coord);
        UpdateAvailableMoves();
    }

    private void SetMovementPermission(bool state) => CanMove = state;
}
