using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;


public class UI_ArtifactManager : MonoBehaviour
{
    [SerializeField] UI_ArtifactEquipUnit[] artifactEquipUnits;
    [SerializeField] UI_ArtifactLibraryUnit[] artifactLibraryUnits;
    [SerializeField] UI_ArtifactDetailUnit detailUnit;

    private List<int> equipedArtifactIndexes = new List<int>(10);

    private bool onceInitFin = false;



    void OnceInit()//主にコールバックを設定
    {
        var index = 1;
        foreach (var artifactLibraryUnit in artifactLibraryUnits)
        {
            artifactLibraryUnit.Init_Once(index, OnMouseOver_ArtifactUnit, OnClick_ArtifactUnit);
            index++;
        }

        index = 1;
        foreach (var artifactEquipUnit in artifactEquipUnits)
        {
            artifactEquipUnit.Init_Once(index, OnMouseOver_ArtifactUnit, OnClick_ArtifactUnit);
            index++;
        }
        onceInitFin = true;
    }


    public async void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.Artifact;
        if (isActive)
        {
            if (!onceInitFin)
            {
                OnceInit();
            }
            await Set_ArtifactEquip();
            Set_ArtifactLibrary();
        }
        detailUnit.SetData(null);
        this.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// アーティファクト装備Unitの初期化
    /// </summary>
    private async UniTask Set_ArtifactEquip()
    {
        // 装備リストをクリア
        equipedArtifactIndexes.Clear();
        foreach (var artifactLibraryUnit in artifactLibraryUnits)
        {
            artifactLibraryUnit.Set_EquipState(false);
        }

        // 装備リストを更新
        foreach (var artifactEquipUnit in artifactEquipUnits)
        {
            var equipedArtifactIndex = await artifactEquipUnit.Init();
            Set_EquipedArtifactIndexes(equipedArtifactIndex, true);
        }
    }
    /// <summary>
    /// アーティファクト装備リストを更新, ライブラリunitの装備状態を更新
    /// </summary>
    private void Set_EquipedArtifactIndexes(int _index, bool _isEquiped)
    {
        if (_index == -1) return;
        if (_isEquiped)
        {
            equipedArtifactIndexes.Add(_index);
        }
        else
        {
            equipedArtifactIndexes.Remove(_index);
        }
        var targetUnit = Array.Find(artifactLibraryUnits, x => x.artifactIndex == _index);
        if (targetUnit != null)
        {
            targetUnit.Set_EquipState(_isEquiped);
        }
    }

    /// <summary>
    /// アーティファクトライブラリUnitの初期化
    /// </summary>
    private void Set_ArtifactLibrary()
    {
        foreach (var artifactLibraryUnit in artifactLibraryUnits)
        {
            artifactLibraryUnit.Init();
        }
    }




    #region -- callBack --
    /// <summary>
    /// マウスオーバー 
    /// </summary>
    private void OnMouseOver_ArtifactUnit(bool _isEnter, ArtifactUnitData _so, Vector3 _position)
    {
        if (_isEnter)
        {
            detailUnit.transform.position = _position;
            detailUnit.SetData(_so);
        }
        else
        {
            detailUnit.SetData(null);
        }
    }

    /// <summary>
    /// unit をクリックした時の処理
    /// </summary>
    private async void OnClick_ArtifactUnit(ArtifactUnitData _so, int _equipSlotIndex)
    {
        // 空きスロットを見つけて登録
        if (_equipSlotIndex == -1)
        {
            var freeSlot = Array.Find(artifactEquipUnits, x => x.isFreeSlot);
            if (freeSlot != null)
            {
                Set_EquipedArtifactIndexes(_so.artifactIndex, true);
                SaveLoader.Inst.Request_SaveArtifactSlotData(freeSlot.slotIndex, true, _so.artifactIndex);
                await UniTask.DelayFrame(1);
                freeSlot.Init();
                Debug.Log($"空きスロット:{freeSlot.slotIndex} / アーティファクト:{_so.artifactIndex}");
            }
            else
            {
                Debug.LogError("空きスロットが見つかりません");
            }
        }
        else // 指定したスロットから削除
        {
            var targetSlot = Array.Find(artifactEquipUnits, x => x.slotIndex == _equipSlotIndex);
            if (targetSlot != null)
            {
                if (targetSlot.isFreeSlot) return;
                Set_EquipedArtifactIndexes(_so.artifactIndex, false);
                SaveLoader.Inst.Request_SaveArtifactSlotData(_equipSlotIndex, true, -1);
                await UniTask.DelayFrame(1);
                targetSlot.Init();
                Debug.Log($"指定したスロット:{_equipSlotIndex} のアーティファクトを削除");
            }
            else
            {
                Debug.LogError($"指定したスロット:{_equipSlotIndex} が見つかりません");
            }
        }

    }
    #endregion

}
