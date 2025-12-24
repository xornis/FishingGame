using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [SerializeField] private HexRoomGenerator generator;
    [SerializeField] private TileAssigner tileAssigner;

    public event System.Action OnIslandReady;

    public HexLayout Layout { get; private set; }
    public float HexScale { get; private set; }
    public readonly Dictionary<HexCoord, TileInstance> tileByCoord = new();

    private void Start()
    {
        var coords = generator.GetCoords(out var layout, out var hexScale);

        Layout = layout;
        HexScale = hexScale;

        var coordSet = new HashSet<HexCoord>(coords);

        foreach (var coord in coordSet)
            tileByCoord[coord] = new TileInstance { coord = coord };

        foreach (var coord in coordSet)
            tileByCoord[coord].data = tileAssigner.AssignBaseTile(coord, coordSet);

        foreach (var coord in coordSet)
            tileByCoord[coord].data = tileAssigner.ApplyRock(coord, tileByCoord);

        foreach (var coord in coordSet)
        {
            ComputeFishTileQuality(coord, tileByCoord[coord]);
            SpawnTile(coord, tileByCoord[coord]);
        }

        OnIslandReady?.Invoke();
    }

    private void SpawnTile(HexCoord coord, TileInstance tile)
    {
        var worldPos = generator.transform.TransformPoint(Layout.HexToWorld(coord));
        var go = Instantiate(tile.data.prefab, worldPos, Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * HexScale;

        tile.view = go.GetComponent<TileView>();
        if (tile.data.fishable) SetFishTileColor(go.GetComponent<SpriteRenderer>(), tile);
    }

    private void ComputeFishTileQuality(HexCoord coord, TileInstance tile)
    {
        if (!tile.data.fishable) return;

        int count = 0;

        foreach (var dir in HexDirectionExtensions.hexDirections)
            if (tileByCoord.TryGetValue(coord.Neighbor(dir), out var neighborTile) && neighborTile.data.fishable)
                count++;

        if (count == 0) tile.fishQuality = FishTileQuality.Poor;
        else if (count == 1) tile.fishQuality = FishTileQuality.Normal;
        else tile.fishQuality = FishTileQuality.Rich;
    }

    private void SetFishTileColor(SpriteRenderer sr, TileInstance tile)
    {
        sr.color = tile.fishQuality switch
        {
            FishTileQuality.Poor => new Color(0.95f, 0.95f, 0.9f),
            FishTileQuality.Normal => new Color(0.9f, 0.95f, 1f),
            FishTileQuality.Rich => new Color(0.8f, 0.9f, 1f),
            _ => Color.white
        };
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
