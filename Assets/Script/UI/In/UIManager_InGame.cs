using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UIManager_InGame : MonoBehaviour
{

    public static UIManager_InGame Inst;

    [SerializeField] TextMeshProUGUI tmp_timer;
    public UI_ResultManager ui_ResultManager;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }

        GameEvent.UI.TimeLimit.Subscribe(Set_TimeLimit).AddTo(this);
    }

    private void Set_TimeLimit(float time)
    {
        tmp_timer.text = time.ToString("F2");
    }


}
