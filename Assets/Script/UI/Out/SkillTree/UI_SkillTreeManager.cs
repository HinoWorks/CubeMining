using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEditor;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;


public enum SkillTreeUnlockState
{
    Hide,
    Locked,
    EnhanceReady,
    EnhanceComplete
}

public class UI_SkillTreeMaanger : MonoBehaviour
{
    public UI_SkillTreeUnit[] skillTreeUnits;
    [SerializeField] UI_SkillTreeDetail ui_skillTreeDetail;

    [Space(10)]
    [Header("Node settings")]
    [SerializeField] GameObject nodeContPrefab;
    [SerializeField] List<UI_SkillTreeNodeCont> nodeConts = new List<UI_SkillTreeNodeCont>();
    [SerializeField] RectTransform nodeRoot;
    [SerializeField] float nodeLineHeight = 4f;


    [Space(10)]
    [Header("Scroll settings")]
    [SerializeField] RectTransform scrollContent;
    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 1.0f;

    private Vector2 lastMousePos;
    private float duration_zoom = 0.05f;



#if UNITY_EDITOR
    [ContextMenu("SkillTreeData_Update")]
    public void SkillTreeData_Update()
    {
        foreach (var nodeCont in nodeConts)
        {
            nodeCont.NotActivate();
        }

        // -- unit set --
        skillTreeUnits = transform.GetComponentsInChildren<UI_SkillTreeUnit>();
        foreach (var skillTreeUnit in skillTreeUnits)
        {

            var so_unit = SOLoader.SkillTreeData.GetSkillTreeUnitData(skillTreeUnit.skillIndex);
            if (so_unit == null)
            {
                Debug.LogError($"SkillTreeUnit not found: {skillTreeUnit.skillIndex}");
                return;
            }
            var so_base = SOLoader.SkillTreeData.GetSkillTreeBaseData(so_unit.refIndex);
            if (so_base == null)
            {
                Debug.LogError($"SkillTreeBase not found: {skillTreeUnit.skillIndex}");
                return;
            }

            skillTreeUnit.OnValidateCall(so_base, so_unit);
        }

        // -- node set --
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            //Debug.Log($"NodeCreate: {skillTreeUnit.skillIndex}");
            NodeCreate(skillTreeUnit);
        }

        EditorUtility.SetDirty(this);
    }

    private void NodeCreate(UI_SkillTreeUnit _unit)
    {
        if (_unit.skillTreeUnit.unlockCheckIndexes == null || _unit.skillTreeUnit.unlockCheckIndexes.Length == 0) return;

        // baseSkillIndex 配列の全ての要素と線を接続する
        foreach (var baseIndex in _unit.skillTreeUnit.unlockCheckIndexes)
        {
            if (baseIndex == -1) continue;
            var baseUnit = Array.Find(skillTreeUnits, x => x.skillIndex == baseIndex);
            if (baseUnit == null) continue;

            var nodeCont = Get_FreeNodeCont();
            nodeCont.SetNodeCont(baseUnit, _unit, nodeLineHeight);
        }
    }

    private UI_SkillTreeNodeCont Get_FreeNodeCont()
    {
        var nodeCont = nodeConts.Find(x => x.gameObject.activeSelf == false);
        if (nodeCont == null)
        {
            var newCont = PrefabUtility.InstantiatePrefab(nodeContPrefab, nodeRoot) as GameObject;
            nodeCont = newCont.GetComponent<UI_SkillTreeNodeCont>();
            nodeConts.Add(nodeCont);
        }
        return nodeCont;
    }
#endif


    public void Start_OnceInit()
    {
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            // ランタイムでは常にSOから最新のSkillTreeを参照する（プレハブにコピーされた古い値でrequiredCountが0になるのを防ぐ）
            //skillTreeUnit.skillTree = SOLoader.SkillTreeData.GetSkillTreeData(skillTreeUnit.skillIndex);
            //if (skillTreeUnit.skillTree == null) continue;
            skillTreeUnit.AwakeCall(OnMouseOver, OnClick_Enhance, UpdateNodeState);
        }
        ui_skillTreeDetail.gameObject.SetActive(false);
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            skillTreeUnit.Init();
        }
    }


    public void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.SkillTree;
        if (isActive)
        {
            ui_skillTreeDetail.gameObject.SetActive(false);
            foreach (var skillTreeUnit in skillTreeUnits)
            {
                skillTreeUnit.Init();
            }
        }
        this.gameObject.SetActive(isActive);


        // 実行時に接続線を更新
        // UpdateAllConnections();
    }

    /*
    /// <summary>
    /// 全ての接続線を更新する
    /// </summary>
    private void UpdateAllConnections()
    {
        foreach (var nodeCont in nodeConts)
        {
            if (!nodeCont.gameObject.activeSelf) continue;
            nodeCont.UpdateConnection();
        }
    }
*/

    public bool IsResourceEnough(SkillTreeUnit _skillTreeUnit)
    {
        var requredResources = new ResourceCount[7];
        requredResources[0] = new ResourceCount() { resourceType = ResourceType.Stone, requiredCount = _skillTreeUnit.req_stone };
        requredResources[1] = new ResourceCount() { resourceType = ResourceType.Iron, requiredCount = _skillTreeUnit.req_iron };
        requredResources[2] = new ResourceCount() { resourceType = ResourceType.Gold, requiredCount = _skillTreeUnit.req_gold };
        requredResources[3] = new ResourceCount() { resourceType = ResourceType.Emerald, requiredCount = _skillTreeUnit.req_emerald };
        requredResources[4] = new ResourceCount() { resourceType = ResourceType.Ruby, requiredCount = _skillTreeUnit.req_ruby };
        requredResources[5] = new ResourceCount() { resourceType = ResourceType.Sapphire, requiredCount = _skillTreeUnit.req_sapphire };
        requredResources[6] = new ResourceCount() { resourceType = ResourceType.Diamond, requiredCount = _skillTreeUnit.req_diamond };

        var isEnough = true;
        foreach (var resource in requredResources)
        {
            if (resource.requiredCount <= 0) continue;
            if (resource.requiredCount > SaveLoader.Inst.Get_ResourceCount(resource.resourceType))
            {
                isEnough = false;
                break;
            }
        }
        return isEnough;
    }


    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    // ---------- Zoom ----------
    void HandleZoom()
    {
        float wheel = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(wheel) < 0.01f) return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            scrollContent,
            Mouse.current.position.ReadValue(),
            null,
            out localMousePos
        );

        float oldScale = scrollContent.localScale.x;
        float newScale = Mathf.Clamp(oldScale + wheel * zoomSpeed, minScale, maxScale);
        if (Mathf.Approximately(oldScale, newScale)) return;

        // マウス位置を固定したままズーム: コンテンツ上の点 localMousePos が画面で動かないように position を補正
        Vector2 posDelta = localMousePos * (oldScale - newScale);
        Vector2 targetPos = scrollContent.anchoredPosition + posDelta;

        scrollContent.localScale = Vector3.one * newScale;
        scrollContent.anchoredPosition = targetPos;
        DOTween.To(() => scrollContent.localScale, x => scrollContent.localScale = x, Vector3.one * newScale, duration_zoom);
        DOTween.To(() => scrollContent.anchoredPosition, x => scrollContent.anchoredPosition = x, targetPos, duration_zoom);
    }

    // ---------- Pan ----------
    void HandlePan()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            lastMousePos = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 current = Mouse.current.position.ReadValue();
            Vector2 delta = current - lastMousePos;
            //scrollContent.anchoredPosition += delta * panSpeed;
            DOTween.To(() => scrollContent.anchoredPosition, x => scrollContent.anchoredPosition = x, scrollContent.anchoredPosition + delta * panSpeed, duration_zoom);
            lastMousePos = current;
        }
    }




    /// <summary>
    /// unit にマウスオーバーした時の処理
    /// </summary>
    private void OnMouseOver(bool _isEnter, UI_SkillTreeUnit _skillTreeUnit)
    {
        if (_isEnter)
        {
            ui_skillTreeDetail.SetData(_skillTreeUnit);
            ui_skillTreeDetail.SetPositionWithAutoFlip(_skillTreeUnit.transform.position);
        }
        else
        {
            ui_skillTreeDetail.SetData(null);
        }
    }


    /// <summary>
    /// unit をクリックした時の処理
    /// </summary>
    private async void OnClick_Enhance(UI_SkillTreeUnit _skillTreeUnit)
    {
#if UNITY_EDITOR
        if (SROptions.isSkillTreeUpgradeNoMaterial)
        {
            Debug.Log("SkillTreeUpgradeNoMaterial");
        }
        else
        {
            if (ui_skillTreeDetail.IsEnhanceReady == false) return;
            foreach (var resource in ui_skillTreeDetail.RequredResources)
            {
                if (resource.requiredCount <= 0) continue;
                SaveLoader.Inst.Request_SaveResource(resource.resourceType, -resource.requiredCount);
            }
        }
#else
        if (ui_skillTreeDetail.IsEnhanceReady == false) return;
        // コスト消費
        foreach (var resource in ui_skillTreeDetail.RequredResources)
        {
            if (resource.requiredCount <= 0) continue;
            SaveLoader.Inst.Request_SaveResource(resource.resourceType, -resource.requiredCount);
        }
#endif

        SaveLoader.Inst.Request_SaveSkillTreeData(_skillTreeUnit.skillIndex, _skillTreeUnit.level + 1);
        SoundManager.Inst.PlaySE(120);

        await UniTask.DelayFrame(2);
        _skillTreeUnit.Init();
        _skillTreeUnit.CallBack_Enhance();
        // ベーススキルの更新（baseSkillIndex 配列の中にこのスキルを含む全てのユニットを更新）
        var checkTargetUnit = Array.FindAll(skillTreeUnits,
            x => x.skillTreeUnit.unlockCheckIndexes != null &&
                 Array.Exists(x.skillTreeUnit.unlockCheckIndexes, idx => idx == _skillTreeUnit.skillIndex));
        foreach (var unit in checkTargetUnit)
        {
            unit.Init();
        }
        ui_skillTreeDetail.SetData_Enhanced(_skillTreeUnit.level + 1);
        // 線の更新は「ターゲットスキルID」だけ見て行う
        UpdateNodeState(-1, _skillTreeUnit.skillIndex, _skillTreeUnit.unlockState, _skillTreeUnit.level + 1);

        // gameParamManager の更新
        GameParamManager.Set_DeltaParam(_skillTreeUnit.skillTree.paramCategory,
            _skillTreeUnit.skillTree.targetIndex, _skillTreeUnit.skillTree.paramType, _skillTreeUnit.skillTree.deltaValue);
    }

    private void UpdateNodeState(int _baseSkillIndex, int _targetSkillIndex, SkillTreeUnlockState _unlockState, int _level)
    {
        // baseSkillIndex は見ずに、対象スキル（ターゲット）の線を全て更新する
        var targetNodes = nodeConts.FindAll(x => x.TargetSkillIndex == _targetSkillIndex);
        foreach (var node in targetNodes)
        {
            node.Set_LineState(_unlockState, _level);
        }
    }

    public SkillTreeUnlockState Get_SkillTreeUnlockState(int _skillIndex)
    {
        var skillTreeUnit = Array.Find(skillTreeUnits, x => x.skillIndex == _skillIndex);
        if (skillTreeUnit == null) return SkillTreeUnlockState.Hide;
        return skillTreeUnit.unlockState;
    }



}