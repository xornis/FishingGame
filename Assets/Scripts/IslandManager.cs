using HexDungeon;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    private HexRoomGenerator generator;

    public HexLayout Layout { get; private set; }
    public HashSet<HexCoord> tiles = new HashSet<HexCoord>();

    private void Awake()
    {
        generator = GetComponent<HexRoomGenerator>();

        var coords = generator.GenerateCoords(out var layout);
        Layout = layout;

        foreach (var coord in coords)
            tiles.Add(coord);
    }
}
