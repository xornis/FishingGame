using HexDungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunController : MonoBehaviour
{
    [SerializeField] private IslandManager islandManager;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    [Header("Settings")]
    public int maxSteps = 10;
    public int maxFishingAttempts = 5;

    private HexCoord currentPlayerPos;

    private int stepsLeft;
    private int fishingAttemptsLeft;

    public int TotalFishingAttempts { get; private set; }
    public int TotalFishCaptured { get; private set; }
    public int TotalStepsWalked { get; private set; }

    private void Start()
    {
        stepsLeft = maxSteps;
        fishingAttemptsLeft = maxFishingAttempts;

        gameEvents.CallStepsChanged(new ResourceData(stepsLeft, maxSteps));
        gameEvents.CallFishingAttemptsChanged(new ResourceData(fishingAttemptsLeft, maxFishingAttempts));
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

        int cost = tile.data.stepCost;
        AddStepsWalked(cost);
        SubtractSteps(cost);

        gameEvents.CallStepsChanged(new ResourceData(stepsLeft, maxSteps));

        CheckRunStatus();
    }

    private void HandleFishingAttempt()
    {
        AddFishingAttempts(1);
        SubtractFishingAttempts(1);

        gameEvents.CallFishingAttemptsChanged(new ResourceData(fishingAttemptsLeft, maxFishingAttempts));

        CheckRunStatus();
    }

    private void CheckRunStatus()
    {
        bool canMove = stepsLeft > 0;
        bool canFish = fishingAttemptsLeft > 0 && HasReachableFishTile();

        gameEvents.SendMovementPermission(canMove);
        gameEvents.SendFishingPermission(canFish);
        
        if (!canMove && !canFish) gameEvents.SendRunEnded();
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

    private void HandleFishCaptured() => AddFishCaptured(1);

    public void SubtractSteps(int amount) => stepsLeft -= amount;

    public void SubtractFishingAttempts(int amount) => fishingAttemptsLeft -= amount;

    public void AddStepsWalked(int amount)
    {
        TotalStepsWalked += amount;
        gameEvents.CallTotalStepsWalkedChanged(TotalStepsWalked);
    }
    public void AddFishCaptured(int amount)
    {
        TotalFishCaptured += amount;
        gameEvents.CallTotalFishCapturedChanged(TotalFishCaptured);
    }
    public void AddFishingAttempts(int amount)
    {
        TotalFishingAttempts += amount;
        gameEvents.CallTotalFishingAttemptsChanged(TotalFishingAttempts);
    }

    public void RestartRun() => SceneManager.LoadScene(0);
}
