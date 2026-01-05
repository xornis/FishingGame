using HexDungeon;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private IslandManager islandManager;
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private PlayerInput playerInput;

    private InputAction clickAction;
    private TileView lastHovered;

    private void Awake()
    {
        clickAction = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        clickAction.performed += OnMouseClickPerformed;
    }

    private void OnDisable()
    {
        clickAction.performed -= OnMouseClickPerformed;
    }

    private void OnMouseClickPerformed(InputAction.CallbackContext ctx)
    {
        Tile currentTile = GetTileOnClick();

        if (currentTile != null)
            gameEvents.CallTileClicked(currentTile);
    }

    private void Update()
    {
        Tile currentTile = GetTileOnClick();
        TileView currentView = currentTile?.view;

        if (currentView != lastHovered)
        {
            lastHovered?.SetHover(false);
            currentView?.SetHover(true);
            lastHovered = currentView;
        }
    }

    private Tile GetTileOnClick()
    {
        Vector2 screenMousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, -Camera.main.transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(worldMousePos, Vector2.zero);

        if (hit.collider != null)
        {
            TileView view = hit.collider.GetComponent<TileView>();
            if (view != null)
            {
                HexCoord coord = islandManager.Layout.WorldToHex(view.transform.position);
                if (islandManager.tileByCoord.TryGetValue(coord, out var tile))
                {
                    if (tile.view == null) tile.view = view;
                    return tile;
                }
            }
        }

        HexCoord directCoord = islandManager.Layout.WorldToHex(worldMousePos);
        if (islandManager.tileByCoord.TryGetValue(directCoord, out var fallbackTile)) return fallbackTile;
        return null;
    }
}
