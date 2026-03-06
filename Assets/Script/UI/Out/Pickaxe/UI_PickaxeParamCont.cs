using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
        Set_Param();
    }



    private void Set_Param()
    {
        for (int i = 0; i < Enum.GetValues(typeof(PickaxeParamType)).Length; i++)
        {
            if (i >= ui_paramUnits.Length) break;
            var paramType = (PickaxeParamType)i;
            var setText = "";
            switch (paramType)
            {
                case PickaxeParamType.Damage:
                    setText = $"{so.damage}";
                    break;
                case PickaxeParamType.AttackInterval:
                    setText = $"{so.attackInterval} <size=75%>sec</size>";
                    break;
                case PickaxeParamType.CriticalRate:
                    setText = $"{(so.criticalRate * 100).ToString("F1")} <size=75%>%</size>";
                    break;
                case PickaxeParamType.ResourceRate:
                    setText = $"+{(so.resourceUpRate * 100).ToString("F1")} <size=75%>%</size>";
                    break;
            }
            var paramBase = SOLoader.AttackUnitData.GetPickaxeParamBase(paramType);
            ui_paramUnits[i].SetData(paramBase.icon, paramBase.paramName, setText);
        }
    }

}
