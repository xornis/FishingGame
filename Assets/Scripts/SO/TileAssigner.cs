using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Assigner", menuName = "Scriptable Objects/Tile Assigner")]
public class TileAssigner : ScriptableObject
{
    public TileData groundTile;
    public TileData fishingPoolTile;
    public TileData rockTile;

    [Range(0f, 1f)] public float fishingPoolChance = 0.25f;
    [Range(0f, 1f)] public float rockChance = 0.4f;

    public TileData GetTileFor(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        return IsOnEdge(coord, allCoords)
            ? (Random.value < fishingPoolChance ? fishingPoolTile : groundTile)
            : groundTile;
    }

    public TileData TryUpgradeToRock(HexCoord coord, Dictionary<HexCoord, TileData> tiles)
    {
        if (!IsEligibleForRock(coord, tiles)) return tiles[coord];
        if (Random.value < rockChance) return rockTile;
        return tiles[coord];
    }

    private bool IsOnEdge(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (!allCoords.Contains(coord.Neighbor(dir)))
                return true;
        return false;
    }

    private bool IsEligibleForRock(HexCoord coord, Dictionary<HexCoord, TileData> tilesByCoord)
    {
        if (tilesByCoord[coord] == fishingPoolTile) return false;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (!tilesByCoord.ContainsKey(coord.Neighbor(dir)))
                return false;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (tilesByCoord.TryGetValue(coord.Neighbor(dir), out var neighborTile) && neighborTile == fishingPoolTile)
                return false;

        return true;
    }
}
