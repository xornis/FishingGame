using HexDungeon;
using System.Collections;
using UnityEngine;

public class FishingInteraction : MonoBehaviour
{
    [SerializeField] private IslandManager manager;
    [SerializeField] private StatsManager statsManager;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    private float CatchChance => statsManager.GetStat(StatType.CatchChance) / 100f;
    private float FishingSpeed => statsManager.GetStat(StatType.FishingSpeed);

    public bool CanFish { get; private set; } = true;
    private bool isFishing;

    private void OnEnable()
    {
        gameEvents.OnTileClicked += HandleFishingRequest;
        gameEvents.OnSetFishingPermission += SetFishingPermission;
    }

    private void OnDisable()
    {
        gameEvents.OnTileClicked -= HandleFishingRequest;
        gameEvents.OnSetFishingPermission -= SetFishingPermission;
    }

    private void HandleFishingRequest(Tile tile)
    {
        HexCoord currentPos = manager.Layout.WorldToHex(transform.position);
        bool target = currentPos.Distance(tile.coord) == 1 && tile.data.fishable;

        if (!target) return;

        if (!CanFish)
        {
            tile.view.PlayErrorEffect();
            return;
        }

        if (isFishing) return;
        
        StartCoroutine(WaitForFishAndCatch(tile));
    }

    private IEnumerator WaitForFishAndCatch(Tile tile)
    {
        isFishing = true;

        gameEvents.CallFishingAttempted();

        float waitTime = FishingSpeed;
        if (tile.state is FishTileState fishState1)
            waitTime *= GetTimeMultiplier(fishState1.fishQuality);

        float count = 1;
        float regularWaitTime = waitTime / count;

        for (int i = 0; i < count; i++)
        {
            tile.view.PlayShake(5f, 5f, regularWaitTime);
            yield return new WaitForSeconds(regularWaitTime);
        }

        float chance = CatchChance;
        if (tile.state is FishTileState fishState2)
            chance *= GetChanceMultiplier(fishState2.fishQuality);

        if (Random.value <= chance)
        {
            gameEvents.CallFishCaptured();
            tile.view.PlayPulse(1.2f, regularWaitTime / 2);
            yield return new WaitForSeconds(regularWaitTime / 2);
        }
        else
        {
            tile.view.PlayPulse(0.8f, regularWaitTime / 2);
            yield return new WaitForSeconds(regularWaitTime / 2);
        }

        isFishing = false;
    }

    private float GetChanceMultiplier(FishableTileData.FishTileQuality quality)
    {
        return quality switch
        {
            FishableTileData.FishTileQuality.Poor => 0.8f,
            FishableTileData.FishTileQuality.Normal => 1.1f,
            FishableTileData.FishTileQuality.Rich => 1.5f,
            _ => 1f
        };
    }

    private float GetTimeMultiplier(FishableTileData.FishTileQuality quality)
    {
        return quality switch
        {
            FishableTileData.FishTileQuality.Poor => 1.3f,
            FishableTileData.FishTileQuality.Normal => 1.1f,
            FishableTileData.FishTileQuality.Rich => 0.8f,
            _ => 1f
        };
    }

    public void SetFishingPermission(bool state) => CanFish = state;
}
