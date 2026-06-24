/// <summary>
/// Steam 実績・統計 API の抽象化。ゲームロジックはこのインターフェース経由でのみ Steam にアクセスする。
/// </summary>
public interface ISteamAchievementService
{
    bool IsAvailable { get; }

    bool IsUnlocked(string apiName);
    bool Unlock(string apiName);
    bool SetProgress(string apiName, int current, int max);

    bool GetStat(string apiName, out int value);
    bool SetStat(string apiName, int value);

    void StoreStats();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    void ResetAllStats(bool achievementsToo);
#endif
}
