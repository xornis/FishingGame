using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private GameObject endRunPanel;

    [SerializeField] private RunController runController;
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        ToggleEndRunPanel(false);
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

    private void OnRunEnded() => ToggleEndRunPanel(true);

    private void UpdateStepsText() => stepsText.text = $"{runController.StepsLeft}/{runController.MaxSteps}";
    private void ToggleEndRunPanel(bool state) => endRunPanel.SetActive(state);
}
