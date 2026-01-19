using UnityEngine;

[CreateAssetMenu(fileName = "CampfireTileData", menuName = "Scriptable Objects/Tiles/Tile Data Inheritors/Walkable Tile Data Inheritors/Campfire Tile Data")]
public class CampfireTileData : WalkableTileData, IStepEffect
{
    public TileStepSounds campfireSound;

    public override void Execute(ResourceManager resourceManager, Tile tile)
    {
        CampfireTileState state = tile.state as CampfireTileState;

        if (state == null || state.isUsed)
        {
            AudioManager.Instance.PlayTileSounds(this.stepSounds);
            resourceManager.ChangeResource(ResourceType.Steps, -1);
            return;
        }

        state.isUsed = true;

        AudioManager.Instance.PlayTileSounds(campfireSound);
        resourceManager.ChangeResource(ResourceType.Steps, stepValueChange);
    }
}
