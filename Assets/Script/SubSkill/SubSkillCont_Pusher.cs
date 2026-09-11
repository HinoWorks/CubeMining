using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// Pusherスキル。取得時にユニットを生成し、インゲーム中に指定回数だけランダム発動する
/// </summary>
public class SubSkillCont_Pusher : SubSkillCont_Base
{
    private int activateCount => Mathf.Max(1, (int)(param.interval));
    private float ct => Random.Range(5f, 7.5f);
    [Header("Spawn")]
    [SerializeField] GameObject pf_pusher;
    [SerializeField] Vector3 spawnStartPosition = new Vector3(-11f, 0f, -9.5f);
    [SerializeField] Vector3 spawnStartPosition_upper = new Vector3(-11f, 0f, 9.5f);
    [SerializeField] float spawnInterval_min = 2.2f;
    [SerializeField] float spawnInterval_max = 11f;
    private float setSpawnInterval = 2.5f;

    private int spawnCount_base = 3;
    private int spawnCount_max = 11;
    private int spawnCount => GetSpawnCount();

    private readonly Vector3 spawnEulerAngles = new Vector3(0f, 180f, 0f);



    [Header("Activate")]
    [SerializeField] float excludeStartEndSeconds = 2f;
    [SerializeField] float moveDistance = 2.5f;
    [SerializeField] float activateStaggerSeconds = 0.08f;

    private readonly List<SubSkillCont_PusherUnit> pusherUnits = new List<SubSkillCont_PusherUnit>();
    private readonly List<SubSkillCont_PusherUnit> pusherUnits_2 = new List<SubSkillCont_PusherUnit>();
    private CancellationTokenSource scheduleCts;



    public override void Init(SubSkillParam _subSkilParam)
    {
        base.Init(_subSkilParam);
        CreateUnits();
    }

    public override void Set_AttackTrigger(bool isTrigger)
    {
        isActive = isTrigger;
        if (isTrigger)
        {
            StartActivateSchedule().Forget();
        }
        else
        {
            CancelSchedule();
            StopAllUnits();
        }
    }

    public override void OnDestroy()
    {
        CancelSchedule();
        DeleteUnits(pusherUnits);
        DeleteUnits(pusherUnits_2);
        base.OnDestroy();
    }

    private static void DeleteUnits(List<SubSkillCont_PusherUnit> units)
    {
        foreach (var unit in units)
        {
            if (unit == null) continue;
            unit.Delete();
        }
        units.Clear();
    }

    private void CreateUnits()
    {
        if (pf_pusher == null)
        {
            Debug.LogError("SubSkillCont_Pusher: pf_pusher is not assigned");
            return;
        }

        var count = spawnCount;
        setSpawnInterval = GetSpawnInterval(count);
        CreateUnit(pusherUnits, spawnStartPosition, Vector3.zero, setSpawnInterval, count);
        CreateUnit(pusherUnits_2, spawnStartPosition_upper, spawnEulerAngles, setSpawnInterval, count, true);
    }

    private int GetSpawnCount()
    {
        var count = spawnCount_base + (int)param.count;
        count = Mathf.Clamp(count, spawnCount_base, spawnCount_max);
        if (count % 2 == 0) count--;
        return Mathf.Max(1, count);
    }

    private float GetSpawnInterval(int count)
    {
        var sideCount = (count - 1) / 2;
        if (sideCount <= 0) return spawnInterval_max;
        return Mathf.Max(spawnInterval_min, spawnInterval_max / sideCount);
    }


    private void CreateUnit(List<SubSkillCont_PusherUnit> units, Vector3 startPosition, Vector3 eulerAngles, float interval, int count, bool isUpper = false)
    {
        var parent = InGameManager.Inst != null ? InGameManager.Inst.ParentPool : transform;
        var rotation = Quaternion.Euler(eulerAngles);

        for (int i = 0; i < count; i++)
        {
            var pairIndex = (i + 1) / 2;
            var side = (i % 2 == 1) ? -1f : 1f;
            var worldPos = startPosition + new Vector3(interval * pairIndex * side, 0f, 0f);
            var unitObj = Instantiate(pf_pusher, worldPos, rotation, parent);
            unitObj.transform.localScale = Vector3.one;

            var unit = unitObj.GetComponent<SubSkillCont_PusherUnit>();
            unit.Init(moveDistance, isUpper, worldPos);
            units.Add(unit);
        }
        units.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
    }



    private async UniTaskVoid StartActivateSchedule()
    {
        CancelSchedule();
        scheduleCts = new CancellationTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            scheduleCts.Token,
            this.GetCancellationTokenOnDestroy());

        var times = BuildActivateTimes();
        if (times.Count == 0) return;

        var prev = 0f;
        foreach (var time in times)
        {
            var delay = Mathf.Max(0f, time - prev);
            prev = time;

            var canceled = await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: linkedCts.Token)
                .SuppressCancellationThrow();
            if (canceled || !isActive) return;

            await ActivateUnits(linkedCts.Token);
            if (!isActive) return;
        }
    }

    private List<float> BuildActivateTimes()
    {
        var times = new List<float>();
        var count = Mathf.Max(0, activateCount);
        if (count <= 0) return times;

        var gameDuration = InGameManager.Inst != null ? InGameManager.Inst.RemainingTime : 0f;
        var exclude = Mathf.Max(0f, excludeStartEndSeconds);
        if (exclude * 2f >= gameDuration) exclude = 0f;

        var windowStart = exclude;
        var windowEnd = gameDuration - exclude;
        if (windowEnd <= windowStart) return times;

        var minGap = Mathf.Max(0f, ct);
        var earliest = windowStart;
        for (int i = 0; i < count; i++)
        {
            var remain = count - i;
            var latest = windowEnd - (remain - 1) * minGap;
            if (latest < earliest) break;

            var t = Random.Range(earliest, latest);
            times.Add(t);
            earliest = t + minGap;
        }

        return times;
    }

    private async UniTask ActivateUnits(CancellationToken token)
    {
        var count = Mathf.Max(pusherUnits.Count, pusherUnits_2.Count);
        var fromRightLower = Random.Range(0, 2) == 1;
        var fromRightUpper = Random.Range(0, 2) == 1;
        for (int i = 0; i < count; i++)
        {
            ActivateUnitAt(pusherUnits, GetStaggerIndex(pusherUnits.Count, i, fromRightLower));
            ActivateUnitAt(pusherUnits_2, GetStaggerIndex(pusherUnits_2.Count, i, fromRightUpper));

            if (i >= count - 1 || activateStaggerSeconds <= 0f) continue;
            var canceled = await UniTask.Delay(
                System.TimeSpan.FromSeconds(activateStaggerSeconds),
                cancellationToken: token).SuppressCancellationThrow();
            if (canceled || !isActive) return;
        }
    }

    private static int GetStaggerIndex(int unitCount, int step, bool fromRight)
    {
        if (step < 0 || step >= unitCount) return -1;
        return fromRight ? unitCount - 1 - step : step;
    }

    private static void ActivateUnitAt(List<SubSkillCont_PusherUnit> units, int index)
    {
        if (index < 0 || index >= units.Count) return;
        var unit = units[index];
        if (unit == null) return;
        unit.Active();
    }

    private void StopAllUnits()
    {
        StopUnits(pusherUnits);
        StopUnits(pusherUnits_2);
    }

    private static void StopUnits(List<SubSkillCont_PusherUnit> units)
    {
        foreach (var unit in units)
        {
            if (unit == null) continue;
            unit.StopMove(true);
        }
    }

    private void CancelSchedule()
    {
        if (scheduleCts == null) return;
        scheduleCts.Cancel();
        scheduleCts.Dispose();
        scheduleCts = null;
    }


}
