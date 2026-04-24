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
        obj_notEquip.SetActive(so == null);
        obj_main.SetActive(so != null);
        if (so == null) return;
        base.SetData(_so);
    }


    #region -- マウスアクション --

    #endregion
}
