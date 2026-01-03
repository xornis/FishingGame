using UnityEngine;

[CreateAssetMenu(fileName = "CampfireTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Walkable Tile Data Inheritors/Campfire Tile Data")]
public class CampfireTileData : WalkableTileData, IStepEffect
{
    public override void Execute(RunController runController, Tile tile)
    {
        CampfireTileState state = tile.state as CampfireTileState;

        if (state == null || state.isUsed)
        {
            runController.ChangeSteps(-1);
            return;
        }

        state.isUsed = true;

        runController.ChangeSteps(stepValueChange);

        Debug.Log(state.isUsed);
    }
}
