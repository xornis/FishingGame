using UnityEngine;

[CreateAssetMenu(fileName = "WalkableTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Walkable Tile Data")]
public class WalkableTileData : TileData, IStepEffect
{
    [SerializeField] protected int stepValueChange;
    [SerializeField] protected float moveSpeedMultiplier;
    [SerializeField] protected TileStepSounds stepSounds;

    public float MoveSpeedMultiplier => 1f / Mathf.Max(Mathf.Epsilon, moveSpeedMultiplier);
    
    public virtual void Execute(ResourceManager resourceManager, Tile tile)
    {
        resourceManager.ChangeResource(ResourceType.Steps, stepValueChange);
        AudioManager.Instance.PlayTileSounds(stepSounds);
    }
}

public interface IStepEffect
{
    public void Execute(ResourceManager resourceManager, Tile tile);
}