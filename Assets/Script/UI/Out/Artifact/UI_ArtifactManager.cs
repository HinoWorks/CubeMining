using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;


public class UI_ArtifactManager : UI_OutGameTabBase
{
    [SerializeField] UI_ArtifactEquipUnit[] artifactEquipUnits;
    [SerializeField] UI_ArtifactLibraryUnit[] artifactLibraryUnits;
    [SerializeField] UI_ArtifactDetailUnit detailUnit;
    private List<int> equipedArtifactIndexes = new List<int>(10);
    private int artifact_activeSlotCount => 1 + GameParamManager.gameBaseParam.artifact_slotCount; //現在のアクティブスロット数
    private int artifact_totalSlotCountMax = 4; //最大スロット数

    private List<int> ingameGetArtifactIndexes = new List<int>(3); //インゲームで取得したアーティファクトのインデックスリスト


    public override void Start_OnceInit()//主にコールバックを設定
    {
        base.thisMenuType = OutGame_MenuType.Artifact;
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
        Set_ArtifactEquip().Forget();
        Set_ArtifactLibrary();
    }

    /// <summary>
    /// 直近インゲームで取得したアーティファクトのインデックスリストに追加
    /// </summary>
    /// <param name="_artifactIndexes"></param>
    public void Set_IngameGetArtifactIndexes(int _artifactIndex)
    {
        if (ingameGetArtifactIndexes.Contains(_artifactIndex)) return;
        ingameGetArtifactIndexes.Add(_artifactIndex);
    }


    // アウトゲームに移行した時、一度だけデータを更新する
    public override async void ToOutGame_InitData()
    {
        await Set_ArtifactEquip();
        Set_ArtifactLibrary();
        base.isReloadFin = true;

        // インゲームで取得したアーティファクトのインデックスリストがある場合、チェックマークを表示
        if (ingameGetArtifactIndexes.Count <= 0) return;
        UIManager_OutGame.Inst.Set_CheckMarkActive(OutGame_MenuType.Artifact, true);
        foreach (var artifactIndex in ingameGetArtifactIndexes)
        {
            var artifactLibraryUnit = Array.Find(artifactLibraryUnits, x => x.artifactIndex == artifactIndex);
            if (artifactLibraryUnit != null)
            {
                artifactLibraryUnit.Set_CheckMarkActive(true);
            }
        }

        ingameGetArtifactIndexes.Clear();
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

        // 解放済みのスロットのみ初期化処理
        for (int i = 0; i < artifact_activeSlotCount; i++)
        {
            var equipedArtifactIndex = await artifactEquipUnits[i].Init();
            Set_EquipedArtifactIndexes(equipedArtifactIndex, true);
        }
        for (int i = artifact_activeSlotCount; i < artifact_totalSlotCountMax; i++)
        {
            artifactEquipUnits[i].Set_Locked();
        }
        /*
            // 装備リストを更新
            foreach (var artifactEquipUnit in artifactEquipUnits)
            {
                var equipedArtifactIndex = await artifactEquipUnit.Init();
                Set_EquipedArtifactIndexes(equipedArtifactIndex, true);
            }
         */
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
            detailUnit.SetData(_so);
            detailUnit.SetPositionWithAutoFlip(_position);
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
                Debug.Log($"空きスロット:{freeSlot.slotIndex} / アーティファクト:{_so.artifactIndex} set");
            }
            else
            {
                Debug.Log("空きスロットが見つかりません");
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
                Debug.Log($"指定したスロット:{_equipSlotIndex} が見つかりません");
            }
        }

    }
    #endregion

}
