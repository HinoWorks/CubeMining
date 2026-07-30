using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

/// <summary>
/// インゲーム中のポーズ制御（時間停止・入力・カーソル・サウンド）
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Inst;

    public bool IsPaused { get; private set; }

    UI_PauseMenu pauseMenu;
    float savedTimeScale = 1f;
    bool savedCursorVisible;
    CursorLockMode savedCursorLockState;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureInstance()
    {
        if (Inst != null) return;
        if (FindFirstObjectByType<PauseManager>() != null) return;
        var go = new GameObject("===PauseManager");
        go.AddComponent<PauseManager>();
    }

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); return; }

    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;
        HandleEscape();
    }

    void OnDestroy()
    {
        if (Inst == this) Inst = null;
    }

    void EnsurePauseMenu()
    {
        if (pauseMenu != null) return;
        pauseMenu = UIManager_InGame.Inst?.UI_PauseMenu;
    }

    public void HandleEscape()
    {
        if (UI_UserSettingManager.Inst != null && UI_UserSettingManager.Inst.IsOpen)
        {
            UI_UserSettingManager.Inst.OnClick_Back();
            return;
        }

        if (TutorialManager.Inst != null && TutorialManager.Inst.IsShowing) return;

        if (!CanTogglePause()) return;

        if (IsPaused) Resume();
        else Pause();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;
        if (!CanTogglePause()) return;

        EnsurePauseMenu();
        if (pauseMenu == null) return;

        IsPaused = true;
        savedTimeScale = Time.timeScale;

        Time.timeScale = 0f;
        DOTween.timeScale = 0f;

        SaveCursorState();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SoundManager.Inst?.SetPaused(true);
        RayManager.Inst?.SetRaycastEnabled(false);
        InputManager.Inst?.SwitchToUI();

        pauseMenu.Open();
    }

    public void Resume()
    {
        if (!IsPaused) return;

        IsPaused = false;
        pauseMenu?.Close();

        Time.timeScale = savedTimeScale;
        DOTween.timeScale = savedTimeScale;

        SoundManager.Inst?.SetPaused(false);
        RayManager.Inst?.SetRaycastEnabled(true);
        InputManager.Inst?.SwitchToPlayer();
        RestoreCursorState();
    }

    /// <summary>ゲーム終了など、ポーズ状態を強制解除する</summary>
    public void ForceResumeIfPaused()
    {
        if (!IsPaused) return;
        IsPaused = false;
        pauseMenu?.Close();

        Time.timeScale = savedTimeScale > 0f ? savedTimeScale : 1f;
        DOTween.timeScale = Time.timeScale;

        SoundManager.Inst?.SetPaused(false);
        RayManager.Inst?.SetRaycastEnabled(true);
        InputManager.Inst?.SwitchToPlayer();
        RestoreCursorState();
    }

    bool CanTogglePause()
    {
        if (GameWatcher.Inst == null || !GameWatcher.Inst.isInGameNow) return false;

        var resultManager = UIManager_InGame.Inst?.ui_ResultManager;
        if (resultManager != null && resultManager.gameObject.activeInHierarchy) return false;

        return true;
    }

    void SaveCursorState()
    {
        savedCursorVisible = Cursor.visible;
        savedCursorLockState = Cursor.lockState;
    }

    void RestoreCursorState()
    {
        Cursor.visible = savedCursorVisible;
        Cursor.lockState = savedCursorLockState;
    }
}
