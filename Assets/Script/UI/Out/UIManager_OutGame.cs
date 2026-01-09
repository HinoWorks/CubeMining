using UnityEngine;
using UniRx;
using UnityEngine.UI;

public enum OutGame_MenuType
{
    None,
    SkillTree,
    Artifact,
}

public class UIManager_OutGame : MonoBehaviour
{
    public static UIManager_OutGame Inst;

    [SerializeField] UI_OutGame_HeaderButton[] headerButtons;
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
