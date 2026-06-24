using UnityEngine;

/// <summary>
/// Steam 未接続時（Editor / 非 Steam ビルド / 初期化失敗）の no-op 実装。
/// </summary>
public sealed class NullSteamAchievementService : ISteamAchievementService
{
    public bool IsAvailable => false;

    public bool IsUnlocked(string apiName)
    {
        LogSkipped(nameof(IsUnlocked), apiName);
        return false;
    }

    public bool Unlock(string apiName)
    {
        LogSkipped(nameof(Unlock), apiName);
        return false;
    }

    public bool SetProgress(string apiName, int current, int max)
    {
        LogSkipped($"{nameof(SetProgress)} ({current}/{max})", apiName);
        return false;
    }

    public bool GetStat(string apiName, out int value)
    {
        value = 0;
        LogSkipped(nameof(GetStat), apiName);
        return false;
    }

    public bool SetStat(string apiName, int value)
    {
        LogSkipped($"{nameof(SetStat)} ({value})", apiName);
        return false;
    }

    public void StoreStats()
    {
        LogSkipped(nameof(StoreStats), null);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void ResetAllStats(bool achievementsToo)
    {
        LogSkipped($"{nameof(ResetAllStats)} (achievementsToo={achievementsToo})", null);
    }
#endif

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    private static void LogSkipped(string action, string apiName)
    {
        if (string.IsNullOrEmpty(apiName))
        {
            Debug.Log($"[Steam] Skipped {action} (Steam unavailable)");
            return;
        }

        Debug.Log($"[Steam] Skipped {action}: {apiName} (Steam unavailable)");
    }
}
