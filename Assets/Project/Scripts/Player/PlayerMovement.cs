using HexDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private IslandManager manager;
    [SerializeField] private StatsManager statsManager;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;
    
    private float MoveSpeed => statsManager.GetStat(StatType.MoveSpeed);

    public bool CanMove { get; private set; } = true;
    private bool isMoving;

    private readonly List<TileView> highlightedTiles = new List<TileView>();

    private void OnEnable()
    {
        gameEvents.OnIslandReady += OnIslandReady;
        gameEvents.OnSetMovementPermission += SetMovementPermission;
        gameEvents.OnTileClicked += HandleMoveRequest;
    }

    private void OnDisable()
    {
        gameEvents.OnIslandReady -= OnIslandReady;
        gameEvents.OnSetMovementPermission -= SetMovementPermission;
        gameEvents.OnTileClicked -= HandleMoveRequest;
    }

    private void HandleMoveRequest(Tile tile)
    {
        HexCoord currentPos = manager.Layout.WorldToHex(transform.position);
        bool target = currentPos.Distance(tile.coord) == 1 && tile.data.walkable;

        if (!target) return;

        if (!CanMove)
        {
            tile.view.PlayErrorEffect();
            return;
        }

        if (isMoving) return;

        tile.view.PlayPulse(0.9f, 0.15f);
        gameEvents.CallStepEnded(tile);
        StartCoroutine(MoveTo(tile));
    }

    private void OnIslandReady()
    {
        SpawnPlayer();
        ShowAvailableMoves();
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
        float durationScale = (tile.data is WalkableTileData walkableTileData) ? walkableTileData.MoveSpeedMultiplier : 1f;
        float duration = MoveSpeed * durationScale;

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

        Tile spawnTile = manager.tileByCoord[randomCoord];
        gameEvents.CallPlayerMoved(spawnTile);
    }

    private void SetMovementPermission(bool state)
    { 
        CanMove = state;
        if (!state) ClearAvailableMoves();
    }
}
