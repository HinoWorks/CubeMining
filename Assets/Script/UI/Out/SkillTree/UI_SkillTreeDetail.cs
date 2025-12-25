using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillTreeDetail : MonoBehaviour
{
    [SerializeField] Image image_icon;
    [SerializeField] TextMeshProUGUI tmp_name;
    [SerializeField] TextMeshProUGUI tmp_description;
    [SerializeField] TextMeshProUGUI tmp_enhanceLevel;
    [SerializeField] TextMeshProUGUI tmp_enhanceCost;
    [SerializeField] TextMeshProUGUI tmp_enhanceButton;



    public void SetData(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);
    }

}
