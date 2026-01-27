using HexDungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunController : MonoBehaviour
{
    [SerializeField] private IslandManager islandManager;
    [SerializeField] private ResourceManager resourceManager;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    private HexCoord currentPlayerPos;
    private bool dayActive;

    public int TotalFishCaptured { get; private set; }

    private void Start()
    {
        dayActive = true;
        CheckDayStatus();
    }

    private void OnEnable()
    {
        gameEvents.OnStepEnded += HandleStep;
        gameEvents.OnPlayerMoved += (tile) => currentPlayerPos = tile.coord;
        gameEvents.OnFishCaptured += HandleFishCaptured;
        gameEvents.OnFishingAttempted += HandleFishingAttempt;
    }

    private void OnDisable()
    {
        gameEvents.OnStepEnded -= HandleStep;
        gameEvents.OnPlayerMoved -= (tile) => currentPlayerPos = tile.coord;
        gameEvents.OnFishCaptured -= HandleFishCaptured;
        gameEvents.OnFishingAttempted -= HandleFishingAttempt;
    }

    private void HandleStep(Tile tile)
    {
        currentPlayerPos = tile.coord;

        if (tile.data is IStepEffect stepEffect)
            stepEffect.Execute(resourceManager, tile);

        CheckDayStatus();
    }

    private void HandleFishingAttempt()
    {
        resourceManager.ChangeResource(ResourceType.FishingAttempts, -1);

        CheckDayStatus();
    }

    private void CheckDayStatus()
    {
        if (!dayActive) return;

        int stepsLeft = resourceManager.GetResourceAmount(ResourceType.Steps);
        int fishingAttemptsLeft = resourceManager.GetResourceAmount(ResourceType.FishingAttempts);

        bool canMove = stepsLeft > 0;
        bool canFish = fishingAttemptsLeft > 0 && HasReachableFishTile();

        if (canMove || canFish)
        {
            gameEvents.SendMovementPermission(canMove);
            gameEvents.SendFishingPermission(canFish);
        }
        else EndDay();
    }

    private bool HasReachableFishTile()
    {
        foreach (var dir in HexDirectionExtensions.hexDirections)
        {
            HexCoord neighbor = currentPlayerPos.Neighbor(dir);

            if (!islandManager.tileByCoord.TryGetValue(neighbor, out var tile))
                continue;
            if (tile.data.fishable)
                return true;
        }
        return false;
    }

    private void HandleFishCaptured() => AddFishCapturedUI(1);

    public void AddFishCapturedUI(int amount)
    {
        TotalFishCaptured += Mathf.Abs(amount);
        gameEvents.CallTotalFishCapturedChanged(TotalFishCaptured);
    }

    public void RestartDay() => SceneManager.LoadScene(0);
    public void EndDay()
    {
        if (!dayActive) return;

        dayActive = false;

        int currentDay = SaveSystem.LoadDay();
        SaveSystem.SaveDay(currentDay+1);

        gameEvents.SendMovementPermission(false);
        gameEvents.SendFishingPermission(false);
        gameEvents.SendRunEnded();
    }
}
