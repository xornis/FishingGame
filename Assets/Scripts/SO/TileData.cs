using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/Tiles/Tile Data")]
public class TileData : ScriptableObject
{
    public enum TileType
    {
        Ground,
        Fish,
        Rock,
        Sand,
        Quicksand,
    }

    public GameObject prefab;
    public TileType tileType;
    public bool walkable;
    public bool fishable;

    [Header("only if walkable is on")]
    public int stepCost;
    [Range(0f, 3f)] public float moveDurationScale;
}
