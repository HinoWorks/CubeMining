using UnityEngine;
using UniRx;


public class UIManager_OutGame : MonoBehaviour
{
    public static UIManager_OutGame Inst;

    [SerializeField] private GameObject main;

    void Awake()
    {
        GameEvent.GameState.SetGameState.Subscribe(ChangeGateState).AddTo(this);
    }

    private void ChangeGateState(GameStateType _state)
    {
        if (_state == GameStateType.OutGame)
        {
            main.SetActive(true);
        }
    }


    #region -- on Click --
    public void OnClick_StartInGame()
    {
        main.SetActive(false);
        GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
    }
    #endregion
}
