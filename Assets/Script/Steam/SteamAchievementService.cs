#if STEAMWORKS_NET
using Steamworks;
using UnityEngine;

/// <summary>
/// Steamworks.NET 経由の実績・統計実装。STEAMWORKS_NET 定義時のみコンパイルされる。
/// </summary>
public sealed class SteamAchievementService : ISteamAchievementService
{
    public bool IsAvailable { get; }

    public SteamAchievementService(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }

    public bool IsUnlocked(string apiName)
    {
        if (!TryValidateApiName(apiName, nameof(IsUnlocked))) return false;

        if (!SteamUserStats.GetAchievement(apiName, out var achieved))
        {
            Debug.LogWarning($"[Steam] GetAchievement failed: {apiName}");
            return false;
        }

        return achieved;
    }

    public bool Unlock(string apiName)
    {
        if (!TryValidateApiName(apiName, nameof(Unlock))) return false;
        if (IsUnlocked(apiName)) return true;

        if (!SteamUserStats.SetAchievement(apiName))
        {
            Debug.LogWarning($"[Steam] SetAchievement failed: {apiName}");
            return false;
        }

        StoreStats();
        return true;
    }

    public bool SetProgress(string apiName, int current, int max)
    {
        if (!TryValidateApiName(apiName, nameof(SetProgress))) return false;
        if (max <= 0)
        {
            Debug.LogWarning($"[Steam] SetProgress max must be positive: {apiName}");
            return false;
        }

        current = Mathf.Clamp(current, 0, max);

        if (!SteamUserStats.IndicateAchievementProgress(apiName, (uint)current, (uint)max))
        {
            Debug.LogWarning($"[Steam] IndicateAchievementProgress failed: {apiName}");
            return false;
        }

        if (current < max) return true;

        return Unlock(apiName);
    }

    public bool GetStat(string apiName, out int value)
    {
        value = 0;
        if (!TryValidateApiName(apiName, nameof(GetStat))) return false;

        if (!SteamUserStats.GetStat(apiName, out value))
        {
            Debug.LogWarning($"[Steam] GetStat failed: {apiName}");
            return false;
        }

        return true;
    }

    public bool SetStat(string apiName, int value)
    {
        if (!TryValidateApiName(apiName, nameof(SetStat))) return false;

        if (!SteamUserStats.SetStat(apiName, value))
        {
            Debug.LogWarning($"[Steam] SetStat failed: {apiName}");
            return false;
        }

        return true;
    }

    public void StoreStats()
    {
        if (!IsAvailable) return;

        if (!SteamUserStats.StoreStats())
        {
            Debug.LogWarning("[Steam] StoreStats failed");
        }
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void ResetAllStats(bool achievementsToo)
    {
        if (!IsAvailable) return;

        if (!SteamUserStats.ResetAllStats(achievementsToo))
        {
            Debug.LogWarning($"[Steam] ResetAllStats failed (achievementsToo={achievementsToo})");
            return;
        }

        StoreStats();
        Debug.Log($"[Steam] ResetAllStats complete (achievementsToo={achievementsToo})");
    }
#endif

    private bool TryValidateApiName(string apiName, string caller)
    {
        if (!IsAvailable)
        {
            Debug.LogWarning($"[Steam] {caller} called while Steam is unavailable");
            return false;
        }

        if (string.IsNullOrEmpty(apiName))
        {
            Debug.LogWarning($"[Steam] {caller} called with empty apiName");
            return false;
        }

        return true;
    }
}
#endif
