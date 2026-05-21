using UnityEngine;
using UniRx;
using System.Numerics;
using Cysharp.Threading.Tasks;


public class PickaxePowerManager : MonoBehaviour
{
    public static PickaxePowerManager Inst;

    public int EquippedIndex { get; private set; }
    public int EquippedLevel { get; private set; }
    public PickaxePowerBase EquippedBase { get; private set; }
    public PickaxePowerLevel EquippedLevelData { get; private set; }

    public int CurrentGauge { get; private set; }
    public int MaxGauge { get; private set; }
    public bool IsGaugeReady => IsActive && MaxGauge > 0 && CurrentGauge >= MaxGauge;
    public float GaugeRate => MaxGauge > 0 ? (float)CurrentGauge / MaxGauge : 0f;
    public bool IsActive { get; private set; }

    public float CooldownDuration { get; private set; }
    public float CooldownRemaining { get; private set; }
    public bool IsOnCooldown => CooldownRemaining > 0f;
    public float CooldownRate => CooldownDuration > 0f ? 1f - (CooldownRemaining / CooldownDuration) : 1f;
    public bool CanActivate => IsGaugeReady && !IsOnCooldown;

    private float coolTime_test = 5f;
    private int blockCountMax_test = 20;

    private bool canAccumulateGauge;


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
                SetState_InGameReady().Forget();
                break;
            case GameStateType.InGame:
                SetState_InGame();
                break;
            case GameStateType.InGame_End:
            case GameStateType.Result:
            case GameStateType.ResultEnd_ToOutGame:
            case GameStateType.ResultEnd_ToIngameReady:
                SetState_InGameEnd();
                break;
        }
    }


    private async UniTaskVoid SetState_InGameReady()
    {
        CurrentGauge = 0;
        CooldownDuration = 0f;
        CooldownRemaining = 0f;
        canAccumulateGauge = false;
        IsActive = false;
        EquippedBase = null;
        EquippedLevelData = null;
        MaxGauge = 0;

        EquippedIndex = SaveLoader.Inst.PickaxePowerEquipedIndex;
        if (EquippedIndex <= 0) return;

        var saveData = await SaveLoader.Inst.Get_PickaxePowerData(EquippedIndex);
        EquippedLevel = saveData == null ? 0 : saveData.level;
        if (EquippedLevel <= 0) return;


        EquippedBase = SOLoader.PickaxePowerData.GetPickaxePowerBase(EquippedIndex);
        EquippedLevelData = SOLoader.PickaxePowerData.GetPickaxePowerLevel(EquippedIndex, EquippedLevel);

        MaxGauge = blockCountMax_test;

        IsActive = true;
        CooldownDuration = coolTime_test;

        if (GameWatcher.Inst != null && GameWatcher.Inst.isInGameNow)
        {
            canAccumulateGauge = true;
        }
    }

    private void SetState_InGame()
    {
        canAccumulateGauge = IsActive;
    }

    private void SetState_InGameEnd()
    {
        canAccumulateGauge = false;
        CooldownRemaining = 0f;
    }


    void Update()
    {
        if (!IsOnCooldown) return;
        if (GameWatcher.Inst == null || !GameWatcher.Inst.isInGameNow) return;

        CooldownRemaining -= Time.deltaTime;
        if (CooldownRemaining <= 0f)
        {
            CooldownRemaining = 0f;
            Debug.Log(" ================== PickaxePower cooldown end ");
        }
    }


    private void OnBlockBreakCount((GameRecordData_Type type, BigInteger delta) record)
    {
        if (!canAccumulateGauge) return;
        if (!IsActive || MaxGauge <= 0) return;
        if (IsOnCooldown) return;

        var add = record.delta > int.MaxValue ? int.MaxValue : (int)record.delta;
        if (add <= 0) add = 1;

        CurrentGauge = Mathf.Min(CurrentGauge + add, MaxGauge);
    }


    /// <summary>
    /// ゲージ満タン時に右クリックから呼ぶ。成功時はゲージを0に戻す。
    /// </summary>
    public bool TryActivate()
    {
        if (GameWatcher.Inst == null || !GameWatcher.Inst.isInGameNow) return false;
        if (!CanActivate) return false;


        Debug.Log(" ================== PickaxePower activated ");
        ExecuteEffect();
        CurrentGauge = 0;
        StartCooldown();
        return true;
    }

    private void StartCooldown()
    {
        CooldownDuration = coolTime_test;
        CooldownRemaining = CooldownDuration;
    }

    private void ExecuteEffect()
    {
        // TODO: index / effectType に応じた効果
    }
}
