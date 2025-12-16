using UnityEngine;

namespace HexDungeon
{
    public enum HexOrientation
    {
        FlatTop,
        PointyTop
    }

    public class HexLayout
    {
        public HexOrientation Orientation { get; }
        public float Size { get; }

        public HexLayout(HexOrientation orientation, float size)
        {
            Orientation = orientation;
            Size = Mathf.Max(0.0001f, size);
        }

        public Vector3 HexToWorld(HexCoord hex) =>
            Orientation == HexOrientation.FlatTop
            ? FlatTopHexToWorld(hex)
            : PointyTopHexToWorld(hex);

        public HexCoord WorldToHex(Vector3 world) =>
            Orientation == HexOrientation.FlatTop
            ? FlatTopWorldToHex(world)
            : PointyTopWorldToHex(world);

        private Vector3 FlatTopHexToWorld(HexCoord hex)
        {
            float size = Size;

            float w = size * 2f;
            float h = Mathf.Sqrt(3f) * size;

            float x = (3f * size / 2f) * hex.Q;
            float y = (h * (hex.R + hex.Q * 0.5f));

            return new Vector3(x, y, 0f);
        }

        private Vector3 PointyTopHexToWorld(HexCoord hex)
        {
            float size = Size;

            float w = Mathf.Sqrt(3f) * size;
            float h = size * 2f;

            float x = w * (hex.Q + hex.R * 0.5f);
            float y = (3f * size / 2f) * hex.R;

            return new Vector3(x, y, 0f);
        }

        private HexCoord FlatTopWorldToHex(Vector3 world)
        {
            float size = Size;

            float q = (2f / 3f) * (world.x / size);
            float r = (world.y / (Mathf.Sqrt(3f) * size)) - (q * 0.5f);

            Vector3 cube = new Vector3(q, -q - r, r);
            Vector3 rounded = HexGeometryUtils.CubeRound(cube);

            return new HexCoord(Mathf.RoundToInt(rounded.x), Mathf.RoundToInt(rounded.z));
        }

        private HexCoord PointyTopWorldToHex(Vector3 world)
        {
            float size = Size;

            float r = (2f / 3f) * (world.y / size);
            float q = (world.x / (Mathf.Sqrt(3f) * size)) - (r * 0.5f);

            Vector3 cube = new Vector3(q, -q - r, r);
            Vector3 rounded = HexGeometryUtils.CubeRound(cube);

            return new HexCoord(Mathf.RoundToInt(rounded.x), Mathf.RoundToInt(rounded.z));
        }
    }
}
