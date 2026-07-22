using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;


public class ParamData
{
    public string paramName;
    public string paramNow;
    public string paramNext;
}

public class UI_PickaxePowerInfo : MonoBehaviour
{
    private UI_PickaxePowerUnit currentUnit;
    private int currentLevel => currentUnit.currentLevel;
    private PickaxePowerBase so_base => currentUnit.so_base;
    private PickaxePowerLevel so_level => currentUnit.so_level;
    private bool isMaxLevel => currentLevel >= so_base.maxLevel;
    private bool resourceReady = false;
    private bool isEquiped = false;
    private List<ParamData> paramDatas = new List<ParamData>();
    private List<ResourceCount> requredResources = new List<ResourceCount>();
    public List<ResourceCount> RequredResources => requredResources;
    private int currentPoints = 0;
    public int requiredPoints => so_level.req_point;

    private Action onClick_Equip;
    private Action onClick_Unlock;
    private Action onClick_Enhance;


    [Header("Base Info")]
    [SerializeField] TextMeshProUGUI tmp_powerName;
    [SerializeField] TextMeshProUGUI tmp_powerDescription;
    [SerializeField] Image image_icon;
    [SerializeField] UI_StarLevel[] ui_StarLevels;
    [SerializeField] TextMeshProUGUI tmp_blockCount;
    [SerializeField] TextMeshProUGUI tmp_CD;
    [SerializeField] ParticleSystem eff_enhance;

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

    [Space(5)]
    [Header("Locked Info")]
    [SerializeField] GameObject obj_locked;
    [SerializeField] TextMeshProUGUI tmp_lockedDescription;



    public void Init_Once(Action _onClick_Equip, Action _onClick_Unlock, Action _onClick_Enhance)
    {
        onClick_Equip = _onClick_Equip;
        onClick_Unlock = _onClick_Unlock;
        onClick_Enhance = _onClick_Enhance;
    }


    public void SetData(UI_PickaxePowerUnit _ui_pickaxePowerUnit, int _currentPoints)
    {
        this.gameObject.SetActive(_ui_pickaxePowerUnit != null);
        if (_ui_pickaxePowerUnit == null) return;
        currentUnit = _ui_pickaxePowerUnit;
        currentPoints = _currentPoints;

        obj_locked.SetActive(!currentUnit.isEnoughPlayerLevel);
        if (!currentUnit.isEnoughPlayerLevel)
        {
            tmp_lockedDescription.SetText($"Lv.: {so_base.unlockLevel}");
            return;
        }


        // データ設定   
        image_icon.sprite = so_base.icon;
        tmp_powerName.SetText(so_base.skillName);
        tmp_powerDescription.SetText(so_base.description);
        // 旧: blockCountチャージ / 現行: useCount回数制限（復帰時は so_base.blockCount に戻す）
        //tmp_blockCount.SetText($"{so_level.value_4}");
        //tmp_CD.SetText($"{so_base.CD} sec");

        for (int i = 0; i < ui_StarLevels.Length; i++)
        {
            ui_StarLevels[i].gameObject.SetActive(i < so_base.maxLevel);
            ui_StarLevels[i].Set_StarLevel(i < currentLevel);
        }

        SetData_Param(currentLevel);
        SetData_RequiredCost();
    }
    public void SetData_Equiped(bool _isEquiped)
    {
        if (currentUnit == null) return;
        var isEquipable = currentLevel > 0;
        isEquiped = _isEquiped;
        obj_equipedMark.SetActive(isEquiped && isEquipable);
        btn_equip.SetActive(!isEquiped && isEquipable);
    }


    private void SetData_Param(int _currentLevel)
    {
        if (_currentLevel <= 0)
        {
            parent_levelUp.SetActive(false);
            return;
        }

        parent_levelUp.SetActive(true);
        paramDatas.Clear();

        var so_nextLevel = SOLoader.PickaxePowerData.GetPickaxePowerLevel(so_base.index, currentLevel + 1);
        var displays = SOLoader.PickaxePowerData.GetParamDisplays(so_base.index);
        foreach (var display in displays)
        {
            paramDatas.Add(new ParamData()
            {
                paramName = display.paramName,
                paramNow = FormatParamValue(so_level.GetValue(display.valueSlot), display),
                paramNext = so_nextLevel != null
                    ? FormatParamValue(so_nextLevel.GetValue(display.valueSlot), display)
                    : ""
            });
        }

        foreach (var unit in ui_paramUnits)
        {
            unit.gameObject.SetActive(false);
        }
        var count = 0;
        foreach (var paramData in paramDatas)
        {
            if (count >= ui_paramUnits.Length) break;
            if (isMaxLevel)
            {
                ui_paramUnits[count].SetData_OnlyNow(paramData.paramName, paramData.paramNow);
            }
            else
            {
                ui_paramUnits[count].SetData(paramData.paramName, paramData.paramNow, paramData.paramNext);
            }
            count++;
        }
    }

    private static string FormatParamValue(float value, PickaxePowerParamDisplay display)
    {
        var prefix = display.prefix ?? "";
        return display.format switch
        {
            ParamFormat.Percent => $"{prefix}{value * 100}%",
            ParamFormat.Second => $"{prefix}{value} sec",
            _ => $"{prefix}{value}"
        };
    }



    public void CallBack_Enhanced(int _newLevel)
    {
        for (int i = 0; i < ui_StarLevels.Length; i++)
        {
            ui_StarLevels[i].Set_StarLevel(i < currentLevel);
        }
        SetData_Param(currentLevel);
        SetData_RequiredCost();
        eff_enhance.Play();

        if (_newLevel == 1)
        {
            btn_equip.SetActive(true);
        }
    }

    private void SetData_RequiredCost()
    {
        parent_enhance.SetActive(!isMaxLevel);
        if (isMaxLevel) return;
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

#if UNITY_EDITOR
        if (SROptions.isPickaxePowerUpgradeNoMaterial)
        {
            resourceReady = true;
        }
#endif
        btn_enhance.Set_Interactable(resourceReady);
        btn_unlock.Set_Interactable(resourceReady);
        btn_unlock.gameObject.SetActive(currentLevel <= 0);
        btn_enhance.gameObject.SetActive(currentLevel > 0);
    }





    #region -- OnClick --
    public void OnClick_Equip()
    {
        if (currentLevel <= 0) return;
        if (isEquiped) return;
        onClick_Equip?.Invoke();
    }
    public void OnClick_Enhance()
    {
        if (isMaxLevel) return;
#if UNITY_EDITOR
        if (!SROptions.isPickaxePowerUpgradeNoMaterial && !resourceReady) return;
#else
        if (!resourceReady) return;
#endif

        onClick_Enhance?.Invoke();
    }
    public void OnClick_Unlock()
    {
        if (isMaxLevel) return;
#if UNITY_EDITOR
        if (!SROptions.isPickaxePowerUpgradeNoMaterial && !resourceReady) return;
#else
        if (!resourceReady) return;
#endif

        onClick_Unlock?.Invoke();
    }
    #endregion
}
