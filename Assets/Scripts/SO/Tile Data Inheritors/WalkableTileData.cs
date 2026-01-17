using UnityEngine;

[CreateAssetMenu(fileName = "WalkableTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Walkable Tile Data")]
public class WalkableTileData : TileData, IStepEffect
{
    public int stepValueChange;
    [Range(0f, 3f)] public float moveDurationScale;
    public TileStepSounds stepSounds;

    public virtual void Execute(ResourceManager resourceManager, RunController runController, Tile tile)
    {
        resourceManager.ChangeResource(ResourceType.Steps, stepValueChange);
        runController.AddStepsWalkedUI(stepValueChange);
        AudioManager.Instance.PlayTileSounds(stepSounds);
    }
}

public interface IStepEffect
{
    public void Execute(ResourceManager resourceManager, RunController runController, Tile tile);
}