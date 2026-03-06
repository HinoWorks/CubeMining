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

    private bool onceInitFin = false;



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
            var so = SOLoader.SkillTreeData.GetSkillTreeData(skillTreeUnit.skillIndex);
            if (so == null)
            {
                Debug.LogError($"SkillTreeData not found: {skillTreeUnit.skillIndex}");
                return;
            }
            skillTreeUnit.OnValidateCall(so);
        }

        // -- node set --
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            NodeCreate(skillTreeUnit);
        }

        EditorUtility.SetDirty(this);
    }

    private void NodeCreate(UI_SkillTreeUnit _unit)
    {
        if (_unit.skillTree.baseSkillIndex == null || _unit.skillTree.baseSkillIndex.Length == 0) return;

        // baseSkillIndex 配列の全ての要素と線を接続する
        foreach (var baseIndex in _unit.skillTree.baseSkillIndex)
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
            nodeCont = Instantiate(nodeContPrefab, nodeRoot).GetComponent<UI_SkillTreeNodeCont>();
            nodeConts.Add(nodeCont);
        }
        return nodeCont;
    }
#endif


    void OnceInit()
    {
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            // ランタイムでは常にSOから最新のSkillTreeを参照する（プレハブにコピーされた古い値でrequiredCountが0になるのを防ぐ）
            skillTreeUnit.skillTree = SOLoader.SkillTreeData.GetSkillTreeData(skillTreeUnit.skillIndex);
            if (skillTreeUnit.skillTree == null) continue;
            skillTreeUnit.AwakeCall(OnMouseOver, OnClick_Enhance, UpdateNodeState);
        }
        ui_skillTreeDetail.gameObject.SetActive(false);
        onceInitFin = true;
    }


    public void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.SkillTree;
        if (isActive)
        {
            if (!onceInitFin)
            {
                OnceInit();
            }
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
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            lastMousePos = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.rightButton.isPressed)
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
            ui_skillTreeDetail.transform.position = _skillTreeUnit.transform.position;
            ui_skillTreeDetail.SetData(_skillTreeUnit);
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
        if (_skillTreeUnit.unlockState != SkillTreeUnlockState.EnhanceReady) return;
        if (ui_skillTreeDetail.IsCraftReady == false) return;

        foreach (var resource in ui_skillTreeDetail.RequredResources)
        {
            // コスト消費
            if (resource.requiredCount <= 0) continue;
            SaveLoader.Inst.Request_SaveResource(resource.resourceType, -resource.requiredCount);
        }
        SaveLoader.Inst.Request_SaveSkillTreeData(_skillTreeUnit.skillIndex, _skillTreeUnit.level + 1);

        await UniTask.DelayFrame(2);
        _skillTreeUnit.Init();

        // ベーススキルの更新（baseSkillIndex 配列の中にこのスキルを含む全てのユニットを更新）
        var checkTargetUnit = Array.FindAll(skillTreeUnits,
            x => x.skillTree.baseSkillIndex != null &&
                 Array.Exists(x.skillTree.baseSkillIndex, idx => idx == _skillTreeUnit.skillIndex));
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