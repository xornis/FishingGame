using UnityEngine;

[CreateAssetMenu(fileName = "FishableTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Fishable Tile Data")]
public class FishableTileData : TileData
{
    public enum FishTileQuality { Poor, Normal, Rich };
}
