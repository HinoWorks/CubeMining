using UnityEngine;
using Cysharp.Threading.Tasks;
#if STEAMWORKS_NET
using Steamworks;
#endif

/// <summary>
/// Steam API の初期化・コールバック処理を担当する。
/// シーン未配置でも自動起動する。シーンに置いた場合は Inspector の設定が優先される。
/// </summary>
[DefaultExecutionOrder(-200)]
public class SteamManager : MonoBehaviour
{
    public static SteamManager Inst { get; private set; }

    [SerializeField] private uint steamAppId = 480;
    [SerializeField] private bool restartAppIfNecessary = true;

    public bool IsSteamAvailable { get; private set; }
    public ISteamAchievementService Achievements { get; private set; }
    public string LastInitFailureReason { get; private set; }

#if STEAMWORKS_NET
    private bool isSteamInitialized;
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Inst != null) return;
        var go = new GameObject(nameof(SteamManager));
        go.AddComponent<SteamManager>();
    }

    void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[Steam] SteamManager initializing...");

#if UNITY_EDITOR && STEAMWORKS_NET
        InitializeSteamDeferredAsync().Forget();
#else
        CompleteInitialization();
#endif
    }

    private void CompleteInitialization()
    {
        try
        {
            Achievements = CreateAchievementService();
            IsSteamAvailable = Achievements.IsAvailable;
        }
        catch (System.Exception ex)
        {
            LastInitFailureReason = $"初期化中に例外: {ex.GetType().Name}: {ex.Message}";
            Achievements = new NullSteamAchievementService();
            IsSteamAvailable = false;
            Debug.LogError($"[Steam] {LastInitFailureReason}\n{ex}");
        }

        LogInitResult();
    }

#if UNITY_EDITOR && STEAMWORKS_NET
    private async UniTaskVoid InitializeSteamDeferredAsync()
    {
        const int maxAttempts = 10;
        const int retryDelayMs = 1000;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            CompleteInitialization();
            if (IsSteamAvailable) return;

            if (attempt < maxAttempts)
            {
                Debug.Log($"[Steam] Init attempt {attempt}/{maxAttempts} failed. Retrying in {retryDelayMs}ms...");
                await UniTask.Delay(retryDelayMs);
            }
        }
    }
#endif

    private void LogInitResult()
    {
        if (IsSteamAvailable)
        {
            Debug.Log($"[Steam] Ready. AppId={steamAppId}");
        }
        else
        {
            var reason = string.IsNullOrEmpty(LastInitFailureReason)
                ? "unknown"
                : LastInitFailureReason;
            Debug.Log($"[Steam] Not available. Reason: {reason}");
        }
    }

    void Update()
    {
#if STEAMWORKS_NET
        if (!isSteamInitialized) return;
        SteamAPI.RunCallbacks();
#endif
    }

    void OnDestroy()
    {
        if (Inst != this) return;

#if STEAMWORKS_NET
        if (isSteamInitialized)
        {
            SteamAPI.Shutdown();
            isSteamInitialized = false;
        }
#endif

        Inst = null;
    }

    void OnApplicationQuit()
    {
#if STEAMWORKS_NET
        if (!isSteamInitialized) return;
        SteamAPI.Shutdown();
        isSteamInitialized = false;
#endif
    }

    private ISteamAchievementService CreateAchievementService()
    {
#if STEAMWORKS_NET
        Debug.Log("[Steam] STEAMWORKS_NET is defined. Attempting SteamAPI init...");
        if (!InitializeSteam())
        {
            return new NullSteamAchievementService();
        }

        return new SteamAchievementService(true);
#else
        LastInitFailureReason =
            "STEAMWORKS_NET が未定義です。Build Target を Standalone (PC/Mac) にし、Steamworks.NET を導入してください。";
        Debug.Log($"[Steam] {LastInitFailureReason}");
        return new NullSteamAchievementService();
#endif
    }

#if STEAMWORKS_NET
    private bool InitializeSteam()
    {
        try
        {
            SteamAppIdFileHelper.EnsureFiles(steamAppId);

#if UNITY_EDITOR
            // Editor 実行時は Steam 経由再起動を行わない（Unity Play からは通常 false だが念のため）。
            restartAppIfNecessary = false;
#endif

            if (restartAppIfNecessary)
            {
                var appId = new AppId_t(steamAppId);
                if (SteamAPI.RestartAppIfNecessary(appId))
                {
                    LastInitFailureReason = "RestartAppIfNecessary が true を返しました。Steam 経由で起動が必要です。";
                    Debug.Log("[Steam] RestartAppIfNecessary returned true. Quitting to relaunch via Steam.");
                    Application.Quit();
                    return false;
                }
            }

            if (!Packsize.Test())
            {
                LastInitFailureReason = "Packsize Test 失敗。Steamworks.NET のネイティブ DLL 構成を確認してください。";
                Debug.Log($"[Steam] {LastInitFailureReason}");
                return false;
            }

            if (!DllCheck.Test())
            {
                LastInitFailureReason = "DllCheck Test 失敗。steam_api のネイティブライブラリを確認してください。";
                Debug.Log($"[Steam] {LastInitFailureReason}");
                return false;
            }

            if (isSteamInitialized)
            {
                SteamAPI.Shutdown();
                isSteamInitialized = false;
            }

            var initResult = SteamAPI.InitEx(out var steamErrMsg);
            if (initResult != ESteamAPIInitResult.k_ESteamAPIInitResult_OK)
            {
                LastInitFailureReason = DescribeInitFailure(initResult, steamErrMsg);
                Debug.Log($"[Steam] {LastInitFailureReason}");
                return false;
            }

            isSteamInitialized = true;
            var userName = SteamFriends.GetPersonaName();
            Debug.Log($"[Steam] Initialized. User: {userName} (AppId: {steamAppId})");
            return true;
        }
        catch (System.Exception ex)
        {
            LastInitFailureReason = $"SteamAPI 例外: {ex.GetType().Name}: {ex.Message}";
            Debug.LogError($"[Steam] {LastInitFailureReason}\n{ex}");
            return false;
        }
    }

    private static string DescribeInitFailure(ESteamAPIInitResult result, string steamErrMsg)
    {
        var hint = result switch
        {
            ESteamAPIInitResult.k_ESteamAPIInitResult_NoSteamClient =>
                "Steam クライアントが起動していないか、オンラインになっていません。Steam を再起動してログイン完了後に再試行してください。",
            ESteamAPIInitResult.k_ESteamAPIInitResult_VersionMismatch =>
                "Steam クライアントが古い可能性があります。Steam を最新に更新してください。",
            _ =>
                "Steam クライアント起動、ログイン、steam_appid.txt（プロジェクトルート）を確認してください。",
        };

        if (string.IsNullOrEmpty(steamErrMsg))
        {
            return $"SteamAPI.InitEx() 失敗 ({result})。{hint}";
        }

        return $"SteamAPI.InitEx() 失敗 ({result}): {steamErrMsg} {hint}";
    }
#endif
}
