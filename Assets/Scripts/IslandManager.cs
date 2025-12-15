using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [SerializeField] private HexRoomGenerator generator;
    [SerializeField] private TileAssigner tileAssigner;

    public HexLayout Layout { get; private set; }
    public float HexScale { get; private set; }
    public readonly Dictionary<HexCoord, TileData> tileByCoord = new();

    private void Start()
    {
        var coords = generator.GetCoords(out var layout, out var hexScale);

        Layout = layout;
        HexScale = hexScale;

        var coordSet = new HashSet<HexCoord>(coords);

        foreach (var coord in coordSet)
            tileByCoord[coord] = tileAssigner.AssignBaseTile(coord, coordSet);

        foreach (var coord in coordSet)
            tileByCoord[coord] = tileAssigner.ApplyRock(coord, tileByCoord);

        foreach (var coord in coordSet)
            Spawn(coord, tileByCoord[coord]);
    }

    private void Spawn(HexCoord coord, TileData tile)
    {
        var worldPos = generator.transform.TransformPoint(Layout.HexToWorld(coord));
        var go = Instantiate(tile.prefab, worldPos, Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * HexScale;
    }
}
