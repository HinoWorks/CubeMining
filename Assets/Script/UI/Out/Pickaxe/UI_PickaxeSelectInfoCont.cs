using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

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



    public override void SetData(PickaxeUnitData _so)
    {
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
                Debug.Log($"スロット: {slotIndex} => 装備:{data_equipPickaxe.equipedPickaxeIndex}");
                Set_EquipData(slotIndex);
                return;
            }
        }
        // 装備中でない => 
        Set_EquipData();
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
        var counter = 0;
        if (so_requiredResources.req_stone > 0)
        {
            Set_ResourceCount(counter, ResourceType.Stone, so_requiredResources.req_stone);
            counter++;
        }
        if (so_requiredResources.req_iron > 0)
        {
            Set_ResourceCount(counter, ResourceType.Iron, so_requiredResources.req_iron);
            counter++;
        }
        if (so_requiredResources.req_gold > 0)
        {
            Set_ResourceCount(counter, ResourceType.Gold, so_requiredResources.req_gold);
            counter++;
        }
        if (so_requiredResources.req_emerald > 0)
        {
            Set_ResourceCount(counter, ResourceType.Emerald, so_requiredResources.req_emerald);
            counter++;
        }
        if (so_requiredResources.req_ruby > 0)
        {
            Set_ResourceCount(counter, ResourceType.Ruby, so_requiredResources.req_ruby);
            counter++;
        }
        if (so_requiredResources.req_sapphire > 0)
        {
            Set_ResourceCount(counter, ResourceType.Sapphire, so_requiredResources.req_sapphire);
            counter++;
        }
        if (so_requiredResources.req_diamond > 0)
        {
            Set_ResourceCount(counter, ResourceType.Diamond, so_requiredResources.req_diamond);
            counter++;
        }
        btn_craft.interactable = isCraftReady;
    }

    private void Set_ResourceCount(int _index, ResourceType _resourceType, int _requiredCount)
    {
        var modCount = StaticManager.Get_BigintegerToUnit(_requiredCount);
        var overResource = _requiredCount <= SaveLoader.Inst.Get_ResourceCount(_resourceType);
        ui_resourceCounts[_index].SetData(SOLoader.ItemData.GetItemUnitData((int)_resourceType).icon, modCount.num.ToString(), overResource ? Color.white : Color.red);
        isCraftReady = isCraftReady && overResource;
        btn_craft.Set_Interactable(isCraftReady);
    }

    private void Set_EquipData(int _equipedSlotIndexNow = -1)
    {
        obj_craft.SetActive(false);
        obj_equip.SetActive(true);

        // TODO HERE===
        btn_equip_slot0.Set_Interactable(0 != _equipedSlotIndexNow);
        btn_equip_slot1.Set_Interactable(1 != _equipedSlotIndexNow);

    }

}
