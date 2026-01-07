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

        gameEvents.CallStepsChanged(new ResourceData(stepsLeft, maxSteps, 0));
        gameEvents.CallFishingAttemptsChanged(new ResourceData(fishingAttemptsLeft, maxFishingAttempts, 0));
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
            stepEffect.Execute(this, tile);

        CheckRunStatus();
    }

    private void HandleFishingAttempt()
    {
        AddFishingAttemptsUI(1);
        ChangeFishingAttempts(-1);

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

    private void HandleFishCaptured() => AddFishCapturedUI(1);

    public void ChangeSteps(int amount)
    {
        stepsLeft += amount;
        gameEvents.CallStepsChanged(new ResourceData(stepsLeft, maxSteps, amount));
    }
    public void ChangeFishingAttempts(int amount)
    {
        fishingAttemptsLeft += amount;
        gameEvents.CallFishingAttemptsChanged(new ResourceData(fishingAttemptsLeft, maxFishingAttempts, amount));
    }

    public void AddStepsWalkedUI(int amount)
    {
        TotalStepsWalked += Mathf.Abs(amount);
        gameEvents.CallTotalStepsWalkedChanged(TotalStepsWalked);
    }
    public void AddFishCapturedUI(int amount)
    {
        TotalFishCaptured += Mathf.Abs(amount);
        gameEvents.CallTotalFishCapturedChanged(TotalFishCaptured);
    }
    public void AddFishingAttemptsUI(int amount)
    {
        TotalFishingAttempts += Mathf.Abs(amount);
        gameEvents.CallTotalFishingAttemptsChanged(TotalFishingAttempts);
    }

    public void RestartRun() => SceneManager.LoadScene(0);
}
