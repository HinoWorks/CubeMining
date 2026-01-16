using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Numerics;
using DG.Tweening;
using TMPro;

public enum OutGame_MenuType
{
    None,
    SkillTree,
    Artifact,
}

public class UIManager_OutGame : MonoBehaviour
{
    public static UIManager_OutGame Inst;

    [Header(" -- Header --")]
    [SerializeField] UI_OutGame_HeaderButton[] headerButtons;
    [SerializeField] TextMeshProUGUI tmp_coin;
    private float currentCoinFloat;

    [Space(10)]
    [Header(" -- Main --")]
    [SerializeField] GameObject main;
    [SerializeField] UI_SkillTreeMaanger ui_skillTreeMaanger;
    public UI_SkillTreeMaanger UI_SkillTreeManager => ui_skillTreeMaanger;





    private OutGame_MenuType[] outGameMenuTypes = new OutGame_MenuType[] {
        OutGame_MenuType.SkillTree,
        OutGame_MenuType.Artifact,
    };
    private OutGame_MenuType currentMenuType;


    void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(this);

        GameEvent.UI.CoinMod.Subscribe(Set_CoinMod).AddTo(this);
        GameEvent.GameState.SetGameState.Subscribe(ChangeGateState).AddTo(this);

        // -- header button set --
        var counter = 0;
        foreach (var headerButton in headerButtons)
        {
            if (counter >= outGameMenuTypes.Length)
            {
                headerButton.gameObject.SetActive(false);
                continue;
            }
            headerButton.AwakeCall(outGameMenuTypes[counter], OnSelect_HeaderButton);
            counter++;
        }
    }



    private void ChangeGateState(GameStateType _state)
    {
        if (_state == GameStateType.OutGame)
        {
            currentMenuType = OutGame_MenuType.None;
            main.SetActive(true);
            OnSelect_HeaderButton(OutGame_MenuType.SkillTree);
        }
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
    }


    #region -- on Click --
    private void OnSelect_HeaderButton(OutGame_MenuType _outGameMenuType)
    {
        if (currentMenuType == _outGameMenuType) return;
        currentMenuType = _outGameMenuType;

        ui_skillTreeMaanger.Init(_outGameMenuType);


        foreach (var headerButton in headerButtons)
        {
            headerButton.Set_Select(currentMenuType);
        }
    }
    public void OnClick_StartInGame()
    {
        main.SetActive(false);
        GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
    }
    #endregion
}
