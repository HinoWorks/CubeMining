using UnityEngine;
using UnityEngine.UI;
using System;



public class UI_SkillTreeUnit : MonoBehaviour
{
    [SerializeField] Image image_icon;
    [SerializeField] GameObject obj_lock;
    [SerializeField] GameObject obj_enhanceReady;
    [SerializeField] GameObject obj_complete;
    [SerializeField] HButton button;

    public Action<bool, UI_SkillTreeUnit> onMouseOver;
    /*
        private SkillTreeUnitData data;
        public void SetData(SkillTreeUnitData _data)
        {
            data = _data;
            image_icon.sprite = data.icon;
            obj_lock.SetActive(data.isLock);
            obj_enhanceReady.SetActive(data.isEnhanceReady);
            obj_complete.SetActive(data.isComplete);
        }

    */

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
