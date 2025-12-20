using UnityEngine;
using UnityEngine.SceneManagement;

public class RunController : MonoBehaviour
{
    [SerializeField] private UIController UIController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private FishingInteraction fishingInteraction;

    public int MaxSteps { get; private set; } = 10;
    public int StepsLeft { get; private set; }

    public int StepsWalked { get; private set; } = 0;
    public int FishCaught { get; private set; } = 0;

    public event System.Action OnRunEnded;

    private void OnEnable()
    {
        playerMovement.OnStepFinished += OnStepFinished;
        fishingInteraction.OnFishCaught += OnFishCaught;
    }

    private void OnDisable()
    {
        playerMovement.OnStepFinished -= OnStepFinished;
        fishingInteraction.OnFishCaught -= OnFishCaught;
    }

    private void Start()
    {
        StepsLeft = MaxSteps;
        StepsLeft = Mathf.Clamp(StepsLeft, 0, MaxSteps);
    }

    private void OnStepFinished()
    {
        AddStepsWalked(1);
        SubstractSteps(1);

        if (StepsLeft == 0)
        {
            playerMovement.SetMovePermission(false);
            fishingInteraction.SetFishingPermission(false);
            
            Debug.Log("No steps left");

            OnRunEnded?.Invoke();
        }
    }

    private void OnFishCaught() => AddFishCaught(1);

    public void SubstractSteps(int amount) => StepsLeft -= amount;

    public void AddFishCaught(int amount) => FishCaught += amount;
    public void AddStepsWalked(int amount) => StepsWalked += amount;

    public void RestartRun() => SceneManager.LoadScene(0);
}
