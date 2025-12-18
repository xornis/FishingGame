using UnityEngine;

public class TileView : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color baseColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    public void HighlightTiles(bool value)
    {
        sr.color = value
            ? Color.Lerp(baseColor, Color.black, 0.2f)
            : baseColor;
    }
}
