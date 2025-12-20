using TMPro;
using UnityEngine;

public class StepsController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private int maxSteps = 10;

    private int stepsLeft;

    private TextMeshProUGUI stepsText;

    private void Awake()
    {
        stepsText = GetComponent<TextMeshProUGUI>();
    }

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
        stepsLeft = maxSteps;
        UpdateText();
    }

    private void OnStepFinished()
    {
        stepsLeft--;
        stepsLeft = Mathf.Clamp(stepsLeft, 0, maxSteps);

        UpdateText();
        if (stepsLeft == 0)
        {
            playerMovement.SetMovePermission(false);
            Debug.Log("No steps left");
        }
    }

    private void UpdateText()
    {
        stepsText.text = $"{stepsLeft}/{maxSteps}";
    } 
}
