using UnityEngine;
using UnityEngine.UI;
using System;


public class UI_SkillTreeUnit : MonoBehaviour
{
    public int skillIndex;
    public SkillTree skillTree;


    [Space(10)]
    [Header("Connect")]
    [SerializeField] Image image_icon;
    [SerializeField] GameObject obj_lock;
    [SerializeField] GameObject obj_enhanceReady;
    [SerializeField] GameObject obj_complete;
    [SerializeField] HButton button;

    public Action<bool, UI_SkillTreeUnit> onMouseOver;





#if UNITY_EDITOR
    public void OnValidateCall(SkillTree _skillTree)
    {
        skillTree = _skillTree;
        image_icon.sprite = skillTree.icon;
    }

#endif



    public void AwakeCall(Action<bool, UI_SkillTreeUnit> _onMouseOver)
    {
        this.onMouseOver = _onMouseOver;
        button.onMouseOver += OnPointerEnter;
    }


    private void OnPointerEnter(bool _isEnter)
    {
        onMouseOver?.Invoke(_isEnter, this);
    }

    #region onClick
    public void OnClick_Enhance()
    {
        Debug.Log("OnClick_Enhance");
    }
    #endregion
}
