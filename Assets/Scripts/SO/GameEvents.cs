using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Scriptable Objects/Events")]
public class GameEvents : ScriptableObject
{
    public event Action<TileInstance> OnStepEnded;
    public event Action OnFishingAttempted;
    public event Action OnFishCaught;
    public event Action OnIslandReady;

    public event Action<bool> OnSetMovementPermission;
    public event Action<bool> OnSetFishingPermission;
    public event Action OnRunEnded;

    public void CallStepEnded(TileInstance tile) => OnStepEnded?.Invoke(tile);
    public void CallFishingAttempted() => OnFishingAttempted?.Invoke();
    public void CallFishCaught() => OnFishCaught?.Invoke();
    public void CallIslandReady() => OnIslandReady?.Invoke();

    public void SendMovementPermission(bool state) => OnSetMovementPermission?.Invoke(state);
    public void SendFishingPermission(bool state) => OnSetFishingPermission?.Invoke(state);
    public void SendRunEnded() => OnRunEnded?.Invoke();
}
