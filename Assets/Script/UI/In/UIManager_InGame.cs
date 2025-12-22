using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System.Numerics;
using DG.Tweening;

public class UIManager_InGame : MonoBehaviour
{
    public static UIManager_InGame Inst;
    [SerializeField] TextMeshProUGUI tmp_timer;
    [SerializeField] TextMeshProUGUI tmp_coin;
    public UI_ResultManager ui_ResultManager;
    public UI_EventManager ui_EventManager;
    private float currentCoinFloat;


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }

        GameEvent.UI.TimeLimit.Subscribe(Set_TimeLimit).AddTo(this);
        GameEvent.UI.CoinMod.Subscribe(Set_CoinMod).AddTo(this);
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

    private void Set_CoinMod(BigInteger mod)
    {
        var modCoin = StaticManager.Get_BigintegerToUnit(mod);
        DOTween.To(() => currentCoinFloat, x => currentCoinFloat = x, modCoin.num, 0.5f).OnUpdate(() =>
        {
            if (modCoin.unit == "")
            {
                tmp_coin.text = $"{currentCoinFloat.ToString("F0")} {modCoin.unit}";
            }
            else
            {
                tmp_coin.text = $"{currentCoinFloat.ToString("F2")} {modCoin.unit}";
            }
        });
        //var setText = StaticManager.Get_BigintegerToString(mod);
        //tmp_coin.text = setText;
    }
}
