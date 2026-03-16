using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_ArtifactLibraryUnit : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] GameObject obj_locked;
    [SerializeField] GameObject obj_equip;
    [SerializeField] HButton btn;

    public int artifactIndex { get; private set; }
    public ArtifactUnitData so { get; private set; }
    public int equipSlotIndex { get; private set; } = -1;
    public bool isOpen { get; private set; } = false;
    public bool isEquiped { get; private set; } = false;
    private Action<bool, ArtifactUnitData, Vector3> onMouseOver;
    private Action<ArtifactUnitData, int> onClick_Equip;



    public void Init_Once(int _index, Action<bool, ArtifactUnitData, Vector3> _onMouseOver,
                            Action<ArtifactUnitData, int> _onClick_Equip)
    {
        artifactIndex = _index;
        so = SOLoader.ArtifactData.Get_ArtifactData(_index);
        if (so == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        this.onMouseOver = _onMouseOver;
        this.onClick_Equip = _onClick_Equip;
        btn.onMouseOver += OnMouseOver_LibraryUnit;
        icon.sprite = so.icon;
        this.gameObject.SetActive(true);
    }
    public async void Init()
    {
        var saveData = await SaveLoader.Inst.Get_ArtifactData(artifactIndex);
        isOpen = artifactIndex == 1 || saveData != null;

        obj_locked.SetActive(!isOpen);
        btn.enabled = isOpen;
    }

    // 装備状態はここで更新する
    public void Set_EquipState(bool _isEquiped)
    {
        isEquiped = _isEquiped;
        obj_equip.SetActive(isEquiped);
    }


    #region -- マウスアクション --
    private void OnMouseOver_LibraryUnit(bool _isEnter)
    {
        if (!isOpen) return;
        onMouseOver?.Invoke(_isEnter, so, transform.position);
    }

    public void OnClick_Equip()
    {
        if (isEquiped) return;
        onClick_Equip?.Invoke(so, equipSlotIndex);
    }
    #endregion

}
