using HexDungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingInteraction : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float baseCatchChance = 0.25f;
    [SerializeField] private float baseWaitingForFishInSeconds = 2f;

    [SerializeField] private IslandManager manager;
    
    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;

    public bool CanFish { get; private set; } = true;
    private bool isFishing;

    private PlayerInput playerInput;
    private InputAction action;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        action = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        action.performed += OnClick;
        gameEvents.OnSetFishingPermission += SetFishingPermission;
    }

    private void OnDisable()
    {
        action.performed -= OnClick;
        gameEvents.OnSetFishingPermission -= SetFishingPermission;
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (!CanFish || isFishing) return;

        if (TryGetClickedFishTile(out Tile tile, out Transform hitTransform))
            StartCoroutine(WaitForFishAndCatch(tile, hitTransform));
    }

    private bool TryGetClickedFishTile(out Tile tile, out Transform hitTransform)
    {
        tile = default;
        hitTransform = default;

        var layout = manager.Layout;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        HexCoord clickedPos = layout.WorldToHex(worldPos);
        HexCoord currentPos = layout.WorldToHex(transform.position);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (currentPos.Distance(clickedPos) != 1) return false;
        if (!manager.tileByCoord.TryGetValue(clickedPos, out var clickedTile)) return false;
        if (!clickedTile.data.fishable) return false;

        tile = clickedTile;
        hitTransform = hit.transform;
        return true;
    }

    private IEnumerator WaitForFishAndCatch(Tile tile, Transform hitTransform)
    {
        isFishing = true;

        gameEvents.CallFishingAttempted();

        float waitTime = baseWaitingForFishInSeconds * GetTimeMultiplier(tile.fishQuality);

        yield return StartCoroutine(AnimateScalePing(hitTransform, waitTime/3, 1.1f));
        yield return StartCoroutine(AnimateScalePing(hitTransform, waitTime/3, 1.1f));

        if (Random.value <= baseCatchChance * GetChanceMultiplier(tile.fishQuality))
        {
            gameEvents.CallFishCaptured();
            yield return StartCoroutine(AnimateScalePing(hitTransform, waitTime/4, 1.4f));
        }
        else
            yield return StartCoroutine(AnimateScalePing(hitTransform, waitTime/6, 0.8f));
        
        isFishing = false;
    }

    private IEnumerator AnimateScalePing(Transform targetTransform, float duration, float animationStrength)
    {
        float timer = 0f;
        Vector3 originalScale = targetTransform.localScale;
        Vector3 targetScale = originalScale * animationStrength;
        targetTransform.localScale = originalScale;

        while (timer < duration)
        {
            float t = Mathf.PingPong(timer / duration * 2f, 1f);
            targetTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            timer += Time.deltaTime;
            yield return null;
        }
        targetTransform.localScale = originalScale;
    }

    private float GetChanceMultiplier(FishTileQuality quality)
    {
        return quality switch
        {
            FishTileQuality.Poor => 0.8f,
            FishTileQuality.Normal => 1.1f,
            FishTileQuality.Rich => 1.5f,
            _ => 1f
        };
    }

    private float GetTimeMultiplier(FishTileQuality quality)
    {
        return quality switch
        {
            FishTileQuality.Poor => 1.3f,
            FishTileQuality.Normal => 1.1f,
            FishTileQuality.Rich => 0.8f,
            _ => 1f
        };
    }

    public void SetFishingPermission(bool state) => CanFish = state;
}
