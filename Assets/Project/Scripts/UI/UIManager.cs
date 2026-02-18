using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HUDCanvas HUDCanvas;
    [SerializeField] private GameMenuCanvas gameMenuCanvas;
    [SerializeField] private ShopCanvas shopCanvas;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    
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
        InitializeCanvases();
    }

    private void OnEscapeButtonPressed(InputAction.CallbackContext ctx)
    {
        if (shopCanvas.IsVisible) return;
        ToggleGameMenu();
    }

    private void InitializeCanvases()
    {
        HUDCanvas.Show();

        gameMenuCanvas.Hide();
        shopCanvas.Hide();
    }

    public void ToggleGameMenu()
    {
        if (gameMenuCanvas.IsVisible)
            gameMenuCanvas.Hide();
        else
            gameMenuCanvas.Show();
    }

    public void ShowShop(int dayNumber, int totalFishCaptured, int earnedCoins)
    {
        gameMenuCanvas.Hide();

        shopCanvas.SetupShop(dayNumber, totalFishCaptured, earnedCoins);
        shopCanvas.Show();
    }

    public void CloseShop()
    {
        shopCanvas.Hide();
    }

    public ShopCanvas GetShopCanvas() => shopCanvas;
}
