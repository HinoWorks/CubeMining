using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_OutGame_HeaderButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_title;
    [SerializeField] GameObject obj_checkMark;
    public OutGame_MenuType outGameMenuType { get; private set; }
    private HButton hButton;


    public Action<OutGame_MenuType> onSelect;


    public void AwakeCall(OutGame_MenuType _outGameMenuType, Action<OutGame_MenuType> _onSelect)
    {
        this.gameObject.SetActive(true);
        outGameMenuType = _outGameMenuType;
        tmp_title.text = outGameMenuType.ToString();
        hButton = this.GetComponent<HButton>();
        onSelect = _onSelect;

        obj_checkMark.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// アクティブ状態とチェックマークの状態を設定
    /// </summary>
    public void Set_Activate(bool _isCheckMarkActive = false)
    {
        this.gameObject.SetActive(true);
        obj_checkMark.SetActive(_isCheckMarkActive);
    }

    /// <summary>
    /// チェックマークの状態を設定
    /// </summary>
    public void Set_CheckMarkActive(bool _active)
    {
        obj_checkMark.SetActive(_active);
    }

    public void OnClick_HeaderButton()
    {
        obj_checkMark.SetActive(false);
        onSelect?.Invoke(outGameMenuType);
    }
    public void Set_Select(OutGame_MenuType _currentType)
    {
        if (this.gameObject.activeSelf == false) return;
        hButton.Set_SelectActive(_currentType == outGameMenuType);
    }


}
