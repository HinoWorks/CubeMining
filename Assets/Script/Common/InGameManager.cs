using UnityEngine;
using UniRx;
using System.Numerics;
using System.Collections.Generic;



/// <summary>
/// リザルト時のリソースデータ
/// </summary>
public class ResourceData_Result
{
    public ResourceType resourceType;
    public BigInteger resourceCount;
}

/// <summary>
/// 今回のゲーム結果データ
/// </summary>
public class GameRecordData_thisGame
{
    public BigInteger blockBreakCount;
    public BigInteger playerExp;
    public BigInteger totalDamage;
    public int Depth;
    public int treasureCount;
}
public enum GameRecordData_Type
{
    BlockBreakCount,
    PlayerExp = 1,
    Damage = 2,
    Depth = 3,
    TreasureCount = 4,
}



public class InGameManager : MonoBehaviour
{
    public static InGameManager Inst;
    [SerializeField] Transform parentPool;
    public Transform ParentPool => parentPool;


    private GameRecordData_thisGame gameRecordData_thisGame;
    private float timer = 0;
    private float timeLimit => GameParamManager.gameBaseParam.ingameTime + exTime;
    private float exTime = 0f;
    private BigInteger getCoin;
    private List<ResourceData_Result> resourceDataList = new List<ResourceData_Result>();
    public List<ResourceData_Result> Get_ResourceDataList() => resourceDataList;

    private List<int> artifactIndexList = new List<int>();
    public List<int> Get_ArtifactIndexList() => artifactIndexList;


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
        GameEvent.InGame.GameRecordDataMod_Ingame.Subscribe(Fix_GameRecordData).AddTo(this);
    }



    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.InGame_Ready:
                // インゲーム開始前の初期化
                gameRecordData_thisGame = new GameRecordData_thisGame();
                GameParamManager.Init_IngameStart();
                AttackManager.Inst.Set_Ready();
                BlockGenerateManager.Inst.Init();
                resourceDataList.Clear();
                artifactIndexList.Clear();
                exTime = 0f;
                GameEvent.UI.PublishCoinMod(getCoin);
                GameEvent.UI.PublishTimeLimit(timeLimit);
                break;

            case GameStateType.InGame:
                timer = 0;
                AttackManager.Inst.Set_AttackState(true);
                BlockGenerateManager.Inst.Set_GenerateState(true);
                break;
            case GameStateType.InGame_End:
                AttackManager.Inst.Set_AttackState(false);
                BlockGenerateManager.Inst.Set_GenerateState(false);
                Save_IngameResult();
                break;
            case GameStateType.Result:
                AttackManager.Inst.AttackUnitDelete();
                break;
            case GameStateType.ResultEnd_ToOutGame:
                BlockGenerateManager.Inst.ResetAllBlocks();
                //Save_IngameResult();
                GameWatcher.Inst.SetGameState(GameStateType.OutGame);
                break;
            case GameStateType.ResultEnd_ToIngameReady:
                BlockGenerateManager.Inst.ResetAllBlocks();
                //Save_IngameResult();
                GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
                break;
            case GameStateType.OutGame:
                break;
        }
    }

    void Update()
    {
        if (!GameWatcher.Inst.isInGameNow) return;
        timer += Time.deltaTime;
        GameEvent.UI.PublishTimeLimit(timeLimit - timer);
        if (timer >= timeLimit)
        {
            GameEvent.UI.PublishTimeLimit(0f);
            GameWatcher.Inst.SetGameState(GameStateType.InGame_End);
        }
    }

    public void AddGetCoin(BigInteger _deltaCoin)
    {
        getCoin += _deltaCoin;
        GameEvent.UI.PublishCoinMod(getCoin);
    }


    /// <summary>
    /// リソース取得
    /// </summary>
    public void AddGetResource(ResourceType _resourceType, BigInteger _deltaResource)
    {
        var targetData = resourceDataList.Find(d => d.resourceType == _resourceType);
        if (targetData == null)
        {
            targetData = new ResourceData_Result()
            {
                resourceType = _resourceType,
                resourceCount = 0
            };
            resourceDataList.Add(targetData);
        }
        targetData.resourceCount += _deltaResource;
        GameEvent.UI.PublishResourceMod_Ingame(_resourceType, targetData.resourceCount);
        //Debug.Log($"AddGetResource: {_resourceType} {targetData.resourceCount}");
    }

    /// <summary>
    /// 時間取得
    /// </summary>
    public void AddGetExTime(float _deltaExTime)
    {
        exTime += _deltaExTime;
        GameEvent.UI.PublishTimeLimit(timeLimit);
        GameEvent.UI.PublishTimeLimit(timeLimit - timer);
    }

    /// <summary>
    /// アーティファクト取得
    /// </summary>
    public void AddGetArtifact(int _artifactIndex)
    {
        artifactIndexList.Add(_artifactIndex);
        // アーティファクトはここでセーブする
        SaveLoader.Inst.Request_SaveArtifactData(_artifactIndex, 1);
        SaveLoader.Inst.Request_ArtifactCount(-1, true);
    }

    private void Save_IngameResult()
    {
        foreach (var data in resourceDataList)
        {
            SaveLoader.Inst.Request_SaveResource(data.resourceType, data.resourceCount);
        }
    }

    #region -- GameRecordData --
    private async void Save_Status()
    {
        var gameRecordData_Now = await SaveLoader.Inst.Get_GameRecordData();

        // total data -> 今回のゲーム結果を加算
        gameRecordData_Now.total_ingameCount++;
        gameRecordData_Now.total_blockBreakCount += gameRecordData_thisGame.blockBreakCount;
        gameRecordData_Now.total_playerExp += gameRecordData_thisGame.playerExp;
        gameRecordData_Now.total_totalDamage += gameRecordData_thisGame.totalDamage;
        gameRecordData_Now.total_depth += gameRecordData_thisGame.Depth;
        gameRecordData_Now.total_treasureCount += gameRecordData_thisGame.treasureCount;
        // one game data -> 今回のゲーム結果が最高値ならそれを更新
        if (gameRecordData_thisGame.blockBreakCount > gameRecordData_Now.oneGame_blockBreakCount)
        {
            gameRecordData_Now.oneGame_blockBreakCount = gameRecordData_thisGame.blockBreakCount;
        }
        if (gameRecordData_thisGame.playerExp > gameRecordData_Now.oneGame_playerExp)
        {
            gameRecordData_Now.oneGame_playerExp = gameRecordData_thisGame.playerExp;
        }
        if (gameRecordData_thisGame.totalDamage > gameRecordData_Now.oneGame_totalDamage)
        {
            gameRecordData_Now.oneGame_totalDamage = gameRecordData_thisGame.totalDamage;
        }
        SaveLoader.Inst.Request_SaveGameRecordData(gameRecordData_Now);
    }

    private void Fix_GameRecordData((GameRecordData_Type type, BigInteger delta) _gameRecordData)
    {
        switch (_gameRecordData.type)
        {
            case GameRecordData_Type.BlockBreakCount:
                gameRecordData_thisGame.blockBreakCount += _gameRecordData.delta;
                break;
            case GameRecordData_Type.PlayerExp:
                gameRecordData_thisGame.playerExp += _gameRecordData.delta;
                break;
            case GameRecordData_Type.Damage:
                gameRecordData_thisGame.totalDamage += _gameRecordData.delta;
                break;
            case GameRecordData_Type.Depth:
                gameRecordData_thisGame.Depth = (int)_gameRecordData.delta;
                break;
            case GameRecordData_Type.TreasureCount:
                gameRecordData_thisGame.treasureCount += (int)_gameRecordData.delta;
                break;
        }
    }
    #endregion


}
