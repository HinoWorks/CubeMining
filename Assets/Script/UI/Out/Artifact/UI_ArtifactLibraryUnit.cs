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

    private Action<bool, UI_ArtifactLibraryUnit> onMouseOver;
    private Action<UI_ArtifactLibraryUnit> onClick_Equip;



    public void Init_Once(int _index, Action<bool, UI_ArtifactLibraryUnit> _onMouseOver,
                            Action<UI_ArtifactLibraryUnit> _onClick_Equip)
    {
        this.onMouseOver = _onMouseOver;
        this.onClick_Equip = _onClick_Equip;
        //btn.onMouseOver += OnPointerEnter;
        artifactIndex = _index;
        so = SOLoader.ArtifactData.Get_ArtifactData(_index);
        if (so == null) return;
        icon.sprite = so.icon;
    }
    public async void Init()
    {
        var saveData = await SaveLoader.Inst.Get_ArtifactData(artifactIndex);
        isOpen = saveData != null;


        obj_locked.SetActive(!isOpen);
        equipSlotIndex = saveData == null ? -1 : saveData.equipSlotIndex;
        btn.enabled = isOpen;
        obj_equip.SetActive(equipSlotIndex != -1);
    }


    private void OnPointerEnter(bool _isEnter)
    {
        if (!isOpen) return;
        onMouseOver?.Invoke(_isEnter, this);
    }

    public void OnClick_Equip()
    {
        Debug.Log("OnClick_Equip");
    }

}
