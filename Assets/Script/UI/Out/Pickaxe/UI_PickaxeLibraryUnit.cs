using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_PickaxeLibraryUnit : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] GameObject obj_locked;
    [SerializeField] GameObject obj_equip;
    [SerializeField] GameObject obj_enhanceReady;
    [SerializeField] HButton btn;
    [SerializeField] HButtonConnect hButtonConnect;

    public int pickaxeIndex { get; private set; }
    public PickaxeUnitData so { get; private set; }
    public int equipSlotIndex { get; private set; } = -1;
    public bool isOpen { get; private set; } = false;
    public bool alreadyCrafted { get; private set; }
    public bool isEquiped { get; private set; } = false;
    public bool isEnhanceReady { get; private set; } = false;
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
        alreadyCrafted = saveData != null && saveData.level > 0;

        // 解放済みでない場合、一つ前のインデックスを確認
        if (!isOpen)
        {
            var prevIndex = pickaxeIndex - 1;
            var prevSaveData = await SaveLoader.Inst.Get_PickaxeData(prevIndex);
            isOpen = prevSaveData != null;
        }
        obj_locked.SetActive(!isOpen);
        btn.enabled = isOpen;
        obj_enhanceReady.SetActive(false);

        if (!isOpen) return;
        if (alreadyCrafted) return;
        Check_Resource();
    }

    public bool Check_Resource()
    {
        var so_resource = SOLoader.AttackUnitData.GetPickaxeResourceData(pickaxeIndex);
        var requredResources = new ResourceCount[7];
        requredResources[0] = new ResourceCount() { resourceType = ResourceType.Stone, requiredCount = so_resource.req_stone };
        requredResources[1] = new ResourceCount() { resourceType = ResourceType.Iron, requiredCount = so_resource.req_iron };
        requredResources[2] = new ResourceCount() { resourceType = ResourceType.Gold, requiredCount = so_resource.req_gold };
        requredResources[3] = new ResourceCount() { resourceType = ResourceType.Emerald, requiredCount = so_resource.req_emerald };
        requredResources[4] = new ResourceCount() { resourceType = ResourceType.Ruby, requiredCount = so_resource.req_ruby };
        requredResources[5] = new ResourceCount() { resourceType = ResourceType.Sapphire, requiredCount = so_resource.req_sapphire };
        requredResources[6] = new ResourceCount() { resourceType = ResourceType.Diamond, requiredCount = so_resource.req_diamond };
        isEnhanceReady = StaticManager.IsResourceEnough(requredResources);
        obj_enhanceReady.SetActive(isEnhanceReady);
        return isEnhanceReady;
    }

    // 装備状態はここで更新する
    public void Set_EquipState(bool _isEquiped)
    {
        isEquiped = _isEquiped;
        obj_equip.SetActive(isEquiped);
    }
    public void Set_SelectState(bool _isSelect)
    {
        hButtonConnect.Set_SelectActive(_isSelect);
    }


    #region -- マウスアクション --
    public void OnClick_Select()
    {
        if (!isOpen) return;
        onClick_Select?.Invoke(so);

    }
    #endregion

}
