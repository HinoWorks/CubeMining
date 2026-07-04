using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Threading.Tasks;


[System.Serializable]
public class ArtifactControllUnit
{
    public ArtifactUnitData so;


    public void Init(ArtifactUnitData _so)
    {
        so = _so;
    }
    public void InitialSet()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Passive) return;
        ActiveCheck();
    }
    public void Set_InGameStart()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.StartIngame) return;
        ActiveCheck();
    }

    public void Set_5secIntervalCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Interval_5sec) return;
        ActiveCheck();
    }
    public void Set_PickaxeAttackTimingCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Interval_attackPickaxe) return;
        ActiveCheck();
    }

    public void Set_BlockBreak_25TimingCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Interval_breakBlock_25) return;
        ActiveCheck();
    }
    public void Set_LastBoosterCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.LastBooster) return;
        ActiveCheck();
    }
    private void ActiveCheck()
    {
        var randomValue = Random.Range(0f, 1f);
        Debug.Log("=ArtifactManager=   ActiveCheck / randomValue:" + randomValue + " / so.activeCheckRate:" + so.activeCheckRate);
        if (so.activeCheckRate >= 0f && randomValue >= so.activeCheckRate) return;
        Set_ArtifactEffect(so.effectType, so.value);
        Set_ArtifactEffect(so.effectType_2, so.value_2);
        GameEvent.InGame.PublishArtifactActiveEffect(so.artifactIndex);
    }

    private void Set_ArtifactEffect(ArtifactEffectType _effectType, float _value)
    {
        if (_effectType == ArtifactEffectType.None) return;
        switch (_effectType)
        {
            case ArtifactEffectType.pickaxe_damage:
                ArtifactManager.Inst.pickaxe_damageRate += _value;
                break;
            case ArtifactEffectType.pickaxe_attackInterval:
                ArtifactManager.Inst.pickaxe_attackInterval += _value;
                break;
            case ArtifactEffectType.pickaxe_criticalRate:
                ArtifactManager.Inst.pickaxe_criticalRate += _value;
                break;
            case ArtifactEffectType.pickaxe_resourceUpRate:
                ArtifactManager.Inst.pickaxe_resourceUpRate += _value;
                break;
            case ArtifactEffectType.pickaxe_size:
                ArtifactManager.Inst.pickaxe_sizeRate += _value;
                break;

            // -- ボムの効果 --
            case ArtifactEffectType.bomb_damage:
                ArtifactManager.Inst.bomb_damageRate += _value;
                break;
            case ArtifactEffectType.bomb_size:
                ArtifactManager.Inst.bomb_sizeRate += _value;
                break;

            // -- ピッケル以外 --
            case ArtifactEffectType.all_damage:
                ArtifactManager.Inst.all_damageRate += _value;
                break;
            case ArtifactEffectType.all_attackInterval:
                ArtifactManager.Inst.all_attackInterval += _value;
                break;

            // -- 共通の効果 --
            case ArtifactEffectType.changeBlockRate:
                ArtifactManager.Inst.changeBlockRate += _value;
                break;
            case ArtifactEffectType.blockBreakRate:
                ArtifactManager.Inst.instantShatterRate += _value;
                break;
            case ArtifactEffectType.resourceUpRate:
                ArtifactManager.Inst.resourceUpRate += _value;
                break;

            // -- 生成 --
            case ArtifactEffectType.create_bomb:
                ArtifactManager.Inst.Create_Bomb();
                break;
            case ArtifactEffectType.create_bonusChest:
                ArtifactManager.Inst.Create_BonusChest();
                break;
            case ArtifactEffectType.create_timeBlock:
                ArtifactManager.Inst.Create_TimeBlock();
                break;
            case ArtifactEffectType.createOre_atGetTime:
                ArtifactManager.Inst.CreateOre_atGetTime((int)_value);
                break;
            case ArtifactEffectType.bonusTimeAdd_atBreakChest:
                ArtifactManager.Inst.BonusTimeAdd_atBreakChest(_value);
                break;

            // -- インゲーム時間追加 --
            case ArtifactEffectType.get_ingameTime:
                InGameManager.Inst.AddGetExTime(_value);
                break;
        }
    }
}




public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager Inst;

    private List<ArtifactControllUnit> artifactControllUnitList = new List<ArtifactControllUnit>();


    // ==== fix parameter ====
    public float pickaxe_damageRate = 0;
    public float pickaxe_attackInterval = 0f;
    public float pickaxe_criticalRate = 0f;
    public float pickaxe_resourceUpRate = 0f;
    public float pickaxe_sizeRate = 0f;

    [Space(5)]
    public float all_damageRate = 0;
    public float all_attackInterval = 0f;

    [Space(5)]
    public float bomb_damageRate = 0;
    public float bomb_sizeRate = 0f;

    [Space(5)]
    public float changeBlockRate = 0f;
    public float resourceUpRate = 0f;
    public float instantShatterRate = 0f;

    [Space(5)]
    public bool isIngameTimeAdd_atBreakChest = false;
    public float ingameTimeAdd_atBreakChest_value = 0.5f;
    public bool isCreateOre_atGetTime = false;
    private int createOre_count = 3;


    // =================


    // 最後の5秒間のチェック
    private bool isLastBoosterCheckFin = false;
    private float lastBoosterCheckTime = 5f;

    // 5秒間隔のチェック
    private float timer_for5secInterval = 0f;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
        GameEvent.InGame.OnPickaxeAttack.Subscribe(_ => Check_PickaxeAttackTiming()).AddTo(this);
        GameEvent.InGame.IngameTimeAdd.Subscribe(IngameEvent_IngameTimeAdd).AddTo(this);
        GameEvent.InGame.GameRecordDataMod_Ingame.Subscribe(data => IngameEvent_IngameTimeAdd_atBreakChest(data.Item1, (int)data.Item2)).AddTo(this);
    }

    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.InGame_Ready:
                SetState_InGameReady();
                break;
            case GameStateType.InGame:
                SetState_InGame();
                break;
            case GameStateType.InGame_End:
                SetState_InGameEnd();
                break;
            case GameStateType.Result:
            case GameStateType.ResultEnd_ToOutGame:
            case GameStateType.ResultEnd_ToIngameReady:
            case GameStateType.OutGame:
                break;
        }
    }


    void Update()
    {
        if (!GameWatcher.Inst.isInGameNow) return;

        timer_for5secInterval += Time.deltaTime;
        if (timer_for5secInterval >= 5f)
        {
            Check_5secInterval();
            timer_for5secInterval = 0f;
        }

        if (InGameManager.Inst.RemainingTime <= lastBoosterCheckTime)
        {
            if (isLastBoosterCheckFin) return;
            isLastBoosterCheckFin = true;
            Check_LastBooster();
        }
    }



    #region -- SetState --
    private async void SetState_InGameReady()
    {

        pickaxe_damageRate = 0f;
        pickaxe_attackInterval = 0f;
        pickaxe_criticalRate = 0f;
        pickaxe_resourceUpRate = 0f;
        pickaxe_sizeRate = 0f;

        all_damageRate = 0f;
        all_attackInterval = 0f;

        bomb_damageRate = 0f;
        bomb_sizeRate = 0f;

        changeBlockRate = 0f;
        resourceUpRate = 0f;
        instantShatterRate = 0f;

        isIngameTimeAdd_atBreakChest = false;
        isCreateOre_atGetTime = false;

        isLastBoosterCheckFin = false;
        timer_for5secInterval = 0f;

        // 装備中のアーティファクトセット、
        artifactControllUnitList.Clear();
        for (int i = 1; i < StaticManager.artifactSlotCount + 1; i++)
        {
            var saveData = await SaveLoader.Inst.Get_ArtifactSlotData(i);
            if (saveData == null) continue;

            var artifactData = SOLoader.ArtifactData.Get_ArtifactData(saveData.equipedArtifactIndex);
            if (artifactData == null) continue;
            var artifactCont = new ArtifactControllUnit();
            artifactCont.Init(artifactData);
            artifactControllUnitList.Add(artifactCont);
            Debug.Log($"アーティファクトセット中 == SlotIndex:{saveData.slotIndex} / {saveData.equipedArtifactIndex}");
        }
        Check_InitialSet();
    }
    private void SetState_InGame()
    {
        foreach (var artifactCont in artifactControllUnitList)
        {
            artifactCont.Set_InGameStart();
        }
    }

    private void SetState_InGameEnd()
    {

    }
    #endregion


    /// <summary>
    /// 主にパッシブ効果
    /// </summary>
    private void Check_InitialSet()
    {
        foreach (var artifactCont in artifactControllUnitList)
        {
            artifactCont.InitialSet();
        }
    }

    /// <summary>
    /// 5秒間隔のチェック
    /// </summary>
    private void Check_5secInterval()
    {
        foreach (var artifactCont in artifactControllUnitList)
        {
            artifactCont.Set_5secIntervalCheck();
        }
    }

    /// <summary>
    /// ピッケル攻撃時のチェック
    /// </summary>
    private void Check_PickaxeAttackTiming()
    {
        foreach (var artifactCont in artifactControllUnitList)
        {
            artifactCont.Set_PickaxeAttackTimingCheck();
        }
    }

    /// <summary>
    /// 最後のブースター効果
    /// </summary>
    private void Check_LastBooster()
    {
        Debug.Log("=ArtifactManager=   Check_LastBooster");
        foreach (var artifactCont in artifactControllUnitList)
        {
            artifactCont.Set_LastBoosterCheck();
        }
    }

    public void Create_Bomb()
    {
        BlockGenerateManager.Inst.Create_Bomb();
    }
    public void Create_BonusChest()
    {
        BlockGenerateManager.Inst.Create_BonusChest();
    }
    public void Create_TimeBlock()
    {
        BlockGenerateManager.Inst.Create_Timer();
    }

    /// <summary>
    /// チェスト破壊時にインゲーム時間を追加するフラグON
    /// </summary>
    public void BonusTimeAdd_atBreakChest(float _addTime)
    {
        isIngameTimeAdd_atBreakChest = true;
        ingameTimeAdd_atBreakChest_value = _addTime;
    }
    /// <summary>
    /// チェスト破壊時にインゲーム時間を追加する
    /// </summary>
    private void IngameEvent_IngameTimeAdd_atBreakChest(GameRecordData_Type gameRecordData_Type, int _count)
    {
        if (gameRecordData_Type != GameRecordData_Type.TreasureCount) return;
        if (!isIngameTimeAdd_atBreakChest) return;
        InGameManager.Inst.AddGetExTime(ingameTimeAdd_atBreakChest_value);
    }

    /// <summary>
    /// インゲーム時間取得時に鉱石を生成するフラグON
    /// </summary>
    public void CreateOre_atGetTime(int _count)
    {
        isCreateOre_atGetTime = true;
        createOre_count = _count;
    }

    /// <summary>
    /// インゲーム時間追加時のイベント = フラグ確認して、鉱石を生成する
    /// </summary>
    /// <param name="time"></param>
    private void IngameEvent_IngameTimeAdd(float time)
    {
        if (!isCreateOre_atGetTime) return;
        BlockGenerateManager.Inst.CreateBlock(createOre_count);
    }

}
