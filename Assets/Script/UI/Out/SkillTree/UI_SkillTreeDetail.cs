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
    [SerializeField] float hoverYOffset = 200f;


    private UI_SkillTreeUnit currentUnit;
    private List<ResourceCount> requredResources = new List<ResourceCount>();
    public List<ResourceCount> RequredResources => requredResources;
    public bool IsEnhanceReady => resourceReady && !isMaxLevel;
    private bool isMaxLevel = false;
    private bool resourceReady = false;

    private RectTransform rectTr;
    private Canvas rootCanvas;
    private Vector3 anchorWorldPosition;



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

    public void SetPositionWithAutoFlip(Vector3 _worldPosition)
    {
        EnsureCachedRefs();
        anchorWorldPosition = _worldPosition;
        ApplyVerticalOffset(hoverYOffset);

        // サイズ更新後に判定しないと、初回だけ正しく判定できない場合がある
        Canvas.ForceUpdateCanvases();
        if (IsAnyCornerOutOfScreen())
        {
            ApplyVerticalOffset(-hoverYOffset);
        }
    }

    private void SetData_Base(int _currentLevel)
    {
        if (currentUnit == null) return;
        Debug.Log("Unit Detail Update Method -- current level: " + _currentLevel);

        var so = currentUnit.skillTree;
        tmp_skillName.SetText(so.skillName);
        tmp_level.SetText($"<size=75%>Lv.</size>{_currentLevel} <size=75%> / {so.maxLevel}</size>");
        tmp_description.SetText(so.description);

        var isParamON = so.unit != "NA";
        obj_param.SetActive(so.unit != "NA");

        if (isParamON)
        {
            //var setParam_1 = "";
            //var setParam_2 = "";

            var paramNow = so.deltaValue * _currentLevel;
            var paramNext = so.deltaValue * (_currentLevel + 1);
            tmp_paramNow.SetText(SetParamText(so.unit, paramNow));
            tmp_paramNext.SetText(SetParamText(so.unit, paramNext));
        }

        //tmp_paramNow.SetText(paramNow.ToString("F2"));
        ///tmp_paramNext.SetText(paramNext.ToString("F2"));

        SetData_RequiredCost(_currentLevel);
        isMaxLevel = _currentLevel >= so.maxLevel;

        tmp_paramNext.gameObject.SetActive(!isMaxLevel);
        obj_vec.SetActive(!isMaxLevel);
        obj_complete.SetActive(isMaxLevel);
        obj_resourceRoot.SetActive(!isMaxLevel);
        this.gameObject.SetActive(true);
    }

    private string SetParamText(string _unit, float _value)
    {
        string setParam = "";
        switch (_unit)
        {
            case "%":
                setParam = $"+{(_value * 100).ToString("F1")}%";
                break;
            case "Sec":
                setParam = $"+{_value.ToString("F0")}Sec";
                break;
            case "Count":
                setParam = $"+{_value.ToString("F0")}";
                break;
            case "NA":
                // 非表示　ここには来ない
                break;
        }
        return setParam;
    }

    private void EnsureCachedRefs()
    {
        if (rectTr == null) rectTr = transform as RectTransform;
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
    }

    private void ApplyVerticalOffset(float _offsetY)
    {
        var targetScreenPos = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(), anchorWorldPosition);
        targetScreenPos.y += _offsetY;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTr.parent as RectTransform,
            targetScreenPos,
            GetCanvasCamera(),
            out var worldPos
        );
        rectTr.position = worldPos;
    }

    private Camera GetCanvasCamera()
    {
        if (rootCanvas == null) return null;
        return rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
    }

    private bool IsAnyCornerOutOfScreen()
    {
        EnsureCachedRefs();
        var corners = new Vector3[4];
        rectTr.GetWorldCorners(corners);
        var cam = GetCanvasCamera();
        for (var i = 0; i < corners.Length; i++)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(cam, corners[i]);
            if (screenPoint.x < 0f || screenPoint.x > Screen.width || screenPoint.y < 0f || screenPoint.y > Screen.height)
            {
                return true;
            }
        }
        return false;
    }

    public void SetData_Enhanced(int _currentLevel)
    {
        Debug.Log("Unit Detail -- enhanced level: " + _currentLevel);
        SetData_Base(_currentLevel);
    }

    private void SetData_RequiredCost(int _level)
    {
        requredResources.Clear();
        resourceReady = true;

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
                resourceReady = false;
                //Debug.Log($"Required Resource is not unlock: {resource.resourceType}");
            }
            else
            {
                var overResource = resource.requiredCount <= SaveLoader.Inst.Get_ResourceCount(resource.resourceType);
                cont.SetData(SOLoader.ItemData.GetItemUnitData((int)resource.resourceType).icon, resource.requiredCount.ToString(), overResource ? Color.white : Color.red);
                resourceReady = resourceReady && overResource;
            }
            count++;
        }
    }
}
