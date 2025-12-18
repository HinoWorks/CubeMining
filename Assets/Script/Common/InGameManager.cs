using UnityEngine;
using UniRx;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Inst;


    private float timer = 0;
    private float timeLimit = 5;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
    }



    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.InGame_Ready:
                AttackManager.Inst.Set_Ready();
                break;
            case GameStateType.InGame:
                timer = 0;
                AttackManager.Inst.Set_AttackState(true);
                break;
            case GameStateType.InGame_End:
                AttackManager.Inst.Set_AttackState(false);
                break;
            case GameStateType.Result:
                break;
            case GameStateType.OutGame:
                break;
        }
    }

    void Update()
    {
        if (!GameWatcher.Inst.isInGameNow) return;
        timer += Time.deltaTime;
        GameEvent.UI.PublishTimeLimit(timeLimit - timer);
        if (timer >= timeLimit)
        {
            GameWatcher.Inst.SetGameState(GameStateType.InGame_End);
        }
    }



}
