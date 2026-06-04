using UnityEngine;
using UniRx;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


public class UnlockStateManager : MonoBehaviour
{
    public static UnlockStateManager Inst;
    private int currentPlayerLevel;

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
            case GameStateType.Title:
                Init_UnlockCheck().Forget();
                break;
            case GameStateType.OutGame:
                Update_UnlockCheck().Forget();
                break;
        }
    }



    private async UniTask Init_UnlockCheck()
    {
        var currentLevelData = await SaveLoader.Inst.Get_PlayerLevelData();
        currentPlayerLevel = currentLevelData == null ? 1 : currentLevelData.level;
        var unlockedDatas = SOLoader.UnlockData.Get_UnlockData_UnderLevel(currentPlayerLevel);
        foreach (var unlockData in unlockedDatas)
        {
            Set_Unlock(unlockData.unlockTargetType, false);
        }
    }

    private async UniTask Update_UnlockCheck()
    {
        var currentLevelData = await SaveLoader.Inst.Get_PlayerLevelData();
        var isChangeState = currentPlayerLevel != (currentLevelData == null ? 1 : currentLevelData.level);
        if (!isChangeState) return;

        currentPlayerLevel = currentLevelData == null ? 1 : currentLevelData.level;
        var unlockedDatas = SOLoader.UnlockData.Get_UnlockData_UnderLevel(currentPlayerLevel);
        foreach (var unlockData in unlockedDatas)
        {
            Set_Unlock(unlockData.unlockTargetType, true);
        }
    }

    private void Set_Unlock(UnlockTargetType _targetType, bool _isFirstUnlockEvent)
    {
        switch (_targetType)
        {
            case UnlockTargetType.SkillTree:
                UIManager_OutGame.Inst.Set_HeaderButtonActiveState(OutGame_MenuType.SkillTree, _isFirstUnlockEvent);
                break;
            case UnlockTargetType.Artifact:
                break;
            case UnlockTargetType.PickaxeCraft:
                UIManager_OutGame.Inst.Set_HeaderButtonActiveState(OutGame_MenuType.Pickaxe, _isFirstUnlockEvent);
                break;
            case UnlockTargetType.PickaxePower:
                UIManager_OutGame.Inst.Set_HeaderButtonActiveState(OutGame_MenuType.PickaxePower, _isFirstUnlockEvent);
                break;
        }
    }






    /*
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
