using HexDungeon;

public enum FishTileQuality { Poor, Normal, Rich };

public class Tile
{
    public HexCoord coord;
    public TileData data;
    public FishTileQuality fishQuality;
    public TileView view;
}
