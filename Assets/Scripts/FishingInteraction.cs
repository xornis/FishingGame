using HexDungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingInteraction : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float catchChance = 0.25f;
    [SerializeField] private float baseWaitingForFishInSeconds = 2f;

    [SerializeField] private IslandManager manager;

    private PlayerInput playerInput;
    private InputAction action;

    private bool canFish = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        action = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        action.performed += OnClick;
    }

    private void OnDisable()
    {
        action.performed -= OnClick;
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        var layout = manager.Layout;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        HexCoord clickedPos = layout.WorldToHex(worldPos);
        HexCoord currentPos = layout.WorldToHex(transform.position);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        TryCatch(clickedPos, currentPos, hit.transform);
    }

    private void TryCatch(in HexCoord clickedPos, in HexCoord currentPos, Transform hitTransform)
    {
        if (currentPos.Distance(clickedPos) != 1) return;
        if (!manager.tileByCoord.TryGetValue(clickedPos, out var tile)) return;

        if (tile.fishable && canFish)
        {
            canFish = false;
            StartCoroutine(WaitForFishAndCatch(hitTransform));
        }
    }

    private IEnumerator WaitForFishAndCatch(Transform hitTransform)
    {
        print("3...");
        yield return StartCoroutine(AnimateScalePing(hitTransform, baseWaitingForFishInSeconds/4, 1.1f));
        print("2...");
        yield return StartCoroutine(AnimateScalePing(hitTransform, baseWaitingForFishInSeconds/4, 1.1f));
        print("1...");
        yield return StartCoroutine(AnimateScalePing(hitTransform, baseWaitingForFishInSeconds/4, 1.1f));

        string message = Random.value < catchChance ? "Caught!" : "Got Away..";
        if (Random.value < catchChance)
            yield return StartCoroutine(AnimateScalePing(hitTransform, baseWaitingForFishInSeconds/4, 1.4f));
        else
            yield return StartCoroutine(AnimateScalePing(hitTransform, baseWaitingForFishInSeconds/6, 0.8f));
        Debug.Log(message);

        canFish = true;
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
}
