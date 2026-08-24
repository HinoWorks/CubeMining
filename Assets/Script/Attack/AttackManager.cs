using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Inst;
    [SerializeField] List<AttackCont_Pickaxe> pickaxeConts = new List<AttackCont_Pickaxe>();
    [SerializeField] List<AttackContBase> attackConts = new List<AttackContBase>();
    [SerializeField] PickaxeAnimCont pickaxeAnimCont;
    private bool isAttacking = false;
    private int currentPickaxeIndex = 0;
    public int currentPickaxeDamage => pickaxeConts[currentPickaxeIndex].baseDamage;
    public Vector3 currentPickaxePosition => pickaxeConts[currentPickaxeIndex].pickaxePosition;
    private int[] slotIndexes = { 0 };

    private readonly Subject<Unit> onPickaxeAttackTiming = new();
    public IObservable<Unit> OnPickaxeAttackTiming => onPickaxeAttackTiming.AsObservable();


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); return; }
        onPickaxeAttackTiming.AddTo(this);
        if (pickaxeAnimCont == null)
            pickaxeAnimCont = GetComponentInChildren<PickaxeAnimCont>(true);
    }

    void Start()
    {
        GameEvent.Input.PointerPrimaryDown.Subscribe(_ => Click_LeftButton()).AddTo(this);
        GameEvent.Input.PointerSecondaryDown.Subscribe(_ => Click_RightButton()).AddTo(this);

        /*
         if (PickaxePowerManager.Inst != null)
         {
             PickaxePowerManager.Inst.PickaxePowerParamChanged
                 .Subscribe(_ => RefreshPickaxeAttackLoops())
                 .AddTo(this);
         }
         */
    }


    public async void Set_Ready()
    {
        isAttacking = false;

        // 装備中のピッケルを生成
        foreach (var slotIndex in slotIndexes)
        {
            var pickaxeSlotData = await SaveLoader.Inst.Get_PickaxeSlotData(slotIndex);
            if (pickaxeSlotData == null || pickaxeSlotData.equipedPickaxeIndex <= 0) continue;

            var pickaxeUnitData = GameParamManager.Get_PickaxeParam(pickaxeSlotData.equipedPickaxeIndex);
            if (pickaxeUnitData != null)
            {
                PickaxeUnitGenerate(slotIndex, pickaxeUnitData);
            }
        }
        // 攻撃ユニット生成 == スキルツリー分のパラメータを読み込む
        foreach (var attackParam in GameParamManager.list_attackParam)
        {
            if (!attackParam.isActive) continue;
            AttackUnitGenerate(attackParam);
        }

        currentPickaxeIndex = 0;
        foreach (var pickaxeCont in pickaxeConts)
        {
            pickaxeCont.Set_SelectPickaxe(currentPickaxeIndex);
        }
    }

    private void PickaxeUnitGenerate(int _slotIndex, PickaxeParam _pickaxeParam)
    {
        var pickaxeUnit = Instantiate(_pickaxeParam.so.pf, transform) as GameObject;
        pickaxeUnit.transform.position = transform.position;

        var pickaxeCont = pickaxeUnit.GetComponent<AttackCont_Pickaxe>();
        pickaxeConts.Add(pickaxeCont);
        pickaxeCont.Init(_slotIndex, _pickaxeParam);
        pickaxeCont.Set_AttackTrigger(isAttacking);

        EnsurePickaxeAnimCont();
        pickaxeAnimCont.SetModel(_pickaxeParam.so.pf_model);
    }

    public void NotifyPickaxeAttackTiming()
    {
        onPickaxeAttackTiming.OnNext(Unit.Default);
    }

    private void EnsurePickaxeAnimCont()
    {
        if (pickaxeAnimCont != null) return;
        pickaxeAnimCont = GetComponentInChildren<PickaxeAnimCont>(true);
        if (pickaxeAnimCont != null) return;

        var go = new GameObject("PickaxeAnim");
        go.transform.SetParent(transform);
        pickaxeAnimCont = go.AddComponent<PickaxeAnimCont>();
    }


    private void AttackUnitGenerate(AttackParam _attackParam)
    {
        var attackUnit = Instantiate(_attackParam.so.pf, transform) as GameObject;
        attackUnit.transform.position = transform.position;
        attackUnit.transform.localScale = Vector3.one;

        var attackCont = attackUnit.GetComponent<AttackContBase>();
        attackConts.Add(attackCont);
        attackCont.Init(_attackParam);
        attackCont.Set_AttackTrigger(isAttacking);
    }


    public void Set_AttackState(bool isStart)
    {
        // 攻撃開始
        isAttacking = isStart;
        foreach (var attackCont in attackConts)
        {
            attackCont.Set_AttackTrigger(isStart);
        }
        foreach (var pickaxeCont in pickaxeConts)
        {
            pickaxeCont.Set_AttackTrigger(isStart);
        }
    }


    public void AttackUnitDelete()
    {
        foreach (var attackCont in attackConts)
        {
            attackCont.OnDestroy();
        }
        attackConts.Clear();
        foreach (var pickaxeCont in pickaxeConts)
        {
            pickaxeCont.OnDestroy();
        }
        pickaxeConts.Clear();
        pickaxeAnimCont?.ClearModel();
    }

    private void Click_LeftButton()
    {
        if (!isAttacking) return;
        Debug.Log("Ingame ---- Click_LeftButton");
    }
    private void Click_RightButton()
    {
        if (!isAttacking) return;
        PickaxePowerManager.Inst?.TryActivate();
    }



    #region -- Change pickaxe -- ピッケルの２種装備はなしに修正
    /*
        private void TryChangePickaxe(int _slotIndex)
        {
            Debug.Log($"ピッケル変更はなしに修正中");

            return;
            if (!isAttacking || pickaxeConts.Count == 0) return;
            if (!HasPickaxeInSlot(_slotIndex)) return;
            currentPickaxeIndex = _slotIndex;
            SelectPickaxe(currentPickaxeIndex);
        }
        private bool HasPickaxeInSlot(int _slotIndex)
        {
            foreach (var pickaxeCont in pickaxeConts)
            {
                if (pickaxeCont.slotIndex == _slotIndex) return true;
            }
            return false;
        }
        private void SelectPickaxe(int _slotIndex)
        {
            Debug.Log($"ChangePickaxe: {_slotIndex}");
            foreach (var pickaxeCont in pickaxeConts)
            {
                pickaxeCont.Set_SelectPickaxe(_slotIndex);
            }
        }
        */
    #endregion

}
