using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;


public class UI_PickaxePowerManager : UI_OutGameTabBase
{
    [SerializeField] TextMeshProUGUI tmp_points;
    [SerializeField] UI_PickaxePowerUnit[] ui_pickaxePowerUnits;
    [SerializeField] UI_PickaxePowerInfo ui_selectInfo;

    private bool isDoingAction = false;

    private int currentPoints = 0;
    private int currentEquipedIndex = 0;
    private UI_PickaxePowerUnit currentSelectUnit = null;




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
        ui_selectInfo.Init_Once(OnClick_Equip, OnClick_Unlock, OnClick_Enhance);
    }

    public override async void ToOutGame_InitData()
    {
        var saveData_playerLevel = await SaveLoader.Inst.Get_PlayerLevelData();
        currentPoints = saveData_playerLevel == null ? 0 : saveData_playerLevel.points;
        tmp_points.SetText($"{currentPoints}");
        var currentPlayerLevel = saveData_playerLevel == null ? 0 : saveData_playerLevel.level;

        // 各Unit初期化(主にリソースチェック)
        foreach (var ui_pickaxePowerUnit in ui_pickaxePowerUnits)
        {
            ui_pickaxePowerUnit.Init(currentPoints, currentPlayerLevel);
        }

        // 装備状態更新
        currentEquipedIndex = SaveLoader.Inst.PickaxePowerEquipedIndex;
        foreach (var ui_pickaxePowerUnit in ui_pickaxePowerUnits)
        {
            ui_pickaxePowerUnit.EquipMark_Update(currentEquipedIndex);
        }

        // info UI 初期化 / 装備中のものがあればそれを表示する
        var targetUnit = Get_Unit(currentEquipedIndex);
        ui_selectInfo.SetData(targetUnit, currentPoints);
        var isEquiped = targetUnit != null && targetUnit.so_base.index == currentEquipedIndex;
        ui_selectInfo.SetData_Equiped(isEquiped);

        base.isReloadFin = true;
    }



    private UI_PickaxePowerUnit Get_Unit(int _index)
    {
        return Array.Find(ui_pickaxePowerUnits, unit => unit.so_base.index == _index);
    }



    #region -- callBack --
    /// <summary>
    /// unit をクリックした時の処理
    /// </summary>
    private void OnClick_SelectPickaxePowerUnit(UI_PickaxePowerUnit _ui_pickaxePowerUnit)
    {
        if (currentSelectUnit != null && currentSelectUnit.so_base.index == _ui_pickaxePowerUnit.so_base.index) return;

        currentSelectUnit?.SelectMark_Update(false);
        currentSelectUnit = _ui_pickaxePowerUnit;
        currentSelectUnit.SelectMark_Update(true);
        ui_selectInfo.SetData(currentSelectUnit, currentPoints);
        ui_selectInfo.SetData_Equiped(currentSelectUnit.so_base.index == currentEquipedIndex);
    }

    /// <summary>
    ///  装備ボタンをクリックした時の処理
    /// </summary>
    private async void OnClick_Equip()
    {
        if (isDoingAction) return;
        isDoingAction = true;

        var newEquipedIndex = currentSelectUnit.so_base.index;
        SaveLoader.Inst.Request_SavePickaxePowerData_EquipedIndex(newEquipedIndex);
        // 装備状態更新
        currentEquipedIndex = newEquipedIndex;
        foreach (var ui_pickaxePowerUnit in ui_pickaxePowerUnits)
        {
            ui_pickaxePowerUnit.EquipMark_Update(newEquipedIndex);
        }
        ui_selectInfo.SetData_Equiped(true);

        await UniTask.DelayFrame(2);
        isDoingAction = false;
    }

    /// <summary>
    ///  アンロックをクリックした時の処理
    /// </summary>
    private async void OnClick_Unlock()
    {
        if (isDoingAction) return;
        isDoingAction = true;

        // リソース消費
        foreach (var resourceCount in ui_selectInfo.RequredResources)
        {
            if (resourceCount.requiredCount <= 0) continue;
            SaveLoader.Inst.Request_SaveResource(resourceCount.resourceType, -resourceCount.requiredCount);
        }
        var pointCost = ui_selectInfo.requiredPoints;
        SaveLoader.Inst.Request_SavePlayerLevelData(-pointCost);
        currentPoints -= pointCost;

        // 強化
        var newLevel = currentSelectUnit.currentLevel + 1;
        SaveLoader.Inst.Request_SavePickaxePowerData_Level(currentSelectUnit.so_base.index, newLevel);

        //UIに反映
        currentSelectUnit.Callback_Enhanced(newLevel, currentPoints);
        await UniTask.DelayFrame(2);
        ui_selectInfo.CallBack_Enhanced(newLevel);
        tmp_points.SetText($"{currentPoints}");

        if (currentSelectUnit.so_base.index == 1 && newLevel == 1)
        {
            // 初めてパワーをアンロックした時は自動で装備する
            SaveLoader.Inst.Request_SavePickaxePowerData_EquipedIndex(currentSelectUnit.so_base.index);
            currentEquipedIndex = currentSelectUnit.so_base.index;
            foreach (var ui_pickaxePowerUnit in ui_pickaxePowerUnits)
            {
                ui_pickaxePowerUnit.EquipMark_Update(currentSelectUnit.so_base.index);
            }
            ui_selectInfo.SetData_Equiped(true);
        }

        // 他のUnitのリソースチェック
        foreach (var ui_pickaxePowerUnit in ui_pickaxePowerUnits)
        {
            if (ui_pickaxePowerUnit.so_base.index == currentSelectUnit.so_base.index) continue;
            ui_pickaxePowerUnit.ResourceCheck(currentPoints);
        }
        isDoingAction = false;
    }


    /// <summary>
    ///  強化をクリックした時の処理
    /// </summary>
    private async void OnClick_Enhance()
    {
        OnClick_Unlock();
    }
    #endregion

}
