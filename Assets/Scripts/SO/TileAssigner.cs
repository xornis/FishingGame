using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Assigner", menuName = "Scriptable Objects/Tile Assigner")]
public class TileAssigner : ScriptableObject
{
    public TileData groundTile;
    public TileData fishTile;
    public TileData rockTile;
    public TileData sandTile;

    [Range(0f, 1f)] public float fishTileChance = 0.25f;
    [Range(0f, 1f)] public float rockChance = 0.4f;

    public TileData AssignBaseTile(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        if (IsOnEdge(coord, allCoords))
            return Random.value < fishTileChance ? fishTile : sandTile;
        return groundTile;
    }

    public TileData ApplyRock(HexCoord coord, Dictionary<HexCoord, TileInstance> tiles)
    {
        if (!EligibleForRock(coord, tiles)) return tiles[coord].data;
        if (Random.value < rockChance) return rockTile;
        return tiles[coord].data;
    }

    private bool IsOnEdge(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (!allCoords.Contains(coord.Neighbor(dir)))
                return true;
        return false;
    }

    private bool EligibleForRock(HexCoord coord, Dictionary<HexCoord, TileInstance> tilesByCoord)
    {
        if (tilesByCoord[coord].data == fishTile) return false;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (!tilesByCoord.ContainsKey(coord.Neighbor(dir)))
                return false;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (tilesByCoord.TryGetValue(coord.Neighbor(dir), out var neighborTile) && neighborTile.data == fishTile)
                return false;

        return true;
    }
}
