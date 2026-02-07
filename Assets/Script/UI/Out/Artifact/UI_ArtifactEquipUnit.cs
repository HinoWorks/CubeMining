using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

public class UI_ArtifactEquipUnit : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] GameObject obj_icon;
    [SerializeField] GameObject obj_locked;
    [SerializeField] HButton btn;

    public int slotIndex { get; private set; }
    public ArtifactUnitData so { get; private set; }
    public bool isOpen { get; private set; } = false;
    public bool isFreeSlot => isOpen && so == null;
    private Action<bool, ArtifactUnitData, Vector3> onMouseOver;
    private Action<ArtifactUnitData, int> onClick_UnEquip;



    public void Init_Once(int _index, Action<bool, ArtifactUnitData, Vector3> _onMouseOver,
                            Action<ArtifactUnitData, int> _onClick_UnEquip)
    {
        this.onMouseOver = _onMouseOver;
        this.onClick_UnEquip = _onClick_UnEquip;
        btn.onMouseOver += OnMouseOver_LibraryUnit;
        slotIndex = _index;
    }
    public async UniTask<int> Init()
    {
        var slotData = await SaveLoader.Inst.Get_ArtifactSlotData(slotIndex);
        if (slotIndex == 1)
        {
            isOpen = true;
        }
        else
        {
            isOpen = slotData == null ? false : slotData.isOpen;
        }
        obj_locked.SetActive(!isOpen);
        btn.enabled = isOpen;
        if (!isOpen) return -1;

        // スロットにアーティファクトが登録されている場合
        if (slotData != null && slotData.equipedArtifactIndex != -1)
        {
            so = SOLoader.ArtifactData.Get_ArtifactData(slotData.equipedArtifactIndex);
            obj_icon.SetActive(true);
            icon.sprite = so.icon;
        }
        else //スロットに登録なし
        {
            so = null;
            obj_icon.SetActive(false);
        }

        return slotData == null ? -1 : slotData.equipedArtifactIndex;
    }



    #region -- マウスアクション --
    private void OnMouseOver_LibraryUnit(bool _isEnter)
    {
        if (!isOpen) return;
        onMouseOver?.Invoke(_isEnter, so, transform.position);
    }

    public void OnClick_UnEquip()
    {
        if (isFreeSlot) return;
        onClick_UnEquip?.Invoke(so, slotIndex);
    }
    #endregion
}
