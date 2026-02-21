using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PickaxeSelectInfoCont : UI_PickaxeParamCont
{

    [SerializeField] TextMeshProUGUI tmp_pickaxeName;

    public override void SetData(PickaxeUnitData _so)
    {
        base.SetData(_so);
        tmp_pickaxeName.SetText(_so.pickaxeName);
    }


}
