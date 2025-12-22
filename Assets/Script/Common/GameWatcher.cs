using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


public class GameWatcher : MonoBehaviour
{

    public static GameWatcher Inst;
    public GameStateType currentGameState { get; private set; } = GameStateType.Title;
    public bool isInGameNow => currentGameState == GameStateType.InGame;
    private void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }




    void Start()
    {
        Set_InGameStart();
    }

    private async void Set_InGameStart()
    {
        await UniTask.Delay(3000);
        SetGameState(GameStateType.InGame_Ready);
    }


    public void SetGameState(GameStateType state)
    {
        currentGameState = state;
        Debug.Log($"<color=yellow> ========= GameState: {currentGameState} ========= </color>");
        GameEvent.GameState.PublishGameState(currentGameState);
    }
}
