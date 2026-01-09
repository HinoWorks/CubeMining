using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class UI_SkillTreeUnit : MonoBehaviour
{
    public int skillIndex;
    public SkillTree skillTree;
    public int level { get; private set; } = 0;
    public SkillTreeUnlockState unlockState;// { get; private set; }


    [Space(10)]
    [Header("Connect")]
    [SerializeField] Image image_icon;
    [SerializeField] GameObject obj_lock;
    [SerializeField] GameObject obj_enhanceReady;
    [SerializeField] GameObject obj_complete;
    [SerializeField] HButton button;

    [Space(10)]
    [Header("DEBUG VIEW")]
    [SerializeField] GameObject obj_debug;
    [SerializeField] TextMeshProUGUI tmp_debug;


    private Action<bool, UI_SkillTreeUnit> onMouseOver;
    private Action<UI_SkillTreeUnit> onClick_Enhance;
    private Action<int, int, SkillTreeUnlockState, int> onUpdateNodeState;



#if UNITY_EDITOR
    public void OnValidateCall(SkillTree _skillTree)
    {
        skillTree = _skillTree;
        image_icon.sprite = skillTree.icon;
        obj_debug.SetActive(true);
        tmp_debug.SetText($"{skillIndex}");
    }
#endif



    public void AwakeCall(Action<bool, UI_SkillTreeUnit> _onMouseOver,
                            Action<UI_SkillTreeUnit> _onClick_Enhance,
                            Action<int, int, SkillTreeUnlockState, int> _onUpdateNodeState)
    {
        this.onMouseOver = _onMouseOver;
        button.onMouseOver += OnPointerEnter;
        this.onClick_Enhance = _onClick_Enhance;
        this.onUpdateNodeState = _onUpdateNodeState;
        obj_debug.SetActive(false);
    }

    public async void Init()
    {
        var skillTreeData = await SaveLoader.Inst.Get_SkillTreeData(skillIndex);
        if (skillTree.baseSkillIndex == -1) //初期スキルのみ
        {
            if (skillTreeData == null)
            {
                unlockState = SkillTreeUnlockState.EnhanceReady;
            }
            else
            {
                level = skillTreeData.level;
                unlockState = skillTreeData.level >= skillTree.maxLevel ?
                 SkillTreeUnlockState.EnhanceComplete : SkillTreeUnlockState.EnhanceReady;
            }
        }
        else if (skillTreeData == null) //データない場合、ベーススキルを確認
        {
            var baseSkillUnitState = UIManager_OutGame.Inst.UI_SkillTreeManager.Get_SkillTreeUnlockState(skillTree.baseSkillIndex);
            switch (baseSkillUnitState)
            {
                case SkillTreeUnlockState.Hide:
                case SkillTreeUnlockState.Locked:
                    unlockState = SkillTreeUnlockState.Hide;
                    break;
                case SkillTreeUnlockState.EnhanceReady:
                    var baseSkillData = await SaveLoader.Inst.Get_SkillTreeData(skillTree.baseSkillIndex);
                    unlockState = baseSkillData == null ? SkillTreeUnlockState.Locked : SkillTreeUnlockState.EnhanceReady;
                    break;
                case SkillTreeUnlockState.EnhanceComplete:
                    unlockState = SkillTreeUnlockState.EnhanceReady;
                    break;
            }
        }
        else //データありの場合、レベルを確認
        {
            level = skillTreeData.level;
            unlockState = skillTreeData.level >= skillTree.maxLevel ?
                 SkillTreeUnlockState.EnhanceComplete : SkillTreeUnlockState.EnhanceReady;
        }
        // Debug.Log($"SkillTreeUnit: {skillIndex} ----> unlockState: {unlockState}");
        onUpdateNodeState?.Invoke(skillTree.baseSkillIndex, skillIndex, unlockState, level);
        SetState();

    }


    private void SetState()
    {
        image_icon.enabled = unlockState == SkillTreeUnlockState.EnhanceReady || unlockState == SkillTreeUnlockState.EnhanceComplete;
        obj_lock.SetActive(unlockState == SkillTreeUnlockState.Locked);
        obj_enhanceReady.SetActive(unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_complete.SetActive(unlockState == SkillTreeUnlockState.EnhanceComplete);

        button.gameObject.SetActive(unlockState != SkillTreeUnlockState.Hide);
    }


    private void OnPointerEnter(bool _isEnter)
    {
        onMouseOver?.Invoke(_isEnter, this);
    }

    #region onClick
    public void OnClick_Enhance()
    {
        onClick_Enhance?.Invoke(this);
    }
    #endregion
}
