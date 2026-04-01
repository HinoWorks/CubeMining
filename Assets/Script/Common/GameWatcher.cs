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
        currentGameState = GameStateType.Title;
        GameEvent.GameState.PublishGameState(currentGameState);
        GameParamManager.Init();
    }


    public void SetGameState(GameStateType state)
    {
        if (!GameParamManager.isInitEnd) return;
        currentGameState = state;
        Debug.Log($"<color=yellow> ========= GameState: {currentGameState} ========= </color>");
        GameEvent.GameState.PublishGameState(currentGameState);
    }
}
