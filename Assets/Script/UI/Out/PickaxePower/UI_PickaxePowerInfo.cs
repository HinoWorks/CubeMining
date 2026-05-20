using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class UI_PickaxePowerInfo : MonoBehaviour
{
    private UI_PickaxePowerUnit currentUnit;
    private int currentLevel => currentUnit.currentLevel;
    private PickaxePowerBase so_base => currentUnit.so_base;
    private PickaxePowerLevel so_level => currentUnit.so_level;
    private bool isMaxLevel => currentLevel >= so_base.maxLevel;
    private bool resourceReady = false;
    private List<ResourceCount> requredResources = new List<ResourceCount>();
    private int currentPoints = 0;


    [Header("Base Info")]
    [SerializeField] TextMeshProUGUI tmp_powerName;
    [SerializeField] TextMeshProUGUI tmp_powerDescription;
    [SerializeField] Image image_icon;
    [SerializeField] UI_StarLevel[] ui_StarLevels;

    [Space(5)]
    [Header("Equip Info")]
    [SerializeField] GameObject btn_equip;
    [SerializeField] GameObject obj_equipedMark;



    [Space(10)]
    [Header("LevelUp Param")]
    [SerializeField] GameObject parent_levelUp;
    [SerializeField] UI_ParamUnit[] ui_paramUnits;


    [Space(10)]
    [Header("Resource Cost")]
    [SerializeField] GameObject parent_enhance;
    [SerializeField] UI_ResourceCount ui_resourcePoint;
    [SerializeField] UI_ResourceCount[] ui_resourceCounts;
    [SerializeField] HButton btn_unlock;
    [SerializeField] HButton btn_enhance;





    public void SetData(UI_PickaxePowerUnit _ui_pickaxePowerUnit, int _currentPoints)
    {
        this.gameObject.SetActive(_ui_pickaxePowerUnit != null);
        if (_ui_pickaxePowerUnit == null) return;
        currentUnit = _ui_pickaxePowerUnit;
        currentPoints = _currentPoints;

        // データ設定   
        image_icon.sprite = so_base.icon;
        tmp_powerName.SetText(so_base.skillName);
        tmp_powerDescription.SetText(so_base.description);


        for (int i = 0; i < ui_StarLevels.Length; i++)
        {
            ui_StarLevels[i].gameObject.SetActive(i < so_base.maxLevel);
            ui_StarLevels[i].Set_StarLevel(i < currentLevel);
        }

        SetData_Param(currentLevel);
        SetData_RequiredCost();
    }


    private void SetData_Param(int _currentLevel)
    {
    }

    public void SetData_Enhanced(int _currentLevel)
    {
        //SetData_Base(_currentLevel);
    }

    private void SetData_RequiredCost()
    {
        if (isMaxLevel)
        {
            parent_levelUp.SetActive(false);
            return;
        }


        parent_levelUp.SetActive(true);
        resourceReady = true;

        // power Point を確認
        resourceReady = currentPoints >= so_level.req_point;
        ui_resourcePoint.SetData(so_level.req_point.ToString(), resourceReady ? Color.white : Color.red);

        // 鉱石リソースを確認
        foreach (var cont in ui_resourceCounts)
        {
            cont.NotActive();
        }
        requredResources.Clear();
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Stone, requiredCount = so_level.req_stone });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Iron, requiredCount = so_level.req_iron });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Gold, requiredCount = so_level.req_gold });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Emerald, requiredCount = so_level.req_emerald });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Ruby, requiredCount = so_level.req_ruby });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Sapphire, requiredCount = so_level.req_sapphire });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Diamond, requiredCount = so_level.req_diamond });

        var count = 0;
        foreach (var resource in requredResources)
        {
            if (resource.requiredCount <= 0) continue;
            var cont = ui_resourceCounts[count];
            if (!GameParamManager.blockChangeRateParam.IsBlockTypeUnlock(resource.resourceType))
            {
                cont.SetLock();
                resourceReady = false;
            }
            else
            {
                var overResource = StaticManager.IsResourceEnough(resource.resourceType, resource.requiredCount);
                cont.SetData(SOLoader.ItemData.GetItemUnitData((int)resource.resourceType).icon, resource.requiredCount.ToString(), overResource ? Color.white : Color.red);
                resourceReady = resourceReady && overResource;
            }
            count++;
        }

        btn_enhance.Set_Interactable(resourceReady);
        btn_unlock.Set_Interactable(resourceReady);
        btn_unlock.gameObject.SetActive(currentLevel <= 0);
        btn_enhance.gameObject.SetActive(currentLevel > 0);
    }






    #region -- OnClick --
    public void OnClick_Equip()
    {
        Debug.Log("OnClick_Equip");
    }
    public void OnClick_Enhance()
    {
        Debug.Log("OnClick_Enhance");
    }
    public void OnClick_Unlock()
    {
        Debug.Log("OnClick_Unlock");
    }
    #endregion
}
