using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Generation Rules", menuName = "Scriptable Objects/Tiles/Tile Generation Rules")]
public class TileGenerationRules : ScriptableObject
{
    public TileData groundTile;
    public TileData fishTile;
    public TileData rockTile;
    public TileData sandTile;
    public TileData quicksandTile;
    public TileData campfireTile;

    [Range(0f, 1f)] public float fishTileChanceInOuterLayer = 0.25f;
    [Range(0f, 1f)] public float sandTileChanceInInnerLayer = 0.5f;
    [Range(0f, 1f)] public float quicksandTileChance = 0.2f;
    [Range(0f, 1f)] public float rockChance = 0.4f;
    [Range(0f, 1f)] public float campfireChance = 1f;

    private bool campfireSpawned;

    public TileData GetBaseTile(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        if (IsOuterEdgeLayer(coord, allCoords))
            return GetOuterEdgeTile(coord);

        if (IsInnerEdgeLayer(coord, allCoords))
            return GetInnerEdgeTile(coord);

        return groundTile;
    }

    private TileData GetOuterEdgeTile(HexCoord coord) => Random.value < fishTileChanceInOuterLayer ? fishTile : GetSandTile(coord);
    private TileData GetInnerEdgeTile(HexCoord coord) => Random.value < sandTileChanceInInnerLayer ? GetSandTile(coord) : groundTile;

    private TileData GetSandTile(HexCoord coord) => Random.value < quicksandTileChance ? quicksandTile : sandTile;

    public TileData ApplyCampfire(HexCoord coord, Dictionary<HexCoord, Tile> tiles)
    {
        if (Random.value > campfireChance || campfireSpawned) 
            return tiles[coord].data;
        
        if (tiles[coord].data.tileType == TileData.TileType.Ground && EligibleForCampfire(coord, tiles))
        {
            campfireSpawned = true;
            return campfireTile;
        }

        return tiles[coord].data;
    }

    public TileData ApplyRock(HexCoord coord, Dictionary<HexCoord, Tile> tiles)
    {
        if (!EligibleForRock(coord, tiles)) return tiles[coord].data;
        if (Random.value < rockChance) return rockTile;
        return tiles[coord].data;
    }

    private bool IsOuterEdgeLayer(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (!allCoords.Contains(coord.Neighbor(dir)))
                return true;
        return false;
    }

    private bool IsInnerEdgeLayer(HexCoord coord, HashSet<HexCoord> allCoords)
    {
        foreach (var dir in HexDirectionExtensions.hexDirections)
        {
            var neighbor = coord.Neighbor(dir);

            if (allCoords.Contains(neighbor) && IsOuterEdgeLayer(neighbor, allCoords))
                return true;
        }

        return false;
    }

    private bool EligibleForRock(HexCoord coord, Dictionary<HexCoord, Tile> tilesByCoord)
    {
        if (tilesByCoord[coord].data.tileType == TileData.TileType.Fish
            || tilesByCoord[coord].data.tileType == TileData.TileType.Sand) return false;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (!tilesByCoord.ContainsKey(coord.Neighbor(dir)))
                return false;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (tilesByCoord.TryGetValue(coord.Neighbor(dir), out var neighborTile) && neighborTile.data == fishTile)
                return false;

        return true;
    }

    private bool EligibleForCampfire(HexCoord coord, Dictionary<HexCoord, Tile> tilesByCoord)
    {
        if (tilesByCoord[coord].data.tileType != TileData.TileType.Ground) return false;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (tilesByCoord.TryGetValue(coord.Neighbor(dir), out var neighbor))
                if (neighbor.data.tileType == TileData.TileType.Rock)
                    return true;

        return false;
    }

    public void ResetGeneration() => campfireSpawned = false;
}
