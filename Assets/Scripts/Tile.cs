using HexDungeon;

public class Tile
{
    public HexCoord coord;
    public TileData data;
    public TileView view;

    public TileState state;
}

public abstract class TileState { }

public class FishState : TileState
{
    public FishableTileData.FishTileQuality fishQuality;
}
