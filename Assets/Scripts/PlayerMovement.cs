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
    private bool isMoving;

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
        if (!CanMove || isMoving) return;

        if (TryGetClickedNeighbor(out Tile tile))
        {
            gameEvents.CallStepEnded(tile);

            StartCoroutine(MoveTo(tile));
        }
    }

    private bool TryGetClickedNeighbor(out Tile tile)
    {
        tile = default;

        var layout = manager.Layout;

        Vector3 screenMousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, -Camera.main.transform.position.z));

        HexCoord currentPos = layout.WorldToHex(transform.position);
        HexCoord clickedPos = layout.WorldToHex(worldMousePos);

        if (currentPos.Distance(clickedPos) != 1) return false;
        if (!manager.tileByCoord.TryGetValue(clickedPos, out var clickedTile)) return false;
        if (!clickedTile.data.walkable) return false;

        tile = clickedTile;
        return true;
    }

    private IEnumerator MoveTo(Tile tile)
    {
        isMoving = true;
        
        yield return AnimateMoveTo(tile);

        gameEvents.CallPlayerMoved(tile);
        UpdateAvailableMoves(); 

        isMoving = false;
    }

    private IEnumerator AnimateMoveTo(Tile tile)
    {
        Vector3 start = transform.position;
        Vector3 end = manager.Layout.HexToWorld(tile.coord);
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

    private void SetMovementPermission(bool state) => CanMove = state;
}
