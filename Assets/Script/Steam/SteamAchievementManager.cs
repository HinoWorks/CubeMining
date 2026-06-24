using System.Collections.Generic;
using System.Numerics;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// SO_AchievementData に基づき実績条件を判定し、Steam に反映する。
/// </summary>
[DefaultExecutionOrder(-199)]
public class SteamAchievementManager : MonoBehaviour
{
    public static SteamAchievementManager Inst { get; private set; }

    private readonly HashSet<string> unlockedKeyCache = new();

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

    /// <summary>リザルト保存後など、セーブデータを再評価する。</summary>
    public void NotifySaveDataUpdated()
    {
        EvaluateAllAsync().Forget();
    }

    /// <summary>Manual 定義の実績をコードから付与する。</summary>
    public bool TryUnlock(string achievementKey)
    {
        var data = SOLoader.AchievementData.GetByKey(achievementKey);
        if (data == null)
        {
            Debug.LogWarning($"[SteamAchievement] Unknown key: {achievementKey}");
            return false;
        }

        return UnlockAchievement(data, data.threshold, data.threshold);
    }

    public async UniTask EvaluateAllAsync()
    {
        if (SaveLoader.Inst == null) return;

        var service = SteamManager.Inst?.Achievements;
        if (service == null || !service.IsAvailable) return;

        var achievementDatas = SOLoader.AchievementData.GetAutoEvaluateDatas();
        if (achievementDatas.Length == 0) return;

        var gameRecord = await SaveLoader.Inst.Get_GameRecordData();
        var playerLevelData = await SaveLoader.Inst.Get_PlayerLevelData();
        var playerLevel = playerLevelData?.level ?? 1;
        var artifactOwnedCount = SaveLoader.Inst.Get_ArtifactTotalCount();
        var pickaxeOwnedCount = SaveLoader.Inst.Get_PickaxeTotalCount();
        var skillTreeOwnedCount = SaveLoader.Inst.Get_SkillTreeTotalCount();

        foreach (var data in achievementDatas)
        {
            if (!TryGetProgress(data, gameRecord, playerLevel, artifactOwnedCount, pickaxeOwnedCount, skillTreeOwnedCount, out var current, out var target))
            {
                continue;
            }

            ApplyAchievement(data, service, current, target);
        }
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
        if (IsAlreadyUnlocked(data, service)) return;

        if (data.useProgress && current < target)
        {
            service.SetProgress(data.steamApiName, current, target);
            return;
        }

        if (current < target) return;

        UnlockAchievement(data, current, target);
    }

    private bool UnlockAchievement(AchievementUnitData data, int current, int target)
    {
        if (string.IsNullOrEmpty(data.steamApiName))
        {
            Debug.LogWarning($"[SteamAchievement] steamApiName is empty: {data.achievementKey}");
            return false;
        }

        var service = SteamManager.Inst?.Achievements;
        if (service == null || !service.IsAvailable) return false;

        if (IsAlreadyUnlocked(data, service)) return true;

        if (data.useProgress && current < target)
        {
            service.SetProgress(data.steamApiName, current, target);
            return false;
        }

        var unlocked = service.Unlock(data.steamApiName);
        if (unlocked)
        {
            unlockedKeyCache.Add(data.achievementKey);
            Debug.Log($"[SteamAchievement] Unlocked: {data.achievementKey} ({data.steamApiName})");
        }

        return unlocked;
    }

    private bool IsAlreadyUnlocked(AchievementUnitData data, ISteamAchievementService service)
    {
        if (unlockedKeyCache.Contains(data.achievementKey)) return true;
        if (service.IsUnlocked(data.steamApiName))
        {
            unlockedKeyCache.Add(data.achievementKey);
            return true;
        }

        return false;
    }

    private static int ToClampedInt(BigInteger value)
    {
        if (value <= 0) return 0;
        if (value > int.MaxValue) return int.MaxValue;
        return (int)value;
    }
}
