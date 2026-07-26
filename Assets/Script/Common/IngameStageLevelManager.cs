using UnityEngine;
using UniRx;
using System.Numerics;


/// <summary>
/// インゲーム中のステージレベル（ラン内リセット）。鉱石破壊数で進行し、短期目標を与える。
/// </summary>
public class IngameStageLevelManager : MonoBehaviour
{
    public static IngameStageLevelManager Inst;

    public int currentLevel { get; private set; } = 1;
    public int breakCountInLevel { get; private set; }
    public int breaksToNext { get; private set; }

    private bool canCount;


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
        GameEvent.InGame.GameRecordDataMod_Ingame
            .Where(x => x.Item1 == GameRecordData_Type.BlockBreakCount)
            .Subscribe(OnBlockBreakCount)
            .AddTo(this);
    }

    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.InGame_Ready:
                ResetStageLevel();
                canCount = false;
                break;
            case GameStateType.InGame:
                canCount = true;
                break;
            case GameStateType.InGame_End:
            case GameStateType.Result:
            case GameStateType.ResultEnd_ToOutGame:
            case GameStateType.ResultEnd_ToIngameReady:
            case GameStateType.OutGame:
                canCount = false;
                break;
        }
    }

    private void ResetStageLevel()
    {
        currentLevel = 1;
        breakCountInLevel = 0;
        RefreshBreaksToNext();
        PublishChanged();
    }

    private void OnBlockBreakCount((GameRecordData_Type type, BigInteger delta) record)
    {
        if (!canCount) return;
        if (currentLevel >= SOLoader.IngameStageLevelData.maxLevel) return;

        var add = record.delta > int.MaxValue ? int.MaxValue : (int)record.delta;
        if (add <= 0) add = 1;

        AddBreakCount(add);
    }

    public void AddBreakCount(int delta)
    {
        if (delta <= 0) return;
        if (currentLevel >= SOLoader.IngameStageLevelData.maxLevel) return;

        breakCountInLevel += delta;

        while (breakCountInLevel >= breaksToNext && currentLevel < SOLoader.IngameStageLevelData.maxLevel)
        {
            breakCountInLevel -= breaksToNext;
            currentLevel++;
            RefreshBreaksToNext();
            OnStageLevelUp(currentLevel);
            GameEvent.IngameStageLevel.PublishLevelUp(currentLevel);
        }

        if (currentLevel >= SOLoader.IngameStageLevelData.maxLevel)
        {
            breakCountInLevel = 0;
            breaksToNext = 0;
        }

        PublishChanged();
    }

    private void RefreshBreaksToNext()
    {
        if (currentLevel >= SOLoader.IngameStageLevelData.maxLevel)
        {
            breaksToNext = 0;
            return;
        }
        breaksToNext = SOLoader.IngameStageLevelData.GetBreaksRequired(currentLevel);
    }

    private void PublishChanged()
    {
        GameEvent.IngameStageLevel.PublishChanged(breakCountInLevel, currentLevel, breaksToNext);
    }

    /// <summary>
    /// ステージレベルアップ時の効果付与フック。具体効果は後から差し込む。
    /// </summary>
    private void OnStageLevelUp(int newLevel)
    {
        ApplyStageLevelEffects(newLevel);
    }

    /// <summary>
    /// レベルアップ時に各種効果を付与する入口。現状はフックのみ。
    /// </summary>
    protected virtual void ApplyStageLevelEffects(int newLevel)
    {
        // TODO: 攻撃力・獲得量・時間加算などの一時効果をここに追加
        Debug.Log($"[IngameStageLevel] Level Up -> {newLevel}");
    }

    public void DEBUG_ForceLevelUp()
    {
        if (currentLevel >= SOLoader.IngameStageLevelData.maxLevel)
        {
            Debug.LogWarning("DEBUG_ForceLevelUp: max stage level reached");
            return;
        }

        var need = breaksToNext - breakCountInLevel;
        if (need <= 0) need = 1;
        AddBreakCount(need);
    }
}
