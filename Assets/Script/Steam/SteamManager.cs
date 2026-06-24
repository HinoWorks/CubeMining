using UnityEngine;
#if STEAMWORKS_NET
using Steamworks;
#endif

/// <summary>
/// Steam API の初期化・コールバック処理を担当する。
/// 起動シーンに 1 つ配置し、DontDestroyOnLoad で常駐させる。
/// </summary>
[DefaultExecutionOrder(-200)]
public class SteamManager : MonoBehaviour
{
    public static SteamManager Inst { get; private set; }

    [SerializeField] private uint steamAppId = 480;
    [SerializeField] private bool restartAppIfNecessary = true;

    public bool IsSteamAvailable { get; private set; }
    public ISteamAchievementService Achievements { get; private set; }

#if STEAMWORKS_NET
    private bool isSteamInitialized;
#endif

    void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);

        Achievements = CreateAchievementService();
        IsSteamAvailable = Achievements.IsAvailable;

        if (!IsSteamAvailable)
        {
            Debug.Log("[Steam] Running without Steam integration.");
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
        if (!InitializeSteam())
        {
            return new NullSteamAchievementService();
        }

        return new SteamAchievementService(true);
#else
        return new NullSteamAchievementService();
#endif
    }

#if STEAMWORKS_NET
    private bool InitializeSteam()
    {
        if (restartAppIfNecessary)
        {
            var appId = new AppId_t(steamAppId);
            if (SteamAPI.RestartAppIfNecessary(appId))
            {
                Debug.Log("[Steam] RestartAppIfNecessary returned true. Quitting to relaunch via Steam.");
                Application.Quit();
                return false;
            }
        }

        if (!Packsize.Test())
        {
            Debug.LogError("[Steam] Packsize Test failed. Steamworks.NET の DLL 構成を確認してください。");
            return false;
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steam] DllCheck Test failed. steam_api64.dll / steam_api.dll を確認してください。");
            return false;
        }

        isSteamInitialized = SteamAPI.Init();
        if (!isSteamInitialized)
        {
            Debug.LogWarning("[Steam] SteamAPI.Init() failed. Steam クライアント起動と steam_appid.txt を確認してください。");
            return false;
        }

        var userName = SteamFriends.GetPersonaName();
        Debug.Log($"[Steam] Initialized. User: {userName} (AppId: {steamAppId})");
        return true;
    }
#endif
}
