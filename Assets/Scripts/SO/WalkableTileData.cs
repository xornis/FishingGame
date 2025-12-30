using UnityEngine;

[CreateAssetMenu(fileName = "WalkableTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Walkable Tile Data")]
public class WalkableTileData : TileData
{
    public int stepCost;
    [Range(0f, 3f)] public float moveDurationScale;
}
