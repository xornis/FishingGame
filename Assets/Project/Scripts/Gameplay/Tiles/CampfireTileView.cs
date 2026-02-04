using UnityEngine;

public class CampfireTileView : TileView
{
    [SerializeField] private Sprite usedSprite;
    private Tile parentTile;

    public void Initialize(Tile tile, GameEvents gameEvents)
    {
        parentTile = tile;
        gameEvents.OnPlayerMoved += HandlePlayerMoved;
    }

    private void HandlePlayerMoved(Tile steppedTile)
    {
        if (steppedTile == parentTile) SetUsedVisuals();
    }

    public void SetUsedVisuals()
    {
        if (parentTile.state is CampfireTileState { isUsed: true })
            sr.sprite = usedSprite;
    }
}
