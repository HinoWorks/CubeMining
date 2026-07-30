using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// チュートリアル表示の判定・キュー・セーブを管理するシングルトン。
/// 表示自体は UI_TutorialManager に委譲する。
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Inst;

    public bool IsShowing { get; private set; }

    UI_TutorialManager tutorialUI;
    readonly Queue<PendingTutorial> pendingQueue = new();
    int currentIndex = -1;
    bool skipSaveOnClose;
    UniTaskCompletionSource currentCloseTcs;

    struct PendingTutorial
    {
        public int index;
        public bool skipSave;
        public UniTaskCompletionSource tcs;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureInstance()
    {
        if (Inst != null) return;
        if (FindFirstObjectByType<TutorialManager>() != null) return;
        var go = new GameObject("===TutorialManager");
        go.AddComponent<TutorialManager>();
    }

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); return; }
    }

    void OnDestroy()
    {
        if (Inst == this) Inst = null;
        currentCloseTcs?.TrySetCanceled();
        while (pendingQueue.Count > 0)
        {
            pendingQueue.Dequeue().tcs?.TrySetCanceled();
        }
    }

    /// <summary>
    /// 未表示ならチュートリアルを出し、閉じるまで待機する。
    /// 既に表示済みなら即座に完了する。
    /// </summary>
    public UniTask Check_Tutorial(TutorialType _tutorialType)
    {
        var _index = SOLoader.TutorialData?.Get_TutorialIndex(_tutorialType) ?? -1;
        if (SaveLoader.Inst != null && SaveLoader.Inst.IsTutorialShown(_index))
        {
            return UniTask.CompletedTask;
        }
        if (pendingQueueContains(_index) || currentIndex == _index)
        {
            return UniTask.CompletedTask;
        }

        var tcs = new UniTaskCompletionSource();

        if (IsShowing)
        {
            pendingQueue.Enqueue(new PendingTutorial
            {
                index = _index,
                skipSave = false,
                tcs = tcs,
            });
            return tcs.Task;
        }

        ShowInternal(_index, false, tcs);
        return tcs.Task;
    }

    /// <summary>
    /// デバッグ用。既読状態を無視して強制表示し、閉じてもセーブしない。
    /// 閉じるまで待機する。
    /// </summary>
    public UniTask Debug_ShowTutorial(int _index)
    {
        var tcs = new UniTaskCompletionSource();
        ShowInternal(_index, true, tcs);
        return tcs.Task;
    }

    /// <summary>UI側の閉じ完了コールバック</summary>
    public void Notify_TutorialClosed()
    {
        if (!IsShowing) return;

        var closedIndex = currentIndex;
        var shouldSave = !skipSaveOnClose;
        var closedTcs = currentCloseTcs;

        IsShowing = false;
        currentIndex = -1;
        skipSaveOnClose = false;
        currentCloseTcs = null;

        if (shouldSave && closedIndex >= 0)
        {
            SaveLoader.Inst?.Request_SaveTutorialShown(closedIndex);
        }

        if (pendingQueue.Count > 0)
        {
            var next = pendingQueue.Dequeue();
            ShowInternal(next.index, next.skipSave, next.tcs);
            closedTcs?.TrySetResult();
            return;
        }

        closedTcs?.TrySetResult();
    }

    void ShowInternal(int _index, bool _skipSave, UniTaskCompletionSource _tcs)
    {
        EnsureTutorialUI();
        if (tutorialUI == null)
        {
            Debug.LogWarning($"[TutorialManager] UI_TutorialManager が見つかりません。index={_index}");
            _tcs?.TrySetResult();
            return;
        }

        IsShowing = true;
        currentIndex = _index;
        skipSaveOnClose = _skipSave;
        currentCloseTcs = _tcs;
        tutorialUI.Open(_index);
    }

    bool pendingQueueContains(int _index)
    {
        foreach (var pending in pendingQueue)
        {
            if (pending.index == _index) return true;
        }
        return false;
    }

    void EnsureTutorialUI()
    {
        if (tutorialUI != null) return;
        tutorialUI = FindFirstObjectByType<UI_TutorialManager>(FindObjectsInactive.Include);
    }

    public void RegisterUI(UI_TutorialManager _ui)
    {
        tutorialUI = _ui;
    }
}
