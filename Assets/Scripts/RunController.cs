using UnityEngine;
using UnityEngine.SceneManagement;

public class RunController : MonoBehaviour
{
    [SerializeField] private UIController UIController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private FishingInteraction fishingInteraction;

    public int MaxSteps { get; private set; } = 10;
    public int StepsLeft { get; private set; }

    public event System.Action OnRunEnded;

    private void OnEnable()
    {
        playerMovement.OnStepFinished += OnStepFinished;
    }

    private void OnDisable()
    {
        playerMovement.OnStepFinished -= OnStepFinished;
    }

    private void Start()
    {
        StepsLeft = MaxSteps;
        StepsLeft = Mathf.Clamp(StepsLeft, 0, MaxSteps);
    }

    private void OnStepFinished()
    {
        SubstractSteps(1);

        if (StepsLeft == 0)
        {
            playerMovement.SetMovePermission(false);
            fishingInteraction.SetFishingPermission(false);
            
            OnRunEnded?.Invoke();

            Debug.Log("No steps left");
        }
    }

    public void SubstractSteps(int amount) => StepsLeft -= amount;

    public void RestartRun() => SceneManager.LoadScene(0);
}
