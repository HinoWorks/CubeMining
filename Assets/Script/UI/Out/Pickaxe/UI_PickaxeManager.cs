using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;


public class UI_PickaxeManager : MonoBehaviour
{
    [SerializeField] UI_PickaxeEquipCont[] pickaxeEquipConts;
    [SerializeField] UI_PickaxeLibraryUnit[] pickaxeLibraryUnits;
    [SerializeField] UI_PickaxeSelectInfoCont selectInfoUnit;
    private bool onceInitFin = false;

    private int[] slotIndexes = { 0, 1 };


    void OnceInit()//主にコールバックを設定
    {
        var index = 1;
        foreach (var pickaxeLibraryUnit in pickaxeLibraryUnits)
        {
            pickaxeLibraryUnit.Init_Once(index, SelectPickaxeUnit);
            index++;
        }

        index = 0;
        onceInitFin = true;
    }


    public async void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.Pickaxe;
        if (isActive)
        {
            if (!onceInitFin)
            {
                OnceInit();
            }
            await Set_PickaxeEquip();
            Set_PickaxeLibrary();
        }
        this.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// ピッケル装備Unitの初期化
    /// </summary>
    private async UniTask Set_PickaxeEquip()
    {
        foreach (var slotIndex in slotIndexes)
        {
            var slotData = await SaveLoader.Inst.Get_PickaxeSlotData(slotIndex);
            if (slotData == null)
            {
                pickaxeEquipConts[slotIndex].SetData(null);
                continue;
            }
            var pickaxeUnitData = SOLoader.AttackUnitData.GetPickaxeUnitData(slotData.equipedPickaxeIndex);
            pickaxeEquipConts[slotIndex].SetData(pickaxeUnitData);
        }
    }


    /// <summary>
    /// ピッケル装備リストを更新, ライブラリunitの装備状態を更新
    /// </summary>
    private void Set_EquipedPickaxeIndexes(int _index, bool _isEquiped)
    {
        /*
        if (_index == -1 || _index <= 0) return;
        if (_isEquiped)
        {
            equipedPickaxeIndexes.Add(_index);
        }
        else
        {
            equipedPickaxeIndexes.Remove(_index);
        }
        var targetUnit = Array.Find(pickaxeLibraryUnits, x => x.pickaxeIndex == _index);
        if (targetUnit != null)
        {
            targetUnit.Set_EquipState(_isEquiped);
        }
        */
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


    /*
    /// <summary>
    ///  装備ボタンをクリックした時の処理
    /// </summary>
    private async void OnClick_SelectPickaxeUnit(PickaxeUnitData _so, int _equipSlotIndex)
    {
        // 空きスロットを見つけて登録
        if (_equipSlotIndex == -1)
        {
            var freeSlot = Array.Find(pickaxeEquipConts, x => x.isFreeSlot);
            if (freeSlot != null)
            {
                Set_EquipedPickaxeIndexes(_so.pickaxeIndex, true);
                SaveLoader.Inst.Request_SavePickaxeSlotData(freeSlot.slotIndex, _so.pickaxeIndex);
                await UniTask.DelayFrame(1);
                freeSlot.Init();
                Debug.Log($"空きスロット:{freeSlot.slotIndex} / ピッケル:{_so.pickaxeIndex}");
            }
            else
            {
                Debug.LogError("空きスロットが見つかりません");
            }
        }
        else // 指定したスロットから削除
        {
            var targetSlot = Array.Find(pickaxeEquipConts, x => x.slotIndex == _equipSlotIndex);
            if (targetSlot != null)
            {
                if (targetSlot.isFreeSlot) return;
                Set_EquipedPickaxeIndexes(_so.pickaxeIndex, false);
                SaveLoader.Inst.Request_SavePickaxeSlotData(_equipSlotIndex, -1);
                await UniTask.DelayFrame(1);
                targetSlot.Init();
                Debug.Log($"指定したスロット:{_equipSlotIndex} のピッケルを削除");
            }
            else
            {
                Debug.LogError($"指定したスロット:{_equipSlotIndex} が見つかりません");
            }
        }
    }
    */
    #endregion

}
