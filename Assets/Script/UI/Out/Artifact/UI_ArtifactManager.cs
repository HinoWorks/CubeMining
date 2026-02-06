using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;

public class UI_ArtifactManager : MonoBehaviour
{

    [SerializeField] UI_ArtifactEquipUnit[] artifactEquipUnits;
    [SerializeField] UI_ArtifactLibraryUnit[] artifactLibraryUnits;


    private bool onceInitFin = false;

    void OnceInit()
    {
        var index = 1;
        foreach (var artifactLibraryUnit in artifactLibraryUnits)
        {
            artifactLibraryUnit.Init_Once(index, OnMouseOver, OnClick_Equip);
            index++;
        }

        onceInitFin = true;
    }

    public void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.Artifact;
        if (isActive)
        {
            if (!onceInitFin)
            {
                OnceInit();
            }
            Set_ArtifactEquip();
            Set_ArtifactLibrary();
        }
        this.gameObject.SetActive(isActive);
    }


    private void Set_ArtifactEquip()
    {

    }
    private void Set_ArtifactLibrary()
    {
        foreach (var artifactLibraryUnit in artifactLibraryUnits)
        {
            artifactLibraryUnit.Init();
        }
    }



    /// <summary>
    /// unit にマウスオーバーした時の処理
    /// </summary>
    private void OnMouseOver(bool _isEnter, UI_ArtifactLibraryUnit _artifactLibraryUnit)
    {
        if (_isEnter)
        {
            Debug.Log("OnMouseOver: " + _artifactLibraryUnit.artifactIndex);
            //ui_artifactLibraryDetail.transform.position = _artifactLibraryUnit.transform.position;
            //ui_artifactLibraryDetail.SetData(_artifactLibraryUnit);
        }
        else
        {
            Debug.Log("OnMouseExit: " + _artifactLibraryUnit.artifactIndex);
            //ui_artifactLibraryDetail.SetData(null);
        }
    }

    /// <summary>
    /// unit をクリックした時の処理
    /// </summary>
    private async void OnClick_Equip(UI_ArtifactLibraryUnit _artifactLibraryUnit)
    {
        /*
        if (_skillTreeUnit.unlockState != SkillTreeUnlockState.EnhanceReady) return;
        if (StaticManager.CoinCheck(_skillTreeUnit.skillTree.cost) == false) return;
        SaveLoader.Inst.Request_SaveSkillTreeData(_skillTreeUnit.skillIndex, _skillTreeUnit.level + 1);
        SaveLoader.Inst.Request_SaveCoin(-_skillTreeUnit.skillTree.cost);

        await UniTask.DelayFrame(2);
        _skillTreeUnit.Init();

        // ベーススキルの更新
        var checkTargetUnit = Array.FindAll(skillTreeUnits, x => x.skillTree.baseSkillIndex == _skillTreeUnit.skillIndex);
        foreach (var unit in checkTargetUnit)
        {
            unit.Init();
        }
        ui_skillTreeDetail.SetData_Enhanced(_skillTreeUnit.level + 1);
        UpdateNodeState(_skillTreeUnit.skillTree.baseSkillIndex, _skillTreeUnit.skillIndex, _skillTreeUnit.unlockState, _skillTreeUnit.level + 1);
        */
    }

}
