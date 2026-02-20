using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_PickaxeLibraryUnit : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] GameObject obj_locked;
    [SerializeField] GameObject obj_equip;
    [SerializeField] HButton btn;

    public int pickaxeIndex { get; private set; }
    public PickaxeUnitData so { get; private set; }
    public int equipSlotIndex { get; private set; } = -1;
    public bool isOpen { get; private set; } = false;
    public bool isEquiped { get; private set; } = false;
    private Action<PickaxeUnitData> onClick_Select;



    public void Init_Once(int _index, Action<PickaxeUnitData> _onClick_Select)
    {
        pickaxeIndex = _index;
        so = SOLoader.AttackUnitData.GetPickaxeUnitData(_index);
        if (so == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        this.onClick_Select = _onClick_Select;
        icon.sprite = so.icon;
    }
    public async void Init()
    {
        var saveData = await SaveLoader.Inst.Get_PickaxeData(pickaxeIndex);
        isOpen = pickaxeIndex == 1 || saveData != null;

        // 解放済みでない場合、一つ前のインデックスを確認
        if (!isOpen)
        {
            var prevIndex = pickaxeIndex - 1;
            var prevSaveData = await SaveLoader.Inst.Get_PickaxeData(prevIndex);
            isOpen = prevSaveData != null;
        }
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
    public void OnClick_Select()
    {
        if (isEquiped) return;
        onClick_Select?.Invoke(so);
    }
    #endregion

}
