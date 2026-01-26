using UnityEngine;
using UniRx;

public class UnlockStateManager : MonoBehaviour
{
    public static UnlockStateManager Inst;
    public GameEventUnitData targetEventData { get; private set; }


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(ChangeGameState).AddTo(this);
    }

    private void ChangeGameState(GameStateType _state)
    {
        switch (_state)
        {
            case GameStateType.InGame_Ready:
            case GameStateType.OutGame:
                CheckUnlockState();
                break;
        }
    }
    private void CheckUnlockState()
    {
        targetEventData = SOLoader.GameEventData.Get_GameEventData(SaveLoader.Inst.UnlockEventIndex);
        if (targetEventData == null)
        {
            Debug.LogError($"GameEventUnitData is not found: {SaveLoader.Inst.UnlockEventIndex}");
            return;
        }
        switch (targetEventData.eventCheckType)
        {
            case EventCheckType.GamePlayCount:

                break;
            case EventCheckType.PlayerLevel:
                break;
            case EventCheckType.BlockBreakCount:
                break;
        }
    }
}
