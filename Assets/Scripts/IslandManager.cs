using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [SerializeField] private HexRoomGenerator generator;
    [SerializeField] private TileAssigner tileAssigner;

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
            ComputeFishQuality(coord, tileByCoord[coord]);
            Spawn(coord, tileByCoord[coord]);
        }
    }

    private void Spawn(HexCoord coord, TileInstance tile)
    {
        var worldPos = generator.transform.TransformPoint(Layout.HexToWorld(coord));
        var go = Instantiate(tile.data.prefab, worldPos, Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * HexScale;
    }

    private void ComputeFishQuality(HexCoord coord, TileInstance tile)
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
}
