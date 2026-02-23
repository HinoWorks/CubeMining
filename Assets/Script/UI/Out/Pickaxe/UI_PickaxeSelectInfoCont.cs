using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using System;
using System.Collections.Generic;


[System.Serializable]
public class ResourceCount
{
    public ResourceType resourceType;
    public int requiredCount;
}


public class UI_PickaxeSelectInfoCont : UI_PickaxeParamCont
{
    [Space(10)]
    [SerializeField] TextMeshProUGUI tmp_pickaxeName;
    [SerializeField] Image image_areaIcon;

    [Space(10)]
    [Header("Craft Area")]
    [SerializeField] GameObject obj_craft;
    [SerializeField] UI_ResourceCount[] ui_resourceCounts;
    [SerializeField] HButton btn_craft;


    [Space(10)]
    [Header("Equip Area")]
    [SerializeField] GameObject obj_equip;
    [SerializeField] HButton btn_equip_slot0;
    [SerializeField] HButton btn_equip_slot1;

    private bool isCraftReady = true;
    private int[] equipedPickaxeIndex = new int[2] { 0, 1 };
    private Action<PickaxeUnitData> onClick_Craft;
    private Action<PickaxeUnitData, int> onClick_Equip;
    private List<ResourceCount> requredResources = new List<ResourceCount>();
    public List<ResourceCount> RequredResources => requredResources;

    public void Init_Once(Action<PickaxeUnitData> _onClick_Craft, Action<PickaxeUnitData, int> _onClick_Equip)
    {
        this.onClick_Craft = _onClick_Craft;
        this.onClick_Equip = _onClick_Equip;
    }

    public override void SetData(PickaxeUnitData _so)
    {
        requredResources.Clear();
        base.SetData(_so);
        Set_PickaxeData();
    }

    private async void Set_PickaxeData()
    {
        tmp_pickaxeName.SetText(so.pickaxeName);
        var data_havePickaxe = await SaveLoader.Inst.Get_PickaxeData(so.pickaxeIndex);
        if (data_havePickaxe == null)
        {
            // 持っていない場合
            Set_CraftData();
            return;
        }

        // 装備中？
        foreach (var slotIndex in equipedPickaxeIndex)
        {
            var data_equipPickaxe = await SaveLoader.Inst.Get_PickaxeSlotData(slotIndex);
            if (data_equipPickaxe != null && data_equipPickaxe.equipedPickaxeIndex == so.pickaxeIndex)
            {
                // 装備中
                Debug.Log($"初期selectState --> スロット: {slotIndex} => 装備:{data_equipPickaxe.equipedPickaxeIndex}");
                Set_EquipState(slotIndex);
                return;
            }
        }
        // 装備中でない => 
        Set_EquipState();
    }


    private void Set_CraftData()
    {
        obj_craft.SetActive(true);
        obj_equip.SetActive(false);
        foreach (var resourceCount in ui_resourceCounts)
        {
            resourceCount.gameObject.SetActive(false);
        }

        isCraftReady = true;
        var so_requiredResources = SOLoader.AttackUnitData.GetPickaxeResourceData(so.pickaxeIndex);
        requredResources.Clear();
        if (so_requiredResources.req_stone > 0)
        {
            Set_ResourceCount(requredResources.Count, ResourceType.Stone, so_requiredResources.req_stone);
            requredResources.Add(new ResourceCount { resourceType = ResourceType.Stone, requiredCount = so_requiredResources.req_stone });
        }
        if (so_requiredResources.req_iron > 0)
        {
            Set_ResourceCount(requredResources.Count, ResourceType.Iron, so_requiredResources.req_iron);
            requredResources.Add(new ResourceCount { resourceType = ResourceType.Iron, requiredCount = so_requiredResources.req_iron });
        }
        if (so_requiredResources.req_gold > 0)
        {
            Set_ResourceCount(requredResources.Count, ResourceType.Gold, so_requiredResources.req_gold);
            requredResources.Add(new ResourceCount { resourceType = ResourceType.Gold, requiredCount = so_requiredResources.req_gold });
        }
        if (so_requiredResources.req_emerald > 0)
        {
            Set_ResourceCount(requredResources.Count, ResourceType.Emerald, so_requiredResources.req_emerald);
            requredResources.Add(new ResourceCount { resourceType = ResourceType.Emerald, requiredCount = so_requiredResources.req_emerald });
        }
        if (so_requiredResources.req_ruby > 0)
        {
            Set_ResourceCount(requredResources.Count, ResourceType.Ruby, so_requiredResources.req_ruby);
            requredResources.Add(new ResourceCount { resourceType = ResourceType.Ruby, requiredCount = so_requiredResources.req_ruby });
        }
        if (so_requiredResources.req_sapphire > 0)
        {
            Set_ResourceCount(requredResources.Count, ResourceType.Sapphire, so_requiredResources.req_sapphire);
            requredResources.Add(new ResourceCount { resourceType = ResourceType.Sapphire, requiredCount = so_requiredResources.req_sapphire });
        }
        if (so_requiredResources.req_diamond > 0)
        {
            Set_ResourceCount(requredResources.Count, ResourceType.Diamond, so_requiredResources.req_diamond);
            requredResources.Add(new ResourceCount { resourceType = ResourceType.Diamond, requiredCount = so_requiredResources.req_diamond });
        }
        btn_craft.Set_Interactable(isCraftReady);
    }

    private void Set_ResourceCount(int _index, ResourceType _resourceType, int _requiredCount)
    {
        var modCount = StaticManager.Get_BigintegerToUnit(_requiredCount);
        var overResource = _requiredCount <= SaveLoader.Inst.Get_ResourceCount(_resourceType);
        ui_resourceCounts[_index].SetData(SOLoader.ItemData.GetItemUnitData((int)_resourceType).icon, modCount.num.ToString(), overResource ? Color.white : Color.red);
        isCraftReady = isCraftReady && overResource;
        btn_craft.Set_Interactable(isCraftReady);
    }

    public void Set_EquipState(int _equipedSlotIndexNow = -1)
    {
        obj_craft.SetActive(false);
        obj_equip.SetActive(true);

        btn_equip_slot0.Set_Interactable(0 != _equipedSlotIndexNow);
        btn_equip_slot1.Set_Interactable(1 != _equipedSlotIndexNow);
    }



    #region -- OnClick --
    public void OnClick_Craft()
    {
        if (!isCraftReady) return;
        onClick_Craft?.Invoke(so);
    }
    public void OnClick_Equip(int _equipedSlotIndex)
    {
        onClick_Equip?.Invoke(so, _equipedSlotIndex);
    }
    #endregion

}
