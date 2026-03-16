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
        Set_ArtifactEffect();
    }

    public void Set_5secIntervalCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Interval_5sec) return;
        Set_ArtifactEffect();
    }
    public void Set_PickaxeAttackTimingCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Interval_attackPickaxe) return;
        Set_ArtifactEffect();
    }
    public void Set_underGround_5TimingCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Interval_underGround_5) return;
        Set_ArtifactEffect();
    }
    public void Set_BlockBreak_25TimingCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.Interval_breakBlock_25) return;
        Set_ArtifactEffect();
    }
    public void Set_LastBoosterCheck()
    {
        if (so.activeCheckTiming != ActiveCheckTiming.LastBooster) return;
        Set_ArtifactEffect();
    }


    private void Set_ArtifactEffect()
    {
        switch (so.effectType)
        {
            case ArtifactEffectType.pickaxe_damage:
                ArtifactManager.Inst.pickaxe_damage += (int)so.value;
                break;
            case ArtifactEffectType.pickaxe_attackInterval:
                ArtifactManager.Inst.pickaxe_attackInterval += so.value;
                break;
            case ArtifactEffectType.pickaxe_criticalRate:
                ArtifactManager.Inst.pickaxe_criticalRate += so.value;
                break;
            case ArtifactEffectType.pickaxe_resourceUpRate:
                ArtifactManager.Inst.pickaxe_resourceUpRate += so.value;
                break;
            case ArtifactEffectType.pickaxe_size:
                ArtifactManager.Inst.pickaxe_size += so.value;
                break;
            case ArtifactEffectType.all_damage:
                ArtifactManager.Inst.all_damage += (int)so.value;
                break;
            case ArtifactEffectType.all_attackInterval:
                ArtifactManager.Inst.all_attackInterval += so.value;
                break;
        }
    }
}




public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager Inst;

    private List<ArtifactControllUnit> artifactControllUnitList = new List<ArtifactControllUnit>();


    // fix parameter
    public int pickaxe_damage = 0;
    public float pickaxe_attackInterval = 0f;
    public float pickaxe_criticalRate = 0f;
    public float pickaxe_resourceUpRate = 0f;
    public float pickaxe_size = 0f;
    public int all_damage = 0;
    public float all_attackInterval = 0f;
    public int bomb_damage = 0;
    public float bomb_size = 0f;
    public int create_bomb = 0;
    public int create_miniPickaxe = 0;
    public int create_bonusChest = 0;
    public float changeBlockRate = 0f;
    public float get_ingameTime = 0f;



    // 最後の5秒間のチェック
    private bool isLastBoosterCheckFin = false;
    private float lastBoosterCheckTime = 5f;

    // 5秒間隔のチェック
    private float timer_for5secInterval = 0f;

    // 新しい地面レイヤーに到達
    private int groundLayerIndex = 0;
    private int groundLayerInterval = 5;
    private int groundLayerTarget = 0;


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
        GameEvent.InGame.OnPickaxeAttack.Subscribe(_ => Check_PickaxeAttackTiming()).AddTo(this);
        GameEvent.InGame.OnNewGroundLayer.Subscribe(layer => Check_NewGroundLayer(layer)).AddTo(this);
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
        isLastBoosterCheckFin = false;
        timer_for5secInterval = 0f;

        // レイヤーチェック用
        groundLayerIndex = 1;
        groundLayerTarget = groundLayerIndex + groundLayerInterval;

        // 装備中のアーティファクトセット、
        artifactControllUnitList.Clear();
        for (int i = 0; i < StaticManager.artifactSlotCount; i++)
        {
            var saveData = await SaveLoader.Inst.Get_ArtifactSlotData(i);
            if (saveData == null) continue;
            var artifactData = SOLoader.ArtifactData.artifactDatas[saveData.equipedArtifactIndex];
            var artifactCont = new ArtifactControllUnit();
            artifactCont.Init(artifactData);
            artifactControllUnitList.Add(artifactCont);
        }
        Check_InitialSet();
    }
    private void SetState_InGame()
    {
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
        Debug.Log("=ArtifactManager=   Check_5secInterval");
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
    /// 地下5層ごとのチェック
    /// </summary>
    private void Check_NewGroundLayer(int layer)
    {
        if (layer < groundLayerTarget) return;

        Debug.Log("=ArtifactManager=   Check_NewGroundLayer: " + layer);
        groundLayerIndex++;
        groundLayerTarget = groundLayerIndex * groundLayerInterval;
        foreach (var artifactCont in artifactControllUnitList)
        {
            artifactCont.Set_underGround_5TimingCheck();
        }
    }
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

}
