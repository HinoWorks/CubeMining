using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_SkillTreeDetail : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_skillName;
    [SerializeField] TextMeshProUGUI tmp_description;
    [SerializeField] GameObject obj_param;
    [SerializeField] TextMeshProUGUI tmp_paramNow;
    [SerializeField] GameObject obj_vec;
    [SerializeField] TextMeshProUGUI tmp_paramNext;
    [SerializeField] TextMeshProUGUI tmp_level;
    [SerializeField] GameObject obj_complete;
    [SerializeField] GameObject obj_resourceRoot;
    [SerializeField] UI_ResourceCount[] ui_resourceCounts;


    private UI_SkillTreeUnit currentUnit;
    private List<ResourceCount> requredResources = new List<ResourceCount>();
    public List<ResourceCount> RequredResources => requredResources;
    public bool IsEnhanceReady { get; private set; } = true;



    public void SetData(UI_SkillTreeUnit _skillTreeUnit = null)
    {
        if (_skillTreeUnit == null)
        {
            this.gameObject.SetActive(false);
            currentUnit = null;
            return;
        }
        currentUnit = _skillTreeUnit;
        SetData_Base(currentUnit.level);
    }

    private void SetData_Base(int _currentLevel)
    {
        var so = currentUnit.skillTree;
        tmp_skillName.SetText(so.skillName);
        tmp_level.SetText($"<size=75%>Lv.</size>{_currentLevel} / <size=75%><color=black>{so.maxLevel}</color></size>");
        tmp_description.SetText(so.description);

        obj_param.SetActive(so.paramType != ParamType.Unlock);
        var paramNow = so.deltaValue * _currentLevel;
        var paramNext = so.deltaValue * (_currentLevel + 1);
        tmp_paramNow.SetText(paramNow.ToString("F2"));
        tmp_paramNext.SetText(paramNext.ToString("F2"));

        SetData_RequiredCost();

        tmp_paramNext.gameObject.SetActive(currentUnit.unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_vec.SetActive(currentUnit.unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_complete.SetActive(currentUnit.unlockState == SkillTreeUnlockState.EnhanceComplete);
        obj_resourceRoot.SetActive(currentUnit.unlockState != SkillTreeUnlockState.EnhanceComplete);
        this.gameObject.SetActive(true);
    }

    public void SetData_Enhanced(int _currentLevel)
    {
        SetData_Base(_currentLevel);
    }

    private void SetData_RequiredCost()
    {
        requredResources.Clear();
        IsEnhanceReady = true;

        foreach (var cont in ui_resourceCounts)
        {
            cont.NotActive();
        }
        var so = currentUnit.skillTreeUnit;
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Stone, requiredCount = so.req_stone });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Iron, requiredCount = so.req_iron });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Gold, requiredCount = so.req_gold });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Emerald, requiredCount = so.req_emerald });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Ruby, requiredCount = so.req_ruby });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Sapphire, requiredCount = so.req_sapphire });
        requredResources.Add(new ResourceCount() { resourceType = ResourceType.Diamond, requiredCount = so.req_diamond });

        var count = 0;
        foreach (var resource in requredResources)
        {
            if (resource.requiredCount <= 0) continue;


            var cont = ui_resourceCounts[count];
            if (!GameParamManager.blockChangeRateParam.IsBlockTypeUnlock(resource.resourceType))
            {
                cont.SetLock();
                IsEnhanceReady = false;
                Debug.Log($"Required Resource is not unlock: {resource.resourceType}");
            }
            else
            {
                var overResource = resource.requiredCount <= SaveLoader.Inst.Get_ResourceCount(resource.resourceType);
                cont.SetData(SOLoader.ItemData.GetItemUnitData((int)resource.resourceType).icon, resource.requiredCount.ToString(), overResource ? Color.white : Color.red);
                IsEnhanceReady = IsEnhanceReady && overResource;
            }
            count++;
        }
        IsEnhanceReady = IsEnhanceReady && currentUnit.level < currentUnit.skillTree.maxLevel;
        //btn_enhance.Set_Interactable(IsCraftReady);
    }
}
