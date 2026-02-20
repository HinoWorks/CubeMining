using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

public class UI_PickaxeEquipCont : UI_PickaxeParamCont
{
    [Space(10)]
    [SerializeField] GameObject obj_notEquip;


    public override void SetData(PickaxeUnitData _so)
    {
        if (_so == null)
        {
            obj_notEquip.SetActive(true);
            obj_main.SetActive(false);
            return;
        }
        base.SetData(_so);
    }



    #region -- マウスアクション --

    #endregion
}
