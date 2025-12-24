using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/Tile Data")]
public class TileData : ScriptableObject
{
    public enum TileType
    {
        Ground,
        Fish,
        Rock,
        Sand
    }

    public GameObject prefab;
    public TileType tileType;
    public bool walkable;
    public bool fishable;
}
