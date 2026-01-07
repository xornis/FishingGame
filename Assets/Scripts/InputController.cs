using HexDungeon;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private IslandManager islandManager;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform playerTransform;

    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    private InputAction clickAction;
    private TileView lastHovered;

    private void Awake() => clickAction = playerInput.actions["Attack"];
    private void OnEnable() => clickAction.performed += OnMouseClickPerformed;
    private void OnDisable() => clickAction.performed -= OnMouseClickPerformed;

    private void OnMouseClickPerformed(InputAction.CallbackContext ctx)
    {
        Tile currentTile = GetTileOnClick();

        if (currentTile == null) return;

        Vector3 playerPos = playerTransform ? playerTransform.position : transform.position;
        HexCoord playerHex = islandManager.Layout.WorldToHex(playerPos);
        bool neighbor = playerHex.Distance(currentTile.coord) == 1;

        if (!neighbor || (!currentTile.data.walkable && !currentTile.data.fishable))
            currentTile.view.PlayErrorEffect();
        else
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
        return islandManager.tileByCoord.TryGetValue(directCoord, out var fallbackTile) ? fallbackTile : null;
    }
}
