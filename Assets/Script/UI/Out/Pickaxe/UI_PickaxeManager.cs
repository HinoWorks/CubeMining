using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using JetBrains.Annotations;

public class UI_PickaxeManager : MonoBehaviour
{
    [SerializeField] UI_PickaxeEquipCont[] pickaxeEquipConts;
    [SerializeField] UI_PickaxeLibraryUnit[] pickaxeLibraryUnits;
    [SerializeField] UI_PickaxeSelectInfoCont selectInfoUnit;
    [SerializeField] UI_PickaxeGetAnimCont ui_getNewPickaxe;
    private int[] slotIndexes = { 0, 1 };
    private HashSet<int> equipedPickaxeIndexes = new HashSet<int>();
    private bool isDoingAction = false;
    private int pickaxeAnimWaitTime = 1000;


    public async void Start_OnceInit()//主にコールバックを設定
    {
        var index = 1;
        foreach (var pickaxeLibraryUnit in pickaxeLibraryUnits)
        {
            pickaxeLibraryUnit.Init_Once(index, SelectPickaxeUnit);
            index++;
        }
        selectInfoUnit.Init_Once(OnClick_CraftPickaxe, OnClick_EquipPickaxe);

        await Set_PickaxeEquip();
        Set_PickaxeLibrary();
        Set_PickaxeLibraryEquipState();
        SelectPickaxeUnit(equipedPickaxeIndexes.First());
    }


    public async void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.Pickaxe;
        if (isActive)
        {
            isDoingAction = true;

            await Set_PickaxeEquip();
            Set_PickaxeLibrary();
            Set_PickaxeLibraryEquipState();
            SelectPickaxeUnit(equipedPickaxeIndexes.First());
            isDoingAction = false;
        }
        this.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// ピッケル装備Unitの初期化
    /// </summary>
    private async UniTask Set_PickaxeEquip()
    {
        equipedPickaxeIndexes.Clear();
        foreach (var slotIndex in slotIndexes)
        {
            var slotData = await SaveLoader.Inst.Get_PickaxeSlotData(slotIndex);
            if (slotData == null || slotData.equipedPickaxeIndex <= 0)
            {
                pickaxeEquipConts[slotIndex].SetData(null);
                Debug.Log($"初期Equip --> スロット: {slotIndex} => 装備: --- ");
                continue;
            }
            var pickaxeUnitData = SOLoader.AttackUnitData.GetPickaxeUnitData(slotData.equipedPickaxeIndex);
            pickaxeEquipConts[slotIndex].SetData(pickaxeUnitData);
            equipedPickaxeIndexes.Add(slotData.equipedPickaxeIndex);
            Debug.Log($"初期Equip --> スロット: {slotIndex} => 装備: {slotData.equipedPickaxeIndex}");
        }
    }


    /// <summary>
    /// ピッケルライブラリUnitの初期化
    /// </summary>
    private void Set_PickaxeLibrary()
    {
        foreach (var pickaxeLibraryUnit in pickaxeLibraryUnits)
        {
            pickaxeLibraryUnit.Init();
        }
    }
    private void Set_PickaxeLibraryEquipState()
    {
        foreach (var pickaxeLibraryUnit in pickaxeLibraryUnits)
        {
            pickaxeLibraryUnit.Set_EquipState(equipedPickaxeIndexes.Contains(pickaxeLibraryUnit.pickaxeIndex));
        }
    }


    #region -- callBack --
    /// <summary>
    /// unit をクリックした時の処理
    /// </summary>
    private void SelectPickaxeUnit(PickaxeUnitData _so)
    {
        selectInfoUnit.SetData(_so);
    }
    private void SelectPickaxeUnit(int _index)
    {
        var so = SOLoader.AttackUnitData.GetPickaxeUnitData(_index);
        selectInfoUnit.SetData(so);
    }



    /// <summary>
    ///  装備ボタンをクリックした時の処理
    /// </summary>
    private async void OnClick_EquipPickaxe(PickaxeUnitData _so, int _equipSlotIndex)
    {
        if (isDoingAction) return;
        isDoingAction = true;
        //装備中のピッケルの位置替え
        if (equipedPickaxeIndexes.Contains(_so.pickaxeIndex))
        {
            var slotData_0 = await SaveLoader.Inst.Get_PickaxeSlotData(0);
            var equipedPickaxeIndex_0 = slotData_0 == null ? -1 : slotData_0.equipedPickaxeIndex;
            var slotData_1 = await SaveLoader.Inst.Get_PickaxeSlotData(1);
            var equipedPickaxeIndex_1 = slotData_1 == null ? -1 : slotData_1.equipedPickaxeIndex;
            SaveLoader.Inst.Request_SavePickaxeSlotData(0, equipedPickaxeIndex_1);
            SaveLoader.Inst.Request_SavePickaxeSlotData(1, equipedPickaxeIndex_0);

            await UniTask.DelayFrame(2);
        }
        else
        {
            SaveLoader.Inst.Request_SavePickaxeSlotData(_equipSlotIndex, _so.pickaxeIndex);
            await UniTask.DelayFrame(1);
        }
        selectInfoUnit.Set_EquipState(_equipSlotIndex);
        await Set_PickaxeEquip();
        Set_PickaxeLibraryEquipState();

        isDoingAction = false;
    }


    /// <summary>
    ///  クラフトボタンをクリックした時の処理
    /// </summary>
    private async void OnClick_CraftPickaxe(PickaxeUnitData _so)
    {
        if (isDoingAction) return;
        isDoingAction = true;

        // 一応チェック
        foreach (var resourceCount in selectInfoUnit.RequredResources)
        {
            if (SaveLoader.Inst.Get_ResourceCount(resourceCount.resourceType) < resourceCount.requiredCount)
            {
                isDoingAction = false;
                Debug.Log($"クラフト不可 --> リソース不足: {resourceCount.resourceType} => {resourceCount.requiredCount}");
                return;
            }
        }
        // クラフト処理 
        foreach (var resourceCount in selectInfoUnit.RequredResources)
        {
            SaveLoader.Inst.Request_SaveResource(resourceCount.resourceType, -resourceCount.requiredCount);
        }
        SaveLoader.Inst.Request_SavePickaxeData(_so.pickaxeIndex, 1);

        // 新しいピッケルを表示
        ui_getNewPickaxe.SetIcon(_so.icon);
        await UniTask.Delay(pickaxeAnimWaitTime);

        Set_PickaxeLibrary();
        selectInfoUnit.Set_EquipState(-1);

        isDoingAction = false;
    }

    #endregion

}
