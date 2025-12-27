using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_OutGame_HeaderButton : MonoBehaviour
{
    private OutGame_MenuType outGameMenuType;
    private HButton hButton;


    public Action<OutGame_MenuType> onSelect;


    public void AwakeCall(OutGame_MenuType _outGameMenuType, Action<OutGame_MenuType> _onSelect)
    {
        this.gameObject.SetActive(true);
        outGameMenuType = _outGameMenuType;
        hButton = this.GetComponent<HButton>();
        onSelect = _onSelect;
    }


    public void OnClick_HeaderButton()
    {
        onSelect?.Invoke(outGameMenuType);
    }
    public void Set_Select(OutGame_MenuType _currentType)
    {
        if (this.gameObject.activeSelf == false) return;
        hButton.Set_SelectActive(_currentType == outGameMenuType);
    }


}
