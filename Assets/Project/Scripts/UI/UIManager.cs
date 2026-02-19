using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HUDCanvas HUDCanvas;
    [SerializeField] private GameMenuCanvas gameMenuCanvas;
    [SerializeField] private ShopCanvas shopCanvas;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("References")]
    [SerializeField] private GameEvents gameEvents;
    
    private InputAction escapeAction;

    private void OnEnable()
    {
        escapeAction.performed += OnEscapeButtonPressed;
    }

    private void OnDisable()
    {
        escapeAction.performed -= OnEscapeButtonPressed;
    }

    private void Awake()
    {
        escapeAction = playerInput.actions["Toggle Game Menu"];
    }

    private void Start()
    {
        EnterGameplay();
    }

    private void OnEscapeButtonPressed(InputAction.CallbackContext ctx)
    {
        if (shopCanvas.IsVisible) return;

        if (gameMenuCanvas.IsVisible)
            EnterGameplay();
        else
            EnterGameMenu();
    }

    private void EnterGameplay()
    {
        HUDCanvas.Show();
        gameMenuCanvas.Hide();
        shopCanvas.Hide();
        gameEvents.SendGameplayPermission(true);
    }

    private void EnterGameMenu()
    {
        gameMenuCanvas.Show();
        gameEvents.SendGameplayPermission(false);
    }

    public void EnterShop(int dayNumber, int totalFishCaptured)
    {
        HUDCanvas.Hide();
        gameMenuCanvas.Hide();
        shopCanvas.SetupShop(dayNumber, totalFishCaptured);
        shopCanvas.Show();
        gameEvents.SendGameplayPermission(false);
    }
}
