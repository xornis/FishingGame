using HexDungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private IslandManager islandManager;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    [Header("Settings")]
    public int maxSteps = 10;
    public int maxFishingAttempts = 5;

    public int StepsLeft { get; private set; }
    public int FishingAttemptsLeft { get; private set; }

    public int TotalFishingAttempts { get; private set; }
    public int TotalFishCaught { get; private set; }
    public int TotalStepsWalked { get; private set; }

    private void Awake()
    {
        StepsLeft = maxSteps;
        FishingAttemptsLeft = maxFishingAttempts;
    }

    private void OnEnable()
    {
        gameEvents.OnStepEnded += HandleStep;
        gameEvents.OnFishCaught += HandleFishCaught;
        gameEvents.OnFishingAttempted += HandleFishingAttempt;
    }

    private void OnDisable()
    {
        gameEvents.OnStepEnded -= HandleStep;
        gameEvents.OnFishCaught -= HandleFishCaught;
        gameEvents.OnFishingAttempted -= HandleFishingAttempt;
    }

    private void HandleStep(TileInstance tile)
    {
        int cost = tile.data.stepCost;

        AddStepsWalked(cost);
        SubtractSteps(cost);

        if (StepsLeft <= 0)
            gameEvents.SendMovementPermission(false);

        CheckRunStatus();
    }

    private void HandleFishingAttempt()
    {
        AddFishingAttempts(1);
        SubtractFishingAttempts(1);

        if (FishingAttemptsLeft <= 0)
            gameEvents.SendFishingPermission(false);

        CheckRunStatus();
    }

    private void CheckRunStatus()
    {
        if (FishingAttemptsLeft <= 0)
        {
            gameEvents.SendFishingPermission(false);
            gameEvents.SendMovementPermission(false);

            gameEvents.SendRunEnded();
        }
        if (StepsLeft <= 0 && !HasReachableFishTile()) gameEvents.SendRunEnded();
    }

    private bool HasReachableFishTile()
    {
        HexCoord playerPos = islandManager.Layout.WorldToHex(playerTransform.position);

        foreach (var dir in HexDirectionExtensions.hexDirections)
        {
            HexCoord neighbor = playerPos.Neighbor(dir);

            if (!islandManager.tileByCoord.TryGetValue(neighbor, out var tile))
                continue;

            if (tile.data.fishable)
                return true;
        }

        return false;
    }

    private void HandleFishCaught() => AddFishCaught(1);

    public void SubtractSteps(int amount) => StepsLeft -= amount;

    public void SubtractFishingAttempts(int amount) => FishingAttemptsLeft -= amount;

    public void AddStepsWalked(int amount) => TotalStepsWalked += amount;
    public void AddFishCaught(int amount) => TotalFishCaught += amount;
    public void AddFishingAttempts(int amount) => TotalFishingAttempts += amount;

    public void RestartRun() => SceneManager.LoadScene(0);
}
