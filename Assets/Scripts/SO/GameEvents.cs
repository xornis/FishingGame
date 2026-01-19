using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Scriptable Objects/Events")]
public class GameEvents : ScriptableObject
{
    // Events
    public event Action<Tile> OnStepEnded;
    public event Action OnFishingAttempted;
    public event Action OnFishCaptured;
    public event Action OnIslandReady;
    public event Action<Tile> OnPlayerMoved;

    public event Action<Tile> OnTileClicked;

    #region UI
    public event Action<int> OnTotalFishCapturedChanged;
    #endregion UI

    // Event-Commands
    public event Action<bool> OnSetMovementPermission;
    public event Action<bool> OnSetFishingPermission;
    public event Action OnRunEnded;

    public void CallStepEnded(Tile tile) => OnStepEnded?.Invoke(tile);
    public void CallFishingAttempted() => OnFishingAttempted?.Invoke();
    public void CallFishCaptured() => OnFishCaptured?.Invoke();
    public void CallIslandReady() => OnIslandReady?.Invoke();
    public void CallPlayerMoved(Tile tile) => OnPlayerMoved?.Invoke(tile);

    public void CallTileClicked(Tile tile) => OnTileClicked?.Invoke(tile);

    public void CallTotalFishCapturedChanged(int value) => OnTotalFishCapturedChanged?.Invoke(value);

    public void SendMovementPermission(bool state) => OnSetMovementPermission?.Invoke(state);
    public void SendFishingPermission(bool state) => OnSetFishingPermission?.Invoke(state);
    public void SendRunEnded() => OnRunEnded?.Invoke();
}
