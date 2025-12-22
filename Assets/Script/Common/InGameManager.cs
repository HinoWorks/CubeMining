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
                BlockGenerateManager.Inst.InitialGenerate();
                break;
            case GameStateType.InGame:
                timer = 0;
                AttackManager.Inst.Set_AttackState(true);
                BlockGenerateManager.Inst.Set_GenerateState(true);
                break;
            case GameStateType.InGame_End:
                AttackManager.Inst.Set_AttackState(false);
                BlockGenerateManager.Inst.Set_GenerateState(false);
                break;
            case GameStateType.Result:
                AttackManager.Inst.AttackUnitDelete();
                break;
            case GameStateType.OutGame:
                BlockGenerateManager.Inst.ResetAllBlocks();
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
            GameEvent.UI.PublishTimeLimit(0f);
            GameWatcher.Inst.SetGameState(GameStateType.InGame_End);
        }
    }



}
