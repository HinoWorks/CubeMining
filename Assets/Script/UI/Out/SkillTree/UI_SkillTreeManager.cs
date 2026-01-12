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
        if (_unit.skillTree.baseSkillIndex == -1) return;
        var baseUnit = Array.Find(skillTreeUnits, x => x.skillIndex == _unit.skillTree.baseSkillIndex);
        if (baseUnit == null) return;

        var nodeCont = Get_FreeNodeCont();
        nodeCont.SetNodeCont(baseUnit, _unit, nodeLineHeight);
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



    void Awake()
    {
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            skillTreeUnit.AwakeCall(OnMouseOver, OnClick_Enhance, UpdateNodeState);
        }
        ui_skillTreeDetail.gameObject.SetActive(false);
    }


    public void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.SkillTree;
        this.gameObject.SetActive(isActive);
        ui_skillTreeDetail.gameObject.SetActive(false);
        if (!isActive) return;

        foreach (var skillTreeUnit in skillTreeUnits)
        {
            skillTreeUnit.Init();
        }

        // 実行時に接続線を更新
        // UpdateAllConnections();
    }

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

        float scaleRatio = newScale / oldScale;
        // scrollContent.localScale = Vector3.one * newScale;
        // マウス位置基準ズーム
        //scrollContent.anchoredPosition -= localMousePos * (scaleRatio - 1f);
        //DOTween.To(() => scrollContent.localScale, x => scrollContent.localScale = x, Vector3.one * newScale, duration_zoom);
        //DOTween.To(() => scrollContent.anchoredPosition, x => scrollContent.anchoredPosition = x, scrollContent.anchoredPosition - localMousePos * (scaleRatio - 1f), duration_zoom);
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
        if (StaticManager.CoinCheck(_skillTreeUnit.skillTree.cost) == false) return;
        SaveLoader.Inst.Request_SaveSkillTreeData(_skillTreeUnit.skillIndex, _skillTreeUnit.level + 1);
        SaveLoader.Inst.Request_SaveCoin(-_skillTreeUnit.skillTree.cost);

        await UniTask.DelayFrame(2);
        _skillTreeUnit.Init();

        // ベーススキルの更新
        var checkTargetUnit = Array.FindAll(skillTreeUnits, x => x.skillTree.baseSkillIndex == _skillTreeUnit.skillIndex);
        foreach (var unit in checkTargetUnit)
        {
            unit.Init();
        }
        ui_skillTreeDetail.SetData_Enhanced(_skillTreeUnit.level + 1);
        UpdateNodeState(_skillTreeUnit.skillTree.baseSkillIndex, _skillTreeUnit.skillIndex, _skillTreeUnit.unlockState, _skillTreeUnit.level + 1);
    }

    private void UpdateNodeState(int _baseSkillIndex, int _targetSkillIndex, SkillTreeUnlockState _unlockState, int _level)
    {
        var targetNodes = nodeConts.FindAll(x => x.BaseSkillIndex == _baseSkillIndex && x.TargetSkillIndex == _targetSkillIndex);
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