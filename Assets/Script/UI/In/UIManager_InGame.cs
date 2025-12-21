using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UIManager_InGame : MonoBehaviour
{

    public static UIManager_InGame Inst;

    [SerializeField] TextMeshProUGUI tmp_timer;
    public UI_ResultManager ui_ResultManager;
    public UI_EventManager ui_EventManager;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }

        GameEvent.UI.TimeLimit.Subscribe(Set_TimeLimit).AddTo(this);
        GameEvent.GameState.SetGameState.Subscribe(ChangeGateState).AddTo(this);
    }

    private void ChangeGateState(GameStateType _state)
    {
        switch (_state)
        {
            case GameStateType.InGame_Ready:
                ui_EventManager.StateGame();
                break;
            case GameStateType.InGame_End:
                ui_EventManager.EndGame();
                break;
            case GameStateType.Result:
                ui_ResultManager.Open();
                break;
        }
    }

    private void Set_TimeLimit(float time)
    {
        tmp_timer.text = time.ToString("F2");
    }


}
