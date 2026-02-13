using HexDungeon;
using System.Collections;
using UnityEngine;

public class FishingInteraction : MonoBehaviour
{
    [SerializeField] private IslandManager manager;
    [SerializeField] private PlayerStats playerStats;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    private float CatchChance => playerStats.GetStat(StatType.CatchChance) / 100f;
    private float FishingSpeed => playerStats.GetStat(StatType.FishingSpeed);

    public bool CanFish { get; private set; } = true;
    private bool isFishing;

    private void OnEnable()
    {
        gameEvents.OnTileClicked += HandleFishingRequest;
        gameEvents.OnSetFishingPermission += SetFishingPermission;
        gameEvents.OnDayEnded += HandleDayEnded;

        playerStats.OnStatChanged += HandleStatChanged;
    }

    private void OnDisable()
    {
        gameEvents.OnTileClicked -= HandleFishingRequest;
        gameEvents.OnSetFishingPermission -= SetFishingPermission;
        gameEvents.OnDayEnded -= HandleDayEnded;

        playerStats.OnStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(StatType type, float value)
    {
        if (type == StatType.CatchChance)
            print($"CatchChanceStat: {value}, CatchChance: {CatchChance}");
        if (type == StatType.FishingSpeed)
            print($"FishingSpeedStat: {value}, FishingSpeed: {FishingSpeed}");
    }

    private void HandleDayEnded()
    {
        SetFishingPermission(false);
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

        print($"CatchChanceStat: {playerStats.GetStat(StatType.CatchChance)}, CatchChance: {CatchChance}");
        print($"FishingSpeedStat: {playerStats.GetStat(StatType.FishingSpeed)}, FishingSpeed: {FishingSpeed}");
    }

    private IEnumerator WaitForFishAndCatch(Tile tile)
    {
        isFishing = true;

        gameEvents.CallFishingAttempted();

        float waitTime = FishingSpeed;
        if (tile.state is FishTileState fishState1)
            waitTime *= GetTimeMultiplier(fishState1.fishQuality);

        float count = 2;
        float regularWaitTime = waitTime / count;

        for (int i = 0; i < count; i++)
        {
            tile.view.PlayPulse(1.1f, regularWaitTime);
            yield return new WaitForSeconds(regularWaitTime);
        }

        float chance = CatchChance;
        if (tile.state is FishTileState fishState2)
            chance *= GetChanceMultiplier(fishState2.fishQuality);

        if (Random.value <= chance)
        {
            gameEvents.CallFishCaptured();
            tile.view.PlayPulse(1.3f, regularWaitTime / 2);
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
