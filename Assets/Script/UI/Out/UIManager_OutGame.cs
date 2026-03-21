using UnityEngine;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

public enum OutGame_MenuType
{
    None,
    SkillTree,
    Artifact,
    Pickaxe,
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
    [SerializeField] UI_PickaxeManager ui_pickaxeManager;
    public UI_SkillTreeMaanger UI_SkillTreeManager => ui_skillTreeMaanger;
    public UI_ArtifactManager UI_ArtifactManager => ui_artifactManager;
    public UI_PickaxeManager UI_PickaxeManager => ui_pickaxeManager;





    private OutGame_MenuType[] outGameMenuTypes = new OutGame_MenuType[] {
        OutGame_MenuType.SkillTree,
        OutGame_MenuType.Artifact,
        OutGame_MenuType.Pickaxe,
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

        ui_skillTreeMaanger.Start_OnceInit();
        ui_artifactManager.Start_OnceInit();
        ui_pickaxeManager.Start_OnceInit();
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
        ui_pickaxeManager.Init(_outGameMenuType);

        foreach (var headerButton in headerButtons)
        {
            headerButton.Set_Select(currentMenuType);
        }
    }
    public async void OnClick_StartInGame()
    {
        UIManager_Title.Inst.Set_OverScreen().Forget();
        await UniTask.Delay(300);
        main.SetActive(false);
        GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
    }
    #endregion
}
