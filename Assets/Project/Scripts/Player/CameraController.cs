using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed = 7f;
    [SerializeField] private float resetDuration = 0.35f;
    [SerializeField] private GameEvents gameEvents;

    private bool gameplayAllowed = true;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction resetCameraAction;
    private Coroutine resetRoutine;

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        resetCameraAction = playerInput.actions["Reset Camera"];
    }

    private void OnEnable()
    {
        resetCameraAction.performed += ResetCameraToPlayer;
        gameEvents.OnSetGameplayPermission += SetGameplayPermission;
    }

    private void OnDisable()
    {
        resetCameraAction.performed -= ResetCameraToPlayer;
        gameEvents.OnSetGameplayPermission -= SetGameplayPermission;
    }

    private void Update()
    {
        if (!gameplayAllowed) return;

        Vector2 move = moveAction.ReadValue<Vector2>();

        if (move != Vector2.zero && resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }

        Vector3 moving = new Vector3(move.x, move.y, 0f) * panSpeed * Time.deltaTime;
        transform.position += moving;
    }

    private void SetGameplayPermission(bool state) => gameplayAllowed = state;

    private void ResetCameraToPlayer(InputAction.CallbackContext ctx)
    {
        if (!gameplayAllowed) return;

        if (resetRoutine != null)
            StopCoroutine(resetRoutine);

        resetRoutine = StartCoroutine(SmoothReset());
    }

    private IEnumerator SmoothReset()
    {
        Vector3 start = transform.position;
        Vector3 target = transform.parent.position;

        float t = 0f;

        while (t < resetDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t) / resetDuration);
            yield return null;
        }

        transform.position = target;
        resetRoutine = null;
    }

}
