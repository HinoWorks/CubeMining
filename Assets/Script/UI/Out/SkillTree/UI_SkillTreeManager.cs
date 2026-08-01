using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEditor;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using UniRx;


public enum SkillTreeUnlockState
{
    Hide,
    Locked,
    EnhanceReady,
    EnhanceComplete
}

public class UI_SkillTreeMaanger : UI_OutGameTabBase
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


    private bool haveEnhanceReadyUnit = false;

    private Vector2 lastMousePos;
    private float duration_zoom = 0.05f;

    private state currentState = state.Idling;



#if UNITY_EDITOR
    [ContextMenu("SkillTreeData_Update")]
    public async void SkillTreeData_Update()
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
        await UniTask.DelayFrame(1);
        Debug.Log("<color=green>=== SkillTreeData_Update end ===</color>");
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





    public override void Start_OnceInit()
    {
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            // ランタイムでは常にSOから最新のSkillTreeを参照する（プレハブにコピーされた古い値でrequiredCountが0になるのを防ぐ）
            skillTreeUnit.AwakeCall(OnMouseOver, OnClick_Enhance, UpdateNodeState);
        }
        ui_skillTreeDetail.gameObject.SetActive(false);
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            skillTreeUnit.Init();
        }
        base.thisMenuType = OutGame_MenuType.SkillTree;
        GameEvent.UI.ResourceMod_OutGame.Subscribe(_ => Check_HaveEnhanceReadyUnit()).AddTo(this);
    }

    public override async void ToOutGame_InitData()
    {
        ui_skillTreeDetail.gameObject.SetActive(false);
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            skillTreeUnit.Init();
        }

        // リソース確認して、headerButtonのチェックマーク更新
        haveEnhanceReadyUnit = false;
        foreach (var unit in skillTreeUnits)
        {
            if (unit.unlockState == SkillTreeUnlockState.EnhanceReady && unit.isEnhanceReady)
            {
                haveEnhanceReadyUnit = true;
                break;
            }
        }
        UIManager_OutGame.Inst.Set_HeaderCheckMarkActiveState(OutGame_MenuType.SkillTree, haveEnhanceReadyUnit);

        base.isReloadFin = true;
    }


    private void Check_HaveEnhanceReadyUnit()
    {
        haveEnhanceReadyUnit = false;
        // 全てのunitにたいし、リソースチェックのみ行い、アップグレード可能を示す矢印を更新
        foreach (var unit in skillTreeUnits)
        {
            var isReady = unit.Set_UpgradeVector();
            if (isReady)
            {
                haveEnhanceReadyUnit = true;
                break;
            }
        }
        UIManager_OutGame.Inst.Set_HeaderCheckMarkActiveState(OutGame_MenuType.SkillTree, haveEnhanceReadyUnit);
    }



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
        return StaticManager.IsResourceEnough(requredResources);
    }


    protected override void Init_ActiveTab()
    {
        // 初期位置をリセット
        scrollContent.anchoredPosition = Vector2.zero;
    }




    #region -- update 主にズームと画面スクロール用 --
    void Update()
    {
        HandleZoom();
        HandlePan();
        Check_PositionReset();
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


    void Check_PositionReset()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            DOTween.To(() => scrollContent.anchoredPosition, x => scrollContent.anchoredPosition = x, Vector2.zero, 0.2f);
        }
    }
    #endregion

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
        if (currentState != state.Idling) return;
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
        
        currentState = state.Doing;
        // コスト消費
        foreach (var resource in ui_skillTreeDetail.RequredResources)
        {
            if (resource.requiredCount <= 0) continue;
            SaveLoader.Inst.Request_SaveResource(resource.resourceType, -resource.requiredCount);
        }
#endif

        var newLevel = _skillTreeUnit.level + 1;
        SaveLoader.Inst.Request_SaveSkillTreeData(_skillTreeUnit.skillIndex, newLevel);
        SoundManager.Inst.PlaySE(120);

        await UniTask.DelayFrame(3);
        _skillTreeUnit.CallBack_Enhance();
        ui_skillTreeDetail.SetData_Enhanced(newLevel);
        if (newLevel == 1)
        {
            GameEvent.AchieveEvent.PublishSkillTreeUnlock();
        }

        // ベーススキルの更新（baseSkillIndex 配列の中にこのスキルを含む全てのユニットを更新）
        var checkTargetUnit = Array.FindAll(skillTreeUnits,
            x => x.skillTreeUnit.unlockCheckIndexes != null &&
                 Array.Exists(x.skillTreeUnit.unlockCheckIndexes, idx => idx == _skillTreeUnit.skillIndex));
        foreach (var unit in checkTargetUnit)
        {
            unit.Init();
        }

        // 全てのunitにたいし、リソースチェックのみ行い、アップグレード可能を示す矢印を更新
        foreach (var unit in skillTreeUnits)
        {
            unit.Set_UpgradeVector();
        }
        GameEvent.UI.PublishResourceMod_OutGame();

        // 線の更新
        UpdateNodeState(_skillTreeUnit.skillIndex, _skillTreeUnit.unlockState, _skillTreeUnit.level + 1);

        // gameParamManager の更新
        GameParamManager.Set_DeltaParam(_skillTreeUnit.skillTree.paramCategory,
            _skillTreeUnit.skillTree.targetIndex, _skillTreeUnit.skillTree.paramType,
            _skillTreeUnit.skillTree.deltaValue, _skillTreeUnit.skillTree.deltaValue2);

        await UniTask.DelayFrame(2);
        currentState = state.Idling;
    }

    private void UpdateNodeState(int _skillIndex, SkillTreeUnlockState _unlockState, int _level)
    {
        var targetNodes = nodeConts.FindAll(x => Array.Exists(x.TargetSkillIndexes, idx => idx == _skillIndex));
        foreach (var node in targetNodes)
        {
            var unlockState_ref = SkillTreeUnlockState.Hide;
            var level_ref = 0;
            foreach (var targetIndex in node.TargetSkillIndexes)
            {
                if (targetIndex == _skillIndex) continue;
                var targetRefUnit = Array.Find(skillTreeUnits, x => x.skillIndex == targetIndex);
                if (targetRefUnit == null) continue;
                unlockState_ref = targetRefUnit.unlockState;
                level_ref = targetRefUnit.level;
            }
            node.Set_LineState(_unlockState, _level, unlockState_ref, level_ref);
        }
    }

    public SkillTreeUnlockState Get_SkillTreeUnlockState(int _skillIndex)
    {
        var skillTreeUnit = Array.Find(skillTreeUnits, x => x.skillIndex == _skillIndex);
        if (skillTreeUnit == null) return SkillTreeUnlockState.Hide;
        return skillTreeUnit.unlockState;
    }


}