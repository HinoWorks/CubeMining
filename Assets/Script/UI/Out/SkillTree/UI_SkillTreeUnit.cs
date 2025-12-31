using UnityEngine;
using UnityEngine.UI;
using System;


public class UI_SkillTreeUnit : MonoBehaviour
{
    public int skillIndex;
    public SkillTree skillTree;
    public int level { get; private set; } = 0;
    public SkillTreeUnlockState unlockState;


    [Space(10)]
    [Header("Connect")]
    [SerializeField] Image image_icon;
    [SerializeField] GameObject obj_lock;
    [SerializeField] GameObject obj_enhanceReady;
    [SerializeField] GameObject obj_complete;
    [SerializeField] HButton button;

    private Action<bool, UI_SkillTreeUnit> onMouseOver;
    private Action<UI_SkillTreeUnit> onClick_Enhance;
    private Action<int, bool> onUpdateNodeState;



#if UNITY_EDITOR
    public void OnValidateCall(SkillTree _skillTree)
    {
        skillTree = _skillTree;
        image_icon.sprite = skillTree.icon;
    }
#endif



    public void AwakeCall(Action<bool, UI_SkillTreeUnit> _onMouseOver,
                            Action<UI_SkillTreeUnit> _onClick_Enhance,
                            Action<int, bool> _onUpdateNodeState)
    {
        this.onMouseOver = _onMouseOver;
        button.onMouseOver += OnPointerEnter;
        this.onClick_Enhance = _onClick_Enhance;
        this.onUpdateNodeState = _onUpdateNodeState;

        Init();
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
            var baseSkillData = await SaveLoader.Inst.Get_SkillTreeData(skillTree.baseSkillIndex);
            unlockState = baseSkillData == null ? SkillTreeUnlockState.Locked : SkillTreeUnlockState.EnhanceReady;
        }
        else //データありの場合、レベルを確認
        {
            level = skillTreeData.level;
            unlockState = skillTreeData.level >= skillTree.maxLevel ?
                 SkillTreeUnlockState.EnhanceComplete : SkillTreeUnlockState.EnhanceReady;
        }
        Debug.Log($"SkillTreeUnit: {skillIndex} ----> unlockState: {unlockState}");
        onUpdateNodeState?.Invoke(skillTree.baseSkillIndex, unlockState != SkillTreeUnlockState.Locked);
        SetState();

    }


    private void SetState()
    {
        image_icon.enabled = unlockState != SkillTreeUnlockState.Locked;
        obj_lock.SetActive(unlockState == SkillTreeUnlockState.Locked);
        obj_enhanceReady.SetActive(unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_complete.SetActive(unlockState == SkillTreeUnlockState.EnhanceComplete);
        button.interactable = unlockState == SkillTreeUnlockState.EnhanceReady;
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
