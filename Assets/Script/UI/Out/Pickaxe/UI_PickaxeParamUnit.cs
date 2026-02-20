using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_PickaxeParamUnit : MonoBehaviour
{
    [SerializeField] Image image_icon;
    [SerializeField] TextMeshProUGUI tmp_title;
    [SerializeField] TextMeshProUGUI tmp_param;

    public void SetData(Sprite _icon, string _title, string _param)
    {
        image_icon.sprite = _icon;
        tmp_title.text = _title;
        tmp_param.text = _param;
    }

}
