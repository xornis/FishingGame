using UnityEngine;

[CreateAssetMenu(fileName = "WalkableTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Walkable Tile Data")]
public class WalkableTileData : TileData, IStepEffect
{
    public int stepValueChange;
    [Range(0f, 3f)] public float moveDurationScale;

    public virtual void Execute(RunController runController, Tile tile)
    {
        runController.ChangeSteps(stepValueChange);
        runController.AddStepsWalkedUI(stepValueChange);
    }
}

public interface IStepEffect
{
    public void Execute(RunController runController, Tile tile);
}