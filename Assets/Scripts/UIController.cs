using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private GameObject endRunPanel;
    [SerializeField] private TextMeshProUGUI fishCaughtText;
    [SerializeField] private TextMeshProUGUI stepsWalkedText;

    [SerializeField] private RunController runController;
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        ToggleGameObject(endRunPanel, false);
        UpdateStepsText();
    }

    private void OnEnable()
    {
        runController.OnRunEnded += OnRunEnded;
        playerMovement.OnStepFinished += UpdateStepsText;
    }

    private void OnDisable()
    {
        runController.OnRunEnded -= OnRunEnded;
        playerMovement.OnStepFinished -= UpdateStepsText;
    }

    private void OnRunEnded()
    {
        ToggleGameObject(endRunPanel, true);
        SetStepsWalkedText();
        SetFishCaughtText();
    }

    private void UpdateStepsText() => stepsText.text = $"{runController.StepsLeft}/{runController.MaxSteps}";
    private void ToggleGameObject(GameObject gameObject, bool state) => gameObject.SetActive(state);
    private void SetStepsWalkedText() => stepsWalkedText.text = $"Steps Walked: {runController.StepsWalked}";
    private void SetFishCaughtText() => fishCaughtText.text = $"Fish Caught: {runController.FishCaught}";
}
