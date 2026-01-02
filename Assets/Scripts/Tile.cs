using HexDungeon;

public class Tile
{
    public HexCoord coord;
    public TileData data;
    public TileView view;

    public TileState state;
}

public abstract class TileState { }

public class FishTileState : TileState
{
    public FishableTileData.FishTileQuality fishQuality;
}

public class CampfireTileState : TileState
{
    public bool isUsed = false;
}
