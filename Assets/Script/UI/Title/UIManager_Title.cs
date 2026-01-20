using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class UIManager_Title : MonoBehaviour
{
    public static UIManager_Title Inst;

    [SerializeField] GameObject obj_main;
    [SerializeField] HButton[] headerButtons;

    void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(this);
        GameEvent.GameState.SetGameState.Subscribe(ChangeGateState).AddTo(this);
        obj_main.SetActive(true);
    }



    private void ChangeGateState(GameStateType _state)
    {
        if (_state == GameStateType.Title)
        {
            foreach (var headerButton in headerButtons)
            {
                headerButton.Set_SelectActive(false);
            }
            obj_main.SetActive(true);
        }
        else
        {
            obj_main.SetActive(false);
        }
    }


    #region -- on Click --
    public void OnClick_StartInGame()
    {
        GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
    }
    public void OnClick_StartOutGame()
    {
        GameWatcher.Inst.SetGameState(GameStateType.OutGame);
    }
    public void OnClick_Option()
    {
        //GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
    }
    public void OnClick_Exit()
    {
        Application.Quit();
    }
    #endregion
}
