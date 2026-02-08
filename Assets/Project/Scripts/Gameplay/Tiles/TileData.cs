using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/Tiles/Tile Data")]
public class TileData : ScriptableObject
{
    public GameObject prefab;
    public TileType tileType;
    public bool walkable;
    public bool fishable;
 
    public enum TileType
    {
        Ground,
        Fish,
        Rock,
        Sand,
        Quicksand,
        Campfire,
    }
}
