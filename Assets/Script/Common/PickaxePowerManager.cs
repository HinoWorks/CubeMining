using UnityEngine;
using UniRx;
using System.Numerics;
using Cysharp.Threading.Tasks;
using System;


public class PickaxePowerManager : MonoBehaviour
{
    public static PickaxePowerManager Inst;

    public int EquippedIndex { get; private set; }
    public int EquippedLevel { get; private set; }
    public PickaxePowerBase EquippedBase { get; private set; }
    public PickaxePowerLevel EquippedLevelData { get; private set; }
    private PickaxePowerCont_Base pickaxePowerCont;
    private int CT => EquippedBase.CD;
    private int blockCountNeeded => EquippedBase.blockCount;


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
    private bool canAccumulateGauge;



    // ゲージ変更イベント
    private Subject<float> powerGaugeRateChanged = new Subject<float>();
    public IObservable<float> PowerGaugeRateChanged => powerGaugeRateChanged.AsObservable();
    private void PublishPowerGaugeRateChanged(float rate)
    {
        powerGaugeRateChanged.OnNext(rate);
    }
    // スキル発動
    private Subject<(int, int)> powerActivate = new Subject<(int, int)>();
    public IObservable<(int, int)> PowerActivate => powerActivate.AsObservable();
    private void PublishPowerActivate(int index, int CT)
    {
        powerActivate.OnNext((index, CT));
    }

    // Power UP param for Pickaxe
    public float pickaxeAttackDamageRate { get; private set; } = 0f;
    public float pickaxeAttackIntervalRate { get; private set; } = 0f;
    public float pickaxeSizeRate { get; private set; } = 0f;

    private Subject<Unit> pickaxePowerParamChanged = new Subject<Unit>();
    public IObservable<Unit> PickaxePowerParamChanged => pickaxePowerParamChanged.AsObservable();
    private void PublishPickaxePowerParamChanged()
    {
        pickaxePowerParamChanged.OnNext(Unit.Default);
    }

    public void ApplyPickaxePowerBuff(float damageRate, float intervalRate, float sizeRate)
    {
        pickaxeAttackDamageRate = damageRate;
        pickaxeAttackIntervalRate = intervalRate;
        pickaxeSizeRate = sizeRate;
        PublishPickaxePowerParamChanged();
    }

    public void EndPickaxePowerBuff()
    {
        if (pickaxeAttackDamageRate == 0f && pickaxeAttackIntervalRate == 0f && pickaxeSizeRate == 0f) return;
        pickaxeAttackDamageRate = 0f;
        pickaxeAttackIntervalRate = 0f;
        pickaxeSizeRate = 0f;
        PublishPickaxePowerParamChanged();
    }








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

        // pickaxePower param reset
        pickaxeAttackDamageRate = 0f;
        pickaxeAttackIntervalRate = 0f;
        pickaxeSizeRate = 0f;


        EquippedIndex = SaveLoader.Inst.PickaxePowerEquipedIndex;
        if (EquippedIndex <= 0) return;

        var saveData = await SaveLoader.Inst.Get_PickaxePowerData(EquippedIndex);
        EquippedLevel = saveData == null ? 0 : saveData.level;
        if (EquippedLevel <= 0) return;


        EquippedBase = SOLoader.PickaxePowerData.GetPickaxePowerBase(EquippedIndex);
        EquippedLevelData = SOLoader.PickaxePowerData.GetPickaxePowerLevel(EquippedIndex, EquippedLevel);
        var newPowerUnit = Instantiate(EquippedBase.pf, transform) as GameObject;
        pickaxePowerCont = newPowerUnit.GetComponent<PickaxePowerCont_Base>();
        pickaxePowerCont.Init(EquippedLevelData);

        MaxGauge = blockCountNeeded;

        IsActive = true;
        CooldownDuration = CT;

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
        PublishPowerGaugeRateChanged(GaugeRate);
    }


    /// <summary>
    /// ゲージ満タン時に右クリックから呼ぶ。成功時はゲージを0に戻す。
    /// </summary>
    public bool TryActivate()
    {
        if (GameWatcher.Inst == null || !GameWatcher.Inst.isInGameNow) return false;
        if (!CanActivate) return false;

        pickaxePowerCont.Activate();
        PublishPowerActivate(EquippedIndex, CT);
        ExecuteEffect();
        CurrentGauge = 0;
        StartCooldown();
        return true;
    }

    private void StartCooldown()
    {
        CooldownDuration = CT;
        CooldownRemaining = CooldownDuration;
    }

    private void ExecuteEffect()
    {
        // TODO: index / effectType に応じた効果
    }
}
