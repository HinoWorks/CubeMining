using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System.Numerics;
using DG.Tweening;

public class UIManager_InGame : MonoBehaviour
{
    public static UIManager_InGame Inst;
    [SerializeField] UI_ResourceCounter[] ui_resourceCounters;
    [SerializeField] TextMeshProUGUI tmp_timer;
    [SerializeField] TextMeshProUGUI tmp_depthCount;
    public UI_ResultManager ui_ResultManager;
    public UI_EventManager ui_EventManager;
    //private float currentCoinFloat;


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }

        GameEvent.UI.TimeLimit.Subscribe(Set_TimeLimit).AddTo(this);
        //GameEvent.UI.CoinMod.Subscribe(Set_CoinMod).AddTo(this);
        GameEvent.UI.DepthCount.Subscribe(Set_DepthCount).AddTo(this);
        GameEvent.GameState.SetGameState.Subscribe(ChangeGateState).AddTo(this);

        foreach (var ui_resourceCounter in ui_resourceCounters)
        {
            ui_resourceCounter.AwakeCall(true);
        }
    }

    private void ChangeGateState(GameStateType _state)
    {
        switch (_state)
        {
            case GameStateType.InGame_Ready:
                ui_EventManager.StateGame();
                foreach (var ui_resourceCounter in ui_resourceCounters)
                {
                    ui_resourceCounter.Set_Init();
                }
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

    private void Set_DepthCount(int depth)
    {
        tmp_depthCount.text = depth.ToString();
    }


    /// <summary>
    /// リソースを飛ばす際のターゲット位置を取得
    /// </summary>
    public Transform Get_ResourceCounterTargetPosition(ResourceType _resourceType)
    {
        foreach (var ui_resourceCounter in ui_resourceCounters)
        {
            if (ui_resourceCounter.ResourceType == _resourceType)
                return ui_resourceCounter.targetPosition;
        }
        return null;
    }


    /*
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
    */
}
