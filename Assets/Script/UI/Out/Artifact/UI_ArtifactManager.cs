using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;

public class UI_ArtifactManager : MonoBehaviour
{

    [SerializeField] UI_ArtifactEquipUnit[] artifactEquipUnits;
    [SerializeField] UI_ArtifactLibraryUnit[] artifactLibraryUnits;


    void Awake()
    {
        // 初期化処理
    }

    public void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.Artifact;
        this.gameObject.SetActive(isActive);
        if (!isActive) return;

        Set_ArtifactEquip();
        Set_ArtifactLibrary();
    }


    private void Set_ArtifactEquip()
    {

    }
    private void Set_ArtifactLibrary()
    {

    }


}
