using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Cysharp.Threading.Tasks;


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
    [SerializeField] GameObject obj_level0;
    [SerializeField] GameObject obj_upgradeable;
    [SerializeField] ParticleSystem eff_enhance;
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
        // ベーススキルが一切無い場合のみ「初期スキル」とみなす
        if (!HasAnyBaseSkill()) //初期スキルのみ
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
        else if (skillTreeData == null) //データない場合、ベーススキル群を確認
        {
            // 複数のベーススキルのいずれかを取得していれば解放
            var isAnyBaseAcquired = await IsAnyBaseSkillAcquired();
            unlockState = isAnyBaseAcquired ? SkillTreeUnlockState.EnhanceReady : SkillTreeUnlockState.Hide;
        }
        else //データありの場合、レベルを確認
        {
            obj_level0.SetActive(skillTreeData.level == 0);
            level = skillTreeData.level;
            unlockState = skillTreeData.level >= skillTree.maxLevel ?
                 SkillTreeUnlockState.EnhanceComplete : SkillTreeUnlockState.EnhanceReady;
        }
        // Debug.Log($"SkillTreeUnit: {skillIndex} ----> unlockState: {unlockState}");
        // 線の更新は UI_SkillTreeManager 側で TargetSkillIndex のみを見て行うため、
        // 第1引数（baseSkillIndex）はダミー値で良い
        onUpdateNodeState?.Invoke(-1, skillIndex, unlockState, level);
        SetState();
    }


    /// <summary>
    /// 何かしらのベーススキルを持っているかどうか
    /// </summary>
    private bool HasAnyBaseSkill()
    {
        return skillTree.baseSkillIndex != null && skillTree.baseSkillIndex[0] != -1;
    }

    /// <summary>
    /// 複数候補のベーススキルのうち、いずれか 1 つでも「取得済み」かどうかを判定する
    /// （レベル 1 以上、もしくは EnhanceComplete）
    /// </summary>
    private async UniTask<bool> IsAnyBaseSkillAcquired()
    {
        if (skillTree.baseSkillIndex == null || skillTree.baseSkillIndex.Length == 0) return false;

        // baseSkillIndex 配列のどれか 1 つでも取得済みであれば OK
        foreach (var idx in skillTree.baseSkillIndex)
        {
            if (idx == -1) continue;
            if (await IsSingleBaseSkillAcquired(idx)) return true;
        }

        return false;
    }

    /// <summary>
    /// 単一のベーススキルが取得済みかどうか判定する
    /// </summary>
    private async UniTask<bool> IsSingleBaseSkillAcquired(int baseSkillIndex)
    {
        var baseSkillUnitState = UIManager_OutGame.Inst.UI_SkillTreeManager.Get_SkillTreeUnlockState(baseSkillIndex);
        switch (baseSkillUnitState)
        {
            case SkillTreeUnlockState.Hide:
            case SkillTreeUnlockState.Locked:
                return false;
            case SkillTreeUnlockState.EnhanceReady:
                // EnhanceReady でも、実際にレベル 0 の場合はまだ未取得なので NG
                var baseSkillData = await SaveLoader.Inst.Get_SkillTreeData(baseSkillIndex);
                return baseSkillData != null && baseSkillData.level > 0;
            case SkillTreeUnlockState.EnhanceComplete:
                return true;
        }
        return false;
    }


    private void SetState()
    {
        image_icon.enabled = unlockState == SkillTreeUnlockState.EnhanceReady || unlockState == SkillTreeUnlockState.EnhanceComplete;
        obj_lock.SetActive(unlockState == SkillTreeUnlockState.Locked);
        obj_enhanceReady.SetActive(unlockState != SkillTreeUnlockState.EnhanceReady || unlockState != SkillTreeUnlockState.EnhanceComplete);
        obj_complete.SetActive(unlockState == SkillTreeUnlockState.EnhanceComplete);

        button.gameObject.SetActive(unlockState != SkillTreeUnlockState.Hide);
        button.Set_Interactable(unlockState == SkillTreeUnlockState.EnhanceReady
            || unlockState == SkillTreeUnlockState.EnhanceComplete);


        // アップグレード可能を示す矢印を表示
        if (unlockState == SkillTreeUnlockState.EnhanceReady)
        {
            obj_upgradeable.SetActive(UIManager_OutGame.Inst.UI_SkillTreeManager.IsResourceEnough(skillTree));
        }
        else
        {
            obj_upgradeable.SetActive(false);
        }
    }


    public void CallBack_Enhance()
    {
        eff_enhance.Play();
    }
    private void OnPointerEnter(bool _isEnter)
    {
        if (this.unlockState == SkillTreeUnlockState.Hide || this.unlockState == SkillTreeUnlockState.Locked) return;
        onMouseOver?.Invoke(_isEnter, this);
    }

    #region onClick
    public void OnClick_Enhance()
    {
        if (this.unlockState != SkillTreeUnlockState.EnhanceReady) return;
        onClick_Enhance?.Invoke(this);
    }
    #endregion
}
