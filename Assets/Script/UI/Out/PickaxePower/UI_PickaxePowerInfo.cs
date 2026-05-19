using UnityEngine;
using UnityEngine.UI;

public class UI_PickaxePowerInfo : MonoBehaviour
{
    [SerializeField] Image image_icon;




    public void SetData(UI_PickaxePowerUnit _ui_pickaxePowerUnit)
    {
        this.gameObject.SetActive(_ui_pickaxePowerUnit != null);
        if (_ui_pickaxePowerUnit == null) return;

        Debug.Log($"SetData: {_ui_pickaxePowerUnit.so_base.index}");
        // データ設定   
        image_icon.sprite = _ui_pickaxePowerUnit.so_base.icon;
    }
}
