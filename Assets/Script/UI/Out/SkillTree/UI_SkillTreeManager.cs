using UnityEngine;

public class UI_SkillTreeMaanger : MonoBehaviour
{
    [SerializeField] UI_SkillTreeUnit[] skillTreeUnits;
    [SerializeField] UI_SkillTreeDetail ui_skillTreeDetail;



    void Awake()
    {
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            skillTreeUnit.AwakeCall(OnMouseOver);
        }
        ui_skillTreeDetail.gameObject.SetActive(false);
    }

    private void OnMouseOver(bool _isEnter, UI_SkillTreeUnit _skillTreeUnit)
    {
        ui_skillTreeDetail.SetData(_isEnter);
    }
}
