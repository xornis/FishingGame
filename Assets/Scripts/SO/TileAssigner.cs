using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Assigner", menuName = "Scriptable Objects/Tile Assigner")]
public class TileAssigner : ScriptableObject
{
    public TileData groundTile;
    public TileData fishingPoolTile;

    public TileData GetTileFor(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        foreach (HexDirection dir in HexDirectionExtensions.hexDirections)
        {
            var chance = Random.value > 0.95f;
            var neighbor = coord.Neighbor(dir);
            if (!allCoords.Contains(neighbor) && chance)
                return fishingPoolTile;
        }
        return groundTile;
    }
}
