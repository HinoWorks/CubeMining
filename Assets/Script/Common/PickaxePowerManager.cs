using UnityEngine;
using UniRx;
using System.Numerics;
using Cysharp.Threading.Tasks;
using System;


public class PickaxePowerManager : MonoBehaviour
{
    public static PickaxePowerManager Inst;

    /// <summary>
    /// true: ブロック破壊数でチャージ（旧仕様）
    /// false: PickaxePowerLevel.useCount 分だけインゲーム中に使用可能（現行）
    /// </summary>
    private static readonly bool UseBlockChargeMode = false;

    public int EquippedIndex { get; private set; }
    public int EquippedLevel { get; private set; }
    public PickaxePowerBase EquippedBase { get; private set; }
    public PickaxePowerLevel EquippedLevelData { get; private set; }
    private PickaxePowerCont_Base pickaxePowerCont;
    private int CT => EquippedBase.CD;
    private int blockCountNeeded => EquippedBase.blockCount;


    public int CurrentGauge { get; private set; }
    public int MaxGauge { get; private set; }
    public bool IsGaugeReady => UseBlockChargeMode
        ? (IsActive && MaxGauge > 0 && CurrentGauge >= MaxGauge)
        : (IsActive && CurrentGauge > 0);
    public float GaugeRate
    {
        get
        {
            if (UseBlockChargeMode)
                return MaxGauge > 0 ? (float)CurrentGauge / MaxGauge : 0f;
            // useCountモード: 残回数があればUI上は満タン扱い
            return CurrentGauge > 0 ? 1f : 0f;
        }
    }
    public bool IsActive { get; private set; }

    public float CooldownDuration { get; private set; }
    public float CooldownRemaining { get; private set; }
    public bool IsOnCooldown => CooldownRemaining > 0f;
    public float CooldownRate => CooldownDuration > 0f ? 1f - (CooldownRemaining / CooldownDuration) : 1f;
    public bool CanActivate => IsGaugeReady && !IsOnCooldown;
    private bool canAccumulateGauge;

    [Space(10)]
    [Header("-- DEBUG --")]
    [SerializeField] bool DEBUG_InitialPowerReady = false;


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

        if (UseBlockChargeMode)
        {
            MaxGauge = blockCountNeeded;
            CurrentGauge = 0;
        }
        else
        {
            // useCount分だけインゲーム中に使用可能。開始時に残回数をセット
            MaxGauge = Mathf.Max(0, (int)EquippedLevelData.value_4);
            CurrentGauge = MaxGauge;
        }

        IsActive = true;
        CooldownDuration = CT;

        if (GameWatcher.Inst != null && GameWatcher.Inst.isInGameNow)
        {
            canAccumulateGauge = UseBlockChargeMode;
        }

        if (DEBUG_InitialPowerReady)
        {
            Debug.Log(" <green>================== DEBUG_InitialPowerReady </green> ");
            CurrentGauge = MaxGauge;
        }

        PublishPowerGaugeRateChanged(GaugeRate);
    }

    private void SetState_InGame()
    {
        canAccumulateGauge = UseBlockChargeMode && IsActive;
    }

    private void SetState_InGameEnd()
    {
        canAccumulateGauge = false;
        CooldownRemaining = 0f;
        pickaxePowerCont?.GameEndCall();
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

            // useCountモード: CD終了後に残回数があれば再度Ready表示
            if (!UseBlockChargeMode && CurrentGauge > 0)
            {
                PublishPowerGaugeRateChanged(GaugeRate);
            }
        }
    }


    private void OnBlockBreakCount((GameRecordData_Type type, BigInteger delta) record)
    {
        // useCountモード時はブロック破壊チャージの動線を塞ぐ（旧仕様復帰時は UseBlockChargeMode = true）
        if (!UseBlockChargeMode) return;

        if (!canAccumulateGauge) return;
        if (!IsActive || MaxGauge <= 0) return;
        if (IsOnCooldown) return;

        var add = record.delta > int.MaxValue ? int.MaxValue : (int)record.delta;
        if (add <= 0) add = 1;

        CurrentGauge = Mathf.Min(CurrentGauge + add, MaxGauge);
        PublishPowerGaugeRateChanged(GaugeRate);
    }


    /// <summary>
    /// ゲージ満タン時（または残useCountあり）に右クリックから呼ぶ。
    /// </summary>
    public bool TryActivate()
    {
        if (GameWatcher.Inst == null || !GameWatcher.Inst.isInGameNow) return false;
        if (!CanActivate) return false;

        pickaxePowerCont.Activate();
        ExecuteEffect();

        if (UseBlockChargeMode)
        {
            CurrentGauge = 0;
            PublishPowerGaugeRateChanged(GaugeRate);
        }
        else
        {
            CurrentGauge = Mathf.Max(0, CurrentGauge - 1);
            // UIは Set_PowerActivate 側でゲージをリセット。CD終了時に残回数があれば再通知
        }

        PublishPowerActivate(EquippedIndex, CT);
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
