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

        TryCatch(clickedPos, currentPos);
    }

    private void TryCatch(in HexCoord clickedPos, in HexCoord currentPos)
    {
        if (currentPos.Distance(clickedPos) != 1) return;
        if (!manager.tileByCoord.TryGetValue(clickedPos, out var tile)) return;

        if (tile.fishable && canFish)
        {
            canFish = false;
            StartCoroutine(WaitForFishAndCatch());
        }
    }

    private IEnumerator WaitForFishAndCatch()
    {
        print("1...");
        yield return new WaitForSeconds(baseWaitingForFishInSeconds/3);
        print("2...");
        yield return new WaitForSeconds(baseWaitingForFishInSeconds/3);
        print("3...");
        yield return new WaitForSeconds(baseWaitingForFishInSeconds/3);

        Debug.Log((Random.value < catchChance) ? "Caught!" : "Got Away..");

        canFish = true;
    }
}
