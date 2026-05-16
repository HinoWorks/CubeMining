using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;


public class UI_PickaxePowerManager : UI_OutGameTabBase
{
    [SerializeField] UI_PickaxePowerUnit[] ui_pickaxePowerUnits;
    [SerializeField] UI_PickaxePowerInfo selectInfoUnit;

    private bool isDoingAction = false;


    public override async void Start_OnceInit()//主にコールバックを設定
    {
        base.thisMenuType = OutGame_MenuType.PickaxePower;
        await UniTask.WaitUntil(() => SaveLoader.Inst.currentState != state.InitialLoad);

        var index = 1;
        foreach (var ui_pickaxePowerUnit in ui_pickaxePowerUnits)
        {
            ui_pickaxePowerUnit.Init_Once(index, OnClick_SelectPickaxePowerUnit);
            index++;
        }
    }

    public override async void ToOutGame_InitData()
    {
        /*
        await Set_PickaxeEquip();
        Set_PickaxeLibrary();
        Set_PickaxeLibraryEquipState();
        SelectPickaxeUnit(equipedPickaxeIndexes.First());
        base.isReloadFin = true;
        */
        base.isReloadFin = true;
    }





    #region -- callBack --
    /// <summary>
    /// unit をクリックした時の処理
    /// </summary>
    private void OnClick_SelectPickaxePowerUnit(PickaxeUnitData _so)
    {
        //selectInfoUnit.SetData(_so);
    }
    private void SelectPickaxeUnit(int _index)
    {
        //var so = SOLoader.AttackUnitData.GetPickaxeUnitData(_index);
        //selectInfoUnit.SetData(so);
    }


    /// <summary>
    ///  装備ボタンをクリックした時の処理
    /// </summary>
    private async void OnClick_EquipPickaxe(PickaxeUnitData _so, int _equipSlotIndex)
    {
        /*
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
        */
    }


    /// <summary>
    ///  クラフトボタンをクリックした時の処理
    /// </summary>
    private async void OnClick_CraftPickaxe(PickaxeUnitData _so)
    {
        /*
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
        */
    }

    #endregion

}
