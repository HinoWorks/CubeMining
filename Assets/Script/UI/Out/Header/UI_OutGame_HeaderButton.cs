using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_OutGame_HeaderButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_title;
    [SerializeField] GameObject obj_checkMark_once;
    [SerializeField] GameObject obj_checkMark_EnhanceReady;

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

        obj_checkMark_once.SetActive(false);
        obj_checkMark_EnhanceReady.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// アクティブ状態とチェックマークの状態を設定
    /// </summary>
    public void Set_ButtonUnlock(bool _isFirstUnlock)
    {
        this.gameObject.SetActive(true);
        obj_checkMark_once.SetActive(_isFirstUnlock);
    }

    /// <summary>
    /// チェックマークの状態を設定
    /// </summary>
    public void Set_CheckMarkActive(bool _active)
    {
        obj_checkMark_EnhanceReady.SetActive(_active);
    }

    /// <summary>
    /// チェックマークの状態を設定 == 一度見ると消えるチェックマーク
    /// </summary>
    public void Set_CheckMarkActive_Once(bool _active)
    {
        obj_checkMark_once.SetActive(_active);
    }

    public void OnClick_HeaderButton()
    {
        obj_checkMark_once.SetActive(false);
        onSelect?.Invoke(outGameMenuType);
    }
    public void Set_Select(OutGame_MenuType _currentType)
    {
        if (this.gameObject.activeSelf == false) return;
        hButton.Set_SelectActive(_currentType == outGameMenuType);
    }


}
