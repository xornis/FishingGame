using UnityEngine;

[CreateAssetMenu(fileName = "CampfireTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Walkable Tile Data Inheritors/Campfire Tile Data")]
public class CampfireTileData : WalkableTileData, IStepEffect
{
    public override void Execute(RunController runController, Tile tile)
    {
        if (tile.state is CampfireTileState { isUsed: true })
        {
            base.Execute(runController, tile);
            return;
        }

        runController.ChangeSteps(stepValueChange);

        if (tile.state is CampfireTileState campfireTileState)
            campfireTileState.isUsed = true;
    }
}
