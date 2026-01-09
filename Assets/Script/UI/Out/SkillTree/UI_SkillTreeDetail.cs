using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillTreeDetail : MonoBehaviour
{
    [SerializeField] Image image_icon;
    [SerializeField] TextMeshProUGUI tmp_skillName;
    //[SerializeField] TextMeshProUGUI tmp_description;
    [SerializeField] TextMeshProUGUI tmp_paramNow;
    [SerializeField] GameObject obj_vec;
    [SerializeField] TextMeshProUGUI tmp_paramNext;
    [SerializeField] TextMeshProUGUI tmp_level;
    [SerializeField] TextMeshProUGUI tmp_cost;
    [SerializeField] GameObject obj_complete;




    public void SetData(UI_SkillTreeUnit _skillTreeUnit = null)
    {
        if (_skillTreeUnit == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        var so = _skillTreeUnit.skillTree;

        image_icon.sprite = so.icon;
        tmp_skillName.SetText(so.skillName);
        tmp_level.SetText($"Lv.{_skillTreeUnit.level} <size=75%>/ {so.maxLevel}</size>");
        var paramNow = so.baseValue + so.deltaValue * _skillTreeUnit.level;
        var paramNext = so.baseValue + so.deltaValue * (_skillTreeUnit.level + 1);
        tmp_paramNow.SetText(paramNow.ToString("F2"));
        tmp_paramNext.SetText(paramNext.ToString("F2"));
        tmp_cost.SetText(so.cost.ToString());
        tmp_cost.color = StaticManager.CoinCheck(so.cost) ? Color.white : Color.red;

        tmp_paramNext.gameObject.SetActive(_skillTreeUnit.unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_vec.SetActive(_skillTreeUnit.unlockState == SkillTreeUnlockState.EnhanceReady);
        obj_complete.SetActive(_skillTreeUnit.unlockState == SkillTreeUnlockState.EnhanceComplete);

        this.gameObject.SetActive(true);


    }


}
