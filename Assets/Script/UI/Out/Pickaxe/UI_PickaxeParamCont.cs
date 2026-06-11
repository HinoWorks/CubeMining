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
        //obj_main.SetActive(_so != null);
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
            var setText_title = "";
            switch (paramType)
            {
                case PickaxeParamType.Damage:
                    setText = $"{so.damage}";
                    setText_title = "Mine Power";
                    break;
                case PickaxeParamType.AttackInterval:
                    setText = $"{so.attackInterval} <size=75%>sec</size>";
                    setText_title = "Mine Time";
                    break;
                case PickaxeParamType.CriticalRate:
                    setText = $"{(so.criticalRate * 100).ToString("F0")} <size=75%>%</size>";
                    setText_title = "2X Power Chance";
                    break;
                case PickaxeParamType.ResourceRate:
                    setText = $"+{(so.resourceUpRate * 100).ToString("F0")} <size=75%>%</size>";
                    setText_title = "Resource Up";
                    break;
                case PickaxeParamType.AreaSize:
                    setText = $"{(so.size * 100).ToString("F0")} <size=75%>%</size>";
                    setText_title = "Mining Area Size";
                    break;
            }
            var paramBase = SOLoader.AttackUnitData.GetPickaxeParamBase(paramType);
            ui_paramUnits[i].SetData(paramBase.icon, setText_title, setText);
        }
    }

}
