using HexDungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayController : MonoBehaviour
{
    [SerializeField] private IslandManager islandManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private UIManager UIManager;

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
        gameEvents.OnPlayerMoved += HandlePlayerMoved;
        gameEvents.OnFishCaptured += HandleFishCaptured;
        gameEvents.OnFishingAttempted += HandleFishingAttempt;
    }

    private void OnDisable()
    {
        gameEvents.OnStepEnded -= HandleStep;
        gameEvents.OnPlayerMoved -= HandlePlayerMoved;
        gameEvents.OnFishCaptured -= HandleFishCaptured;
        gameEvents.OnFishingAttempted -= HandleFishingAttempt;
    }

    private void HandleStep(Tile tile)
    {
        if (tile.data is IStepEffect stepEffect)
            stepEffect.Execute(resourceManager, tile);

        CheckDayStatus();
    }

    private void HandlePlayerMoved(Tile tile) => currentPlayerPos = tile.coord;

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
        bool canFish = fishingAttemptsLeft > 0 && islandManager.HasReachableFishTile(currentPlayerPos);

        if (canMove || canFish)
        {
            gameEvents.SendMovementPermission(canMove);
            gameEvents.SendFishingPermission(canFish);
        }
        else EndDay();
    }

    private void HandleFishCaptured() => AddFishCapturedUI(1);

    private int CalculateCoins()
    {
        int coinsPerFish = Mathf.RoundToInt(statsManager.GetStat(StatType.CoinsPerFish));
        int earnedCoins = TotalFishCaptured * coinsPerFish;

        resourceManager.ChangeResource(ResourceType.Coins, earnedCoins);

        int totalCoins = resourceManager.GetResourceAmount(ResourceType.Coins);
        SaveSystem.SaveCurrentResource(ResourceType.Coins, totalCoins);

        return earnedCoins;
    }

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
        gameEvents.SendDayEnded();

        int earnedCoins = CalculateCoins();
        shopManager.GenerateShopButtons(resourceManager);
        UIManager.ShowShop(currentDay, TotalFishCaptured);
    }
}
