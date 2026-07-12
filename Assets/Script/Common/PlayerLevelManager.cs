using System.Numerics;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Xml.Linq;



/// <summary>
/// メタ進行（レベル・累計EXP・レベル内EXP・未使用ポイント）。ラン中はメモリのみ更新し、リザルトでセーブへ反映する。
/// </summary>
public class PlayerLevelManager : MonoBehaviour
{
    public static PlayerLevelManager Inst;
    public PlayerLevelData currentLevelData { get; private set; }
    public int currentLevel => currentLevelData.level;

    private BigInteger requestExp = 0;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }


    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
    }


    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.InGame_Ready:
                SetState_InGameReadyAsync();
                break;
            case GameStateType.InGame_End:
                SavePlayerLevelData();
                break;
            case GameStateType.Result:
                break;
            case GameStateType.ResultEnd_ToOutGame:
                break;
            case GameStateType.ResultEnd_ToIngameReady:
                break;
            case GameStateType.OutGame:
                break;
        }
    }

    private async UniTaskVoid SetState_InGameReadyAsync()
    {
        currentLevelData = await SaveLoader.Inst.Get_PlayerLevelData();
        if (currentLevelData == null)
        {
            currentLevelData = new PlayerLevelData()
            {
                level = 1,
                totalExp = 0,
                expInCurrentLevel = 0,
                points = 0
            };
            SaveLoader.Inst.Request_SavePlayerLevelData(currentLevelData);
        }
        requestExp = SOLoader.PlayerLevelData.GetPlayerLevel(currentLevelData.level).exp;
        GameEvent.PlayerLevel.PublishPlayerLevelChanged(currentLevelData.expInCurrentLevel, currentLevelData.level, requestExp);
    }


    public void AddExp(BigInteger delta)
    {
        currentLevelData.totalExp += delta;
        currentLevelData.expInCurrentLevel += delta;
        if (currentLevelData.expInCurrentLevel >= requestExp)
        {
            currentLevelData.level++;
            currentLevelData.points++;
            currentLevelData.expInCurrentLevel -= requestExp;
            requestExp = SOLoader.PlayerLevelData.GetPlayerLevel(currentLevelData.level).exp;
            GameEvent.PlayerLevel.PublishPlayerLevelUp(currentLevelData.level, currentLevelData.level);
        }
        GameEvent.PlayerLevel.PublishPlayerLevelChanged(currentLevelData.expInCurrentLevel, currentLevelData.level, requestExp);
    }
    private void SavePlayerLevelData()
    {
        SaveLoader.Inst.Request_SavePlayerLevelData(currentLevelData);
    }


    public void DEBUG_ForceLevelUp()
    {
        DEBUG_ForceLevelUpAsync().Forget();
    }

    private async UniTaskVoid DEBUG_ForceLevelUpAsync()
    {
        if (currentLevelData == null)
        {
            currentLevelData = await SaveLoader.Inst.Get_PlayerLevelData();
            if (currentLevelData == null)
            {
                currentLevelData = new PlayerLevelData()
                {
                    level = 1,
                    totalExp = 0,
                    expInCurrentLevel = 0,
                    points = 0
                };
            }
            requestExp = SOLoader.PlayerLevelData.GetPlayerLevel(currentLevelData.level).exp;
        }

        if (currentLevelData.level >= SOLoader.PlayerLevelData.maxLevel)
        {
            Debug.LogWarning("DEBUG_ForceLevelUp: max level reached");
            return;
        }

        var expToAdd = requestExp - currentLevelData.expInCurrentLevel;
        if (expToAdd <= 0) expToAdd = 1;
        AddExp(expToAdd);
        SavePlayerLevelData();
    }


}
