using System.Collections.Generic;
using System.Numerics;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// SO_AchievementData に基づき実績条件を判定し、Steam に反映する。
/// 「Unlocked」ログはゲーム条件達成時。Steam への反映結果は別ログで出す。
/// </summary>
[DefaultExecutionOrder(-199)]
public class SteamAchievementManager : MonoBehaviour
{
    public static SteamAchievementManager Inst { get; private set; }

    private readonly HashSet<string> earnedInGameKeys = new();
    private readonly HashSet<string> steamSyncedKeys = new();
    private bool isEvaluating;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Inst != null) return;
        var go = new GameObject(nameof(SteamAchievementManager));
        go.AddComponent<SteamAchievementManager>();
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
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(OnGameStateChanged).AddTo(this);
        GameEvent.PlayerLevel.LevelUp.Subscribe(_ => EvaluateAllAsync().Forget()).AddTo(this);
        GameEvent.AchieveEvent.SkillTreeUnlock.Subscribe(_ => EvaluateAllAsync().Forget()).AddTo(this);
        GameEvent.AchieveEvent.PickaxeCraft.Subscribe(_ => EvaluateAllAsync().Forget()).AddTo(this);
        EvaluateAllAsync().Forget();
    }

    void OnDestroy()
    {
        if (Inst == this) Inst = null;
    }

    private void OnGameStateChanged(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.Title:
            case GameStateType.Result:
                EvaluateAllAsync().Forget();
                break;
        }
    }

    public void NotifySaveDataUpdated(GameRecordData gameRecordOverride = null)
    {
        EvaluateAllAsync(gameRecordOverride).Forget();
    }

    public bool TryUnlock(string achievementKey)
    {
        var data = SOLoader.AchievementData.GetByKey(achievementKey);
        if (data == null)
        {
            Debug.LogWarning($"[SteamAchievement] Unknown key: {achievementKey}");
            return false;
        }

        MarkEarnedInGame(data);
        return SyncToSteam(data);
    }

    public async UniTask EvaluateAllAsync(GameRecordData gameRecordOverride = null)
    {
        if (isEvaluating) return;
        isEvaluating = true;

        try
        {
            if (SaveLoader.Inst == null)
            {
                Debug.Log("[SteamAchievement] Skip: SaveLoader.Inst is null");
                return;
            }

            if (!await WaitForSteamAsync())
            {
                Debug.Log("[SteamAchievement] Skip: Steam is not available");
                return;
            }

            var service = SteamManager.Inst.Achievements;
            var achievementDatas = SOLoader.AchievementData.GetAutoEvaluateDatas();
            if (achievementDatas.Length == 0)
            {
                Debug.LogWarning("[SteamAchievement] Skip: SO_AchievementData has no entries");
                return;
            }

            var gameRecord = gameRecordOverride ?? await SaveLoader.Inst.Get_GameRecordData();
            var playerLevelData = await SaveLoader.Inst.Get_PlayerLevelData();
            var playerLevel = playerLevelData?.level ?? 1;
            var artifactOwnedCount = SaveLoader.Inst.Get_ArtifactTotalCount();
            var pickaxeOwnedCount = SaveLoader.Inst.Get_PickaxeTotalCount();
            var skillTreeOwnedCount = SaveLoader.Inst.Get_SkillTreeTotalCount();

            foreach (var data in achievementDatas)
            {
                if (!TryGetProgress(
                        data,
                        gameRecord,
                        playerLevel,
                        artifactOwnedCount,
                        pickaxeOwnedCount,
                        skillTreeOwnedCount,
                        out var current,
                        out var target))
                {
                    continue;
                }

                ApplyAchievement(data, service, current, target);
            }
        }
        finally
        {
            isEvaluating = false;
        }
    }

    private static async UniTask<bool> WaitForSteamAsync()
    {
        const int maxAttempts = 30;
        const int retryDelayMs = 200;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var steam = SteamManager.Inst;
            if (steam != null && steam.IsSteamAvailable && steam.Achievements != null && steam.Achievements.IsAvailable)
            {
                return true;
            }

            if (attempt < maxAttempts)
            {
                await UniTask.Delay(retryDelayMs);
            }
        }

        return false;
    }

    private static bool TryGetProgress(
        AchievementUnitData data,
        GameRecordData gameRecord,
        int playerLevel,
        int artifactOwnedCount,
        int pickaxeOwnedCount,
        int skillTreeOwnedCount,
        out int current,
        out int target)
    {
        current = 0;
        target = Mathf.Max(1, data.threshold);

        switch (data.conditionType)
        {
            case AchievementConditionType.TotalRecord:
                current = ToClampedInt(SO_AchievementData.GetTotalRecordValue(gameRecord, data.recordField));
                return data.recordField != AchievementRecordField.None;

            case AchievementConditionType.OneGameBest:
                current = ToClampedInt(SO_AchievementData.GetOneGameBestValue(gameRecord, data.recordField));
                return data.recordField != AchievementRecordField.None;

            case AchievementConditionType.PlayerLevel:
                current = playerLevel;
                return true;

            case AchievementConditionType.ArtifactOwned:
                current = artifactOwnedCount;
                return true;

            case AchievementConditionType.PickaxeOwned:
                current = pickaxeOwnedCount;
                return true;

            case AchievementConditionType.SkillTreeOwned:
                current = skillTreeOwnedCount;
                return true;

            default:
                return false;
        }
    }

    private void ApplyAchievement(
        AchievementUnitData data,
        ISteamAchievementService service,
        int current,
        int target)
    {
        if (data.useProgress && current < target)
        {
            service.SetProgress(data.steamApiName, current, target);
            return;
        }

        if (current < target) return;

        MarkEarnedInGame(data);
        SyncToSteam(data, service);
    }

    private void MarkEarnedInGame(AchievementUnitData data)
    {
        if (earnedInGameKeys.Contains(data.achievementKey)) return;

        earnedInGameKeys.Add(data.achievementKey);
        Debug.Log($"[SteamAchievement] Unlocked: {data.achievementKey} ({data.steamApiName})");
    }

    private bool SyncToSteam(AchievementUnitData data, ISteamAchievementService service = null)
    {
        if (string.IsNullOrEmpty(data.steamApiName))
        {
            Debug.LogWarning($"[SteamAchievement] steamApiName is empty: {data.achievementKey}");
            return false;
        }

        if (steamSyncedKeys.Contains(data.achievementKey)) return true;

        service ??= SteamManager.Inst?.Achievements;
        if (service == null || !service.IsAvailable)
        {
            Debug.LogWarning($"[SteamAchievement] Steam sync skipped: {data.achievementKey}");
            return false;
        }

        if (service.IsUnlocked(data.steamApiName))
        {
            steamSyncedKeys.Add(data.achievementKey);
            return true;
        }

        if (!service.Unlock(data.steamApiName))
        {
            Debug.LogWarning(
                $"[SteamAchievement] Steam sync failed: {data.achievementKey} ({data.steamApiName}). " +
                "Steam パートナーに API Name が登録されているか、App ID を確認してください。");
            return false;
        }

        steamSyncedKeys.Add(data.achievementKey);
        Debug.Log($"[SteamAchievement] Steam synced: {data.achievementKey} ({data.steamApiName})");
        return true;
    }

    private static int ToClampedInt(BigInteger value)
    {
        if (value <= 0) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        return (int)value;
    }
}
