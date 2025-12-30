using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [SerializeField] private HexRoomGenerator generator;
    [SerializeField] private TileGenerationRules tileGenerationRules;
    
    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    public HexLayout Layout { get; private set; }
    public float HexScale { get; private set; }
    public readonly Dictionary<HexCoord, Tile> tileByCoord = new();

    private void Start()
    {
        var coords = generator.GetCoords(out var layout, out var hexScale);

        Layout = layout;
        HexScale = hexScale;

        var coordSet = new HashSet<HexCoord>(coords);

        foreach (var coord in coordSet)
            tileByCoord[coord] = new Tile { coord = coord };

        foreach (var coord in coordSet)
            tileByCoord[coord].data = tileGenerationRules.GetBaseTile(coord, coordSet);

        foreach (var coord in coordSet)
            tileByCoord[coord].data = tileGenerationRules.ApplyRock(coord, tileByCoord);

        foreach (var coord in coordSet)
        {
            ComputeFishTileQuality(coord, tileByCoord[coord]);
            SpawnTile(coord, tileByCoord[coord]);
        }

        gameEvents.CallIslandReady();
    }

    private void SpawnTile(HexCoord coord, Tile tile)
    {
        var worldPos = generator.transform.TransformPoint(Layout.HexToWorld(coord));
        var go = Instantiate(tile.data.prefab, worldPos, Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * HexScale;

        tile.view = go.GetComponent<TileView>();
        if (tile.data is FishableTileData) SetFishTileColor(go.GetComponent<SpriteRenderer>(), tile);
    }

    private void ComputeFishTileQuality(HexCoord coord, Tile tile)
    {
        if (tile.data is not FishableTileData) return;

        int count = 0;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (tileByCoord.TryGetValue(coord.Neighbor(dir), out var neighborTile) && neighborTile.data.fishable)
                count++;

        FishState fishState = new FishState();

        fishState.fishQuality = count switch
        {
            0 => FishableTileData.FishTileQuality.Poor,
            1 => FishableTileData.FishTileQuality.Normal,
            _ => FishableTileData.FishTileQuality.Rich
        };

        tile.state = fishState;
    }

    private void SetFishTileColor(SpriteRenderer sr, Tile tile)
    {
        if (tile.state is FishState fishState)
        {
            sr.color = fishState.fishQuality switch
            {
                FishableTileData.FishTileQuality.Poor => new Color(0.95f, 0.95f, 0.9f),
                FishableTileData.FishTileQuality.Normal => new Color(0.9f, 0.95f, 1f),
                FishableTileData.FishTileQuality.Rich => new Color(0.8f, 0.9f, 1f),
                _ => Color.white
            };
        }
    }

    public HexCoord GetRandomGroundTileCoord()
    {
        var groundTiles = new List<HexCoord>();

        foreach (var tile in tileByCoord)
            if (tile.Value.data.tileType == TileData.TileType.Ground)
                groundTiles.Add(tile.Key);

        return groundTiles[Random.Range(0, groundTiles.Count)];
    }
}
