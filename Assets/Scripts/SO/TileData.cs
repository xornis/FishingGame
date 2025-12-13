using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/Tile Data")]
public class TileData : ScriptableObject
{
    public GameObject prefab;
    public bool walkable;
    public bool fishable;
}
