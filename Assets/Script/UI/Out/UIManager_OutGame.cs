using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Numerics;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

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
    [SerializeField] UI_ResourceCounter[] ui_resourceCounters;
    private float currentCoinFloat;

    [Space(10)]
    [Header(" -- Main --")]
    [SerializeField] GameObject main;
    [SerializeField] UI_SkillTreeMaanger ui_skillTreeMaanger;
    [SerializeField] UI_ArtifactManager ui_artifactManager;
    public UI_SkillTreeMaanger UI_SkillTreeManager => ui_skillTreeMaanger;
    public UI_ArtifactManager UI_ArtifactManager => ui_artifactManager;





    private OutGame_MenuType[] outGameMenuTypes = new OutGame_MenuType[] {
        OutGame_MenuType.SkillTree,
        OutGame_MenuType.Artifact,
    };
    private OutGame_MenuType currentMenuType;




    void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(this);
    }
    void Start()
    {
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

        // -- resource counter set --
        foreach (var ui_resourceCounter in ui_resourceCounters)
        {
            ui_resourceCounter.AwakeCall(false);
        }
        Debug.Log("UI_ResourceCounter set === OutGame");
    }

    private void ChangeGateState(GameStateType _state)
    {
        if (_state == GameStateType.OutGame)
        {
            currentMenuType = OutGame_MenuType.None;
            main.SetActive(true);
            OnSelect_HeaderButton(OutGame_MenuType.SkillTree);

            foreach (var ui_resourceCounter in ui_resourceCounters)
            {
                ui_resourceCounter.CounterUpdateCheck();
            }
        }
    }


    #region -- on Click --
    private void OnSelect_HeaderButton(OutGame_MenuType _outGameMenuType)
    {
        if (currentMenuType == _outGameMenuType) return;
        currentMenuType = _outGameMenuType;

        ui_skillTreeMaanger.Init(_outGameMenuType);
        ui_artifactManager.Init(_outGameMenuType);

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
