using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class UnlockStateManager : MonoBehaviour
{
    public static UnlockStateManager Inst;
    public UnlockData targetEventData { get; private set; }

    [Header("確認用 -- のちに削除 --")]
    [SerializeField] private List<UnlockTargetType> list_unlockTargetType = new List<UnlockTargetType>();

    /*
        void Awake()
        {
            if (Inst == null) { Inst = this; }
            else { Destroy(this); }
        }

        void Start()
        {
            GameEvent.GameState.SetGameState.Subscribe(ChangeGameState).AddTo(this);

            list_unlockTargetType.Clear();
            foreach (var unlockData in SOLoader.UnlockData.unlockDatas)
            {
                if (SaveLoader.Inst.UnlockEventIndex <= unlockData.eventIndex) continue;
                var isUnlock = Check_UnlockState(unlockData.unlockCheckType, unlockData.checkCount);
                if (isUnlock)
                {
                    list_unlockTargetType.Add(unlockData.unlockTargetType);
                }
            }
        }

        private bool Check_UnlockState(UnlockCheckType _targetType, int _checkCount)
        {
            switch (_targetType)
            {
                case UnlockCheckType.GamePlayCount:
                    return SaveLoader.Inst.IngameCount >= _checkCount;
                case UnlockCheckType.PlayerLevel:
                    return SaveLoader.Inst.PlayerLevel >= _checkCount;
                case UnlockCheckType.BlockBreakCount:
                    return SaveLoader.Inst.BlockCount >= _checkCount;
                default:
                    return false;
            }
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
            targetEventData = SOLoader.UnlockData.Get_UnlockData(SaveLoader.Inst.UnlockEventIndex);
            if (targetEventData == null)
            {
                Debug.LogError($"UnlockData is not found: {SaveLoader.Inst.UnlockEventIndex}");
                return;
            }
            var isUnlock = Check_UnlockState(targetEventData.unlockCheckType, targetEventData.checkCount);
            if (isUnlock)
            {
                list_unlockTargetType.Add(targetEventData.unlockTargetType);
                SaveLoader.Inst.Request_SaveUnlockEventIndex(SaveLoader.Inst.UnlockEventIndex + 1);
            }
        }

        public void UnlockCheck(UnlockTargetType _targetType)
        {
            if (list_unlockTargetType.Contains(_targetType)) return;
        }
        */
}
