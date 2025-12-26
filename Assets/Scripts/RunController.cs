using HexDungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunController : MonoBehaviour
{
    [SerializeField] private UIController UIController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private FishingInteraction fishingInteraction;
    [SerializeField] private IslandManager islandManager;

    public int MaxSteps { get; private set; } = 10;
    public int StepsLeft { get; private set; }

    public int MaxFishTries { get; private set; } = 5;
    public int FishTriesLeft { get; private set; }

    public int FishTries { get; private set; } = 0;
    public int FishCaught { get; private set; } = 0;
    public int StepsWalked { get; private set; } = 0;

    public event System.Action OnRunEnded;

    private void OnEnable()
    {
        playerMovement.OnStepFinished += OnStepFinished;
        fishingInteraction.OnFishCaught += OnFishCaught;
        fishingInteraction.OnFishTry += OnFishTry;
    }

    private void OnDisable()
    {
        playerMovement.OnStepFinished -= OnStepFinished;
        fishingInteraction.OnFishCaught -= OnFishCaught;
        fishingInteraction.OnFishTry -= OnFishTry;
    }

    private void Start()
    {
        StepsLeft = MaxSteps;
        StepsLeft = Mathf.Clamp(StepsLeft, 0, MaxSteps);

        FishTriesLeft = MaxFishTries;
        FishTriesLeft = Mathf.Clamp(FishTriesLeft, 0, MaxFishTries);
    }

    private void OnStepFinished(TileInstance tile)
    {
        int cost = tile.data.stepCost;

        AddStepsWalked(cost);
        SubstractSteps(cost);

        if (StepsLeft <= 0)
            playerMovement.SetMovePermission(false);
        
        if (StepsLeft <= 1) 
            playerMovement.ClearQueuedStep();

        CheckEndRun();
    }

    private void OnFishTry()
    {
        AddFishTries(1);
        SubstractFishTries(1);

        if (FishTriesLeft <= 0)
        {
            fishingInteraction.SetFishingPermission(false);
            Debug.Log("No fish tries left");
        }

        CheckEndRun();
    }

    private void CheckEndRun()
    {
        if (FishTriesLeft <= 0)
        {
            fishingInteraction.SetFishingPermission(false);
            playerMovement.SetMovePermission(false);

            OnRunEnded?.Invoke();
        }
        if (StepsLeft <= 0 && !HasReachableFishTile()) OnRunEnded?.Invoke();
    }

    private bool HasReachableFishTile()
    {
        HexCoord playerPos = islandManager.Layout.WorldToHex(playerMovement.transform.position);

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

    private void OnFishCaught() => AddFishCaught(1);

    public void SubstractSteps(int amount) => StepsLeft -= amount;

    public void SubstractFishTries(int amount) => FishTriesLeft -= amount;

    public void AddStepsWalked(int amount) => StepsWalked += amount;
    public void AddFishCaught(int amount) => FishCaught += amount;
    public void AddFishTries(int amount) => FishTries += amount;

    public void RestartRun() => SceneManager.LoadScene(0);
}
