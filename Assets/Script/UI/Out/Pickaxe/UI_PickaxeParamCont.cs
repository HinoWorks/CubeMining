using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PickaxeParamCont : MonoBehaviour
{
    [SerializeField] protected GameObject obj_main;
    [SerializeField] Image icon;
    [SerializeField] UI_PickaxeParamUnit[] ui_paramUnits;
    public PickaxeUnitData so { get; private set; }


    public virtual void SetData(PickaxeUnitData _so)
    {
        obj_main.SetActive(_so != null);
        so = _so;
        icon.sprite = so.icon;
        foreach (var ui_paramUnit in ui_paramUnits)
        {
            ui_paramUnit.SetData(null, null, null);
        }
    }

}
