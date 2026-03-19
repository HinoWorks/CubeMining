using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class UIManager_Title : MonoBehaviour
{
    public static UIManager_Title Inst;

    [SerializeField] GameObject obj_main;
    [SerializeField] HButton[] headerButtons;
    [SerializeField] private SimpleAnimation simpleAnimation;
    private string animName_ScreenON = "ScreenON";
    private string animName_ScreenOFF = "ScreenOFF";
    private float waitTime_overScreen = 0.4f;

    private CancellationTokenSource overScreenCTS;

    void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(this);
        GameEvent.GameState.SetGameState.Subscribe(ChangeGateState).AddTo(this);
        obj_main.SetActive(true);
        simpleAnimation.gameObject.SetActive(false);
    }



    private void ChangeGateState(GameStateType _state)
    {
        if (_state == GameStateType.Title)
        {
            foreach (var headerButton in headerButtons)
            {
                headerButton.Set_SelectActive(false);
            }
            obj_main.SetActive(true);
        }
        else
        {
            obj_main.SetActive(false);
        }
    }


    #region -- on Click --
    public void OnClick_StartInGame()
    {
        GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
    }
    public void OnClick_StartOutGame()
    {
        GameWatcher.Inst.SetGameState(GameStateType.OutGame);
    }
    public void OnClick_Option()
    {
        //GameWatcher.Inst.SetGameState(GameStateType.InGame_Ready);
    }
    public void OnClick_Exit()
    {
        Application.Quit();
    }
    #endregion

    void OnDestroy()
    {
        overScreenCTS?.Cancel();
        overScreenCTS?.Dispose();
        overScreenCTS = null;
    }



    #region -- Animation --
    /// <summary>
    /// 画面オーバー/アンダー切り替え用。
    /// boolのみで完結するように state 名と CancellationToken は内部で決定する。
    /// </summary>
    public async UniTask Set_OverScreen()
    {
        if (simpleAnimation == null)
        {
            Debug.LogWarning($"{nameof(UIManager_Title)}: SimpleAnimation is null.");
            return;
        }
        simpleAnimation.gameObject.SetActive(true);
        // 連打などで同時に走らないよう、前回用の待機をキャンセルする。
        overScreenCTS?.Cancel();
        overScreenCTS?.Dispose();
        overScreenCTS = new CancellationTokenSource();

        var cancellationToken = overScreenCTS.Token;

        // 念のため stateName が空の場合は何もしない（インスペクタ未設定対策）。
        if (string.IsNullOrEmpty(animName_ScreenON) || string.IsNullOrEmpty(animName_ScreenOFF)) return;
        simpleAnimation.Play(animName_ScreenON);
        await UniTask.Delay((int)(waitTime_overScreen * 1000), cancellationToken: cancellationToken);
        simpleAnimation.Play(animName_ScreenOFF);
        await UniTask.Delay(200, cancellationToken: cancellationToken);

        simpleAnimation.gameObject.SetActive(false);
    }
    #endregion

    [ContextMenu("DEBUG_Set_OverScreen")]
    public void DEBUG_Set_OverScreen()
    {
        Set_OverScreen().Forget();
    }
}
