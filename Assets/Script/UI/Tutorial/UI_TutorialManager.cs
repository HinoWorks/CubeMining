using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// チュートリアル表示専用 UI。
/// 内容は SO_TutorialData から取得する。
/// ロジック（既読判定・セーブ・キュー）は TutorialManager が担当する。
/// </summary>
public class UI_TutorialManager : UI_PopUpBase
{
    public static UI_TutorialManager Inst;

    [SerializeField] TextMeshProUGUI tmp_title;
    [SerializeField] TextMeshProUGUI tmp_body;
    [SerializeField] Image img_icon;

    int currentIndex = -1;

    void OnDestroy()
    {
        if (Inst == this) Inst = null;
    }

    public void Open(int _index)
    {
        currentIndex = _index;
        ApplyContent(_index);
        base.Open();
    }

    public override void Close()
    {
        currentIndex = -1;
        TutorialManager.Inst?.Notify_TutorialClosed();

        base.Close();
    }

    /// <summary>閉じるボタンから呼ぶ</summary>
    public void OnClick_Close()
    {
        Close();
    }

    void ApplyContent(int _tutorialIndex)
    {
        var data = SOLoader.TutorialData?.Get_TutorialUnitData(_tutorialIndex);
        if (data == null) return;

        if (tmp_title != null) tmp_title.text = data.title;
        if (tmp_body != null) tmp_body.text = data.description;
        if (img_icon != null)
        {
            img_icon.sprite = data.icon;
            img_icon.enabled = data.icon != null;
        }
    }
}
