using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillTreeDetail : MonoBehaviour
{
    //[SerializeField] Image image_icon;
    [SerializeField] TextMeshProUGUI tmp_skillName;
    [SerializeField] TextMeshProUGUI tmp_description;
    [SerializeField] TextMeshProUGUI tmp_paramNow;
    [SerializeField] GameObject obj_vec;
    [SerializeField] TextMeshProUGUI tmp_paramNext;
    [SerializeField] TextMeshProUGUI tmp_level;
    [SerializeField] TextMeshProUGUI tmp_cost;
    [SerializeField] GameObject obj_complete;

    private UI_SkillTreeUnit currentUnit;



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

    private void SetData_Base(int _currentLevel)
    {
        var so = currentUnit.skillTree;
        tmp_skillName.SetText(so.skillName);
        tmp_level.SetText($"Lv.{_currentLevel} <size=75%>/ {so.maxLevel}</size>");
        tmp_description.SetText(so.description);
        var paramNow = so.baseValue + so.deltaValue * _currentLevel;
        var paramNext = so.baseValue + so.deltaValue * (_currentLevel + 1);
        tmp_paramNow.SetText(paramNow.ToString("F2"));
        tmp_paramNext.SetText(paramNext.ToString("F2"));
        tmp_cost.SetText(so.cost.ToString());
        tmp_cost.color = StaticManager.CoinCheck(so.cost) ? Color.white : Color.red;

        tmp_paramNext.gameObject.SetActive(currentUnit.unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_vec.SetActive(currentUnit.unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_complete.SetActive(currentUnit.unlockState == SkillTreeUnlockState.EnhanceComplete);

        this.gameObject.SetActive(true);
    }

    public void SetData_Enhanced(int _currentLevel)
    {
        SetData_Base(_currentLevel);




    }


}
