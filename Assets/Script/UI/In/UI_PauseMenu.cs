using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// インゲームポーズメニュー
/// </summary>
public class UI_PauseMenu : MonoBehaviour
{


    public void Open()
    {
        Debug.Log("Open");
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }




    public void OnClick_Resume()
    {
        PauseManager.Inst?.Resume();
        this.Close();
    }
    public void OnClick_End()
    {
        InGameManager.Inst?.SessionEnd();
        this.Close();
    }


}
