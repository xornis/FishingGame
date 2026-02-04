using UnityEngine;

[CreateAssetMenu(fileName = "Tile Sounds", menuName = "Scriptable Objects/Tiles/Tile Step Sounds")]
public class TileStepSounds : ScriptableObject
{
    public AudioClip[] tileSounds;
    [Range(0f, 1f)] public float volume = 1f;
}
