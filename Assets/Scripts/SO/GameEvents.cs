using UnityEngine;
using System;
using HexDungeon;

[Serializable]
public struct ResourceData
{
    public int current;
    public int max;

    public ResourceData(int current, int max)
    {
        this.current = current;
        this.max = max;
    }
}

[CreateAssetMenu(fileName = "GameEvents", menuName = "Scriptable Objects/Events")]
public class GameEvents : ScriptableObject
{
    // Events
    public event Action<TileInstance> OnStepEnded;
    public event Action OnFishingAttempted;
    public event Action OnFishCaptured;
    public event Action OnIslandReady;
    public event Action<HexCoord> OnPlayerMoved;

    #region UI
    public event Action<ResourceData> OnStepsChanged;
    public event Action<ResourceData> OnFishingAttemptsChanged;

    public event Action<int> OnTotalStepsWalkedChanged;
    public event Action<int> OnTotalFishCapturedChanged;
    public event Action<int> OnTotalFishingAttemptsChanged;
    #endregion UI

    // Event-Commands
    public event Action<bool> OnSetMovementPermission;
    public event Action<bool> OnSetFishingPermission;
    public event Action OnRunEnded;

    public void CallStepEnded(TileInstance tile) => OnStepEnded?.Invoke(tile);
    public void CallFishingAttempted() => OnFishingAttempted?.Invoke();
    public void CallFishCaptured() => OnFishCaptured?.Invoke();
    public void CallIslandReady() => OnIslandReady?.Invoke();
    public void CallPlayerMoved(HexCoord coord) => OnPlayerMoved?.Invoke(coord);

    public void CallStepsChanged(ResourceData resourceData) => OnStepsChanged?.Invoke(resourceData); 
    public void CallFishingAttemptsChanged(ResourceData resourceData) => OnFishingAttemptsChanged?.Invoke(resourceData); 

    public void CallTotalStepsWalkedChanged(int value) => OnTotalStepsWalkedChanged?.Invoke(value);
    public void CallTotalFishCapturedChanged(int value) => OnTotalFishCapturedChanged?.Invoke(value);
    public void CallTotalFishingAttemptsChanged(int value) => OnTotalFishingAttemptsChanged?.Invoke(value);

    public void SendMovementPermission(bool state) => OnSetMovementPermission?.Invoke(state);
    public void SendFishingPermission(bool state) => OnSetFishingPermission?.Invoke(state);
    public void SendRunEnded() => OnRunEnded?.Invoke();
}
