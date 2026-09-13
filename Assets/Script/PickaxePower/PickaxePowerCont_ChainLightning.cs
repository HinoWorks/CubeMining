using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PickaxePowerCont_ChainLightning : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_Lightning;
    [SerializeField] GameObject pf_LightningOrigin;

    private float damageRate => EquippedLevelData.value_1;
    private int basePointCount => Mathf.Max(1, (int)EquippedLevelData.value_2);
    private int attackCount => Mathf.Max(1, (int)EquippedLevelData.value_3);


    private float attackDuration = 2f;
    private float initialDelay = 0.5f;
    private const float sizeRate = 1f;
    private const float originDestroyDelay = 1f;

    private readonly List<PickaxePowerCont_ChainLightningUnit> list_lightningUnits = new List<PickaxePowerCont_ChainLightningUnit>();
    private readonly List<PickaxePowerCont_ChainLightningOrigin> list_originUnits = new List<PickaxePowerCont_ChainLightningOrigin>();
    private readonly List<Vector2> list_baseCorners = new List<Vector2>();
    private CancellationTokenSource attackCts;

    private int damage => (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
    private const float BoltSpeed = 150f;
    private Vector3 basePositionOffset = new Vector3(0f, 1.5f, 0f);


    private static readonly Vector2[] ViewportCorners =
    {
        new Vector2(0.1f, 0.1f),
        //new Vector2(0.1f, 0.9f),
        new Vector2(0.9f, 0.1f),
        //new Vector2(0.9f, 0.9f),
        new Vector2(0.1f, 0.5f),
        new Vector2(0.9f,0.5f)
    };

    public override void Activate()
    {
        Debug.Log("Power == ChainLightning");

        SpawnBaseLightnings();

        //CameraManager.Inst.ShakeCamera_Large();
        //StaticManager.SlowGameTime_PickaxePower();

        AttackLoop().Forget();
    }

    public override void GameEndCall()
    {
        StopAttack();
        DeactivateAllLightningUnits();
        DestroyAllOrigins();
    }

    public override void OnDestroyCall()
    {
        StopAttack();
        DeactivateAllLightningUnits();
        DestroyAllOrigins();
        base.OnDestroyCall();
    }

    void OnDestroy()
    {
        StopAttack();
        list_lightningUnits.Clear();
        list_originUnits.Clear();
        list_baseCorners.Clear();
    }

    private void SpawnBaseLightnings()
    {
        DestroyAllOrigins();
        list_baseCorners.Clear();

        var count = basePointCount;
        var corners = PickRandomCorners(count);
        var targetY = AttackManager.Inst.currentPickaxePosition.y;

        EnsureUnitCount(count);

        for (int i = 0; i < count; i++)
        {
            list_baseCorners.Add(corners[i]);

            var spawnPosition = ViewportCornerToWorld(corners[i], targetY);

            var unit = list_lightningUnits[i];
            unit.transform.position = spawnPosition;
            unit.gameObject.SetActive(true);

            CreateOriginUnit(spawnPosition);
        }

        for (int i = count; i < list_lightningUnits.Count; i++)
        {
            list_lightningUnits[i].Deactivate();
        }
    }

    private async UniTaskVoid AttackLoop()
    {
        CancelAttack();
        attackCts = new CancellationTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            attackCts.Token,
            this.GetCancellationTokenOnDestroy());
        var token = linkedCts.Token;

        var count = attackCount;

        var canceled = await UniTask.Delay(TimeSpan.FromSeconds(initialDelay), cancellationToken: token).SuppressCancellationThrow();
        if (canceled) return;
        for (int i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested) return;

            AttackReadyAnim();
            canceled = await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token).SuppressCancellationThrow();
            if (canceled) return;

            ExecuteAttack();
            CameraManager.Inst.ShakeCamera_Large();
            if (i >= count - 1) break;
        }

        var destroyCanceled = await UniTask.Delay(TimeSpan.FromSeconds(originDestroyDelay), cancellationToken: token)
            .SuppressCancellationThrow();
        if (destroyCanceled) return;

        PlayEndAllOrigins();
    }

    private void ExecuteAttack()
    {
        var targetPosition = AttackManager.Inst.currentPickaxePosition;
        var count = list_baseCorners.Count;
        for (int i = 0; i < count; i++)
        {
            var startPosition = ViewportCornerToWorld(list_baseCorners[i], targetPosition.y);
            var origin = i < list_originUnits.Count ? list_originUnits[i] : null;
            ShotLightning(list_lightningUnits[i], origin, startPosition, targetPosition);
            origin?.PlayShotEff();
        }

        SoundManager.Inst.PlaySE(201, true);
    }

    private void AttackReadyAnim()
    {
        var count = list_baseCorners.Count;
        for (int i = 0; i < count; i++)
        {
            var origin = i < list_originUnits.Count ? list_originUnits[i] : null;
            origin?.PlayShot();
        }
    }

    private void ShotLightning(
        PickaxePowerCont_ChainLightningUnit unit,
        PickaxePowerCont_ChainLightningOrigin origin,
        Vector3 startPosition,
        Vector3 targetPosition)
    {

        var startPosi_calculated = startPosition + basePositionOffset;
        var direction = targetPosition - startPosi_calculated;
        if (direction.sqrMagnitude < 0.001f) return;

        unit.transform.position = startPosi_calculated;
        unit.transform.rotation = Quaternion.LookRotation(direction.normalized);
        unit.Init(damage, sizeRate, direction.normalized * BoltSpeed, targetPosition);
        //origin?.PlayShot();
    }

    private Vector2[] PickRandomCorners(int count)
    {
        var picked = new Vector2[count];
        var available = new List<Vector2>(ViewportCorners);

        for (int i = 0; i < count; i++)
        {
            if (available.Count == 0)
            {
                available.AddRange(ViewportCorners);
            }

            var index = UnityEngine.Random.Range(0, available.Count);
            picked[i] = available[index];
            available.RemoveAt(index);
        }

        return picked;
    }

    private static Vector3 ViewportCornerToWorld(Vector2 viewport, float targetY)
    {
        var cam = Camera.main;
        if (cam == null)
        {
            return new Vector3(viewport.x * 20f - 10f, targetY, viewport.y * 20f - 10f);
        }

        var ray = cam.ViewportPointToRay(new Vector3(viewport.x, viewport.y, 0f));
        var plane = new Plane(Vector3.up, new Vector3(0f, targetY, 0f));
        if (plane.Raycast(ray, out var enter))
        {
            return ray.GetPoint(enter);
        }

        var world = cam.ViewportToWorldPoint(new Vector3(viewport.x, viewport.y, cam.nearClipPlane + 10f));
        return new Vector3(world.x, targetY, world.z);
    }

    private void EnsureUnitCount(int count)
    {
        while (list_lightningUnits.Count < count)
        {
            CreateLightningUnit();
        }
    }

    private PickaxePowerCont_ChainLightningUnit CreateLightningUnit()
    {
        var newLightning = Instantiate(pf_Lightning, transform) as GameObject;
        var newUnit = newLightning.GetComponent<PickaxePowerCont_ChainLightningUnit>();
        list_lightningUnits.Add(newUnit);
        return newUnit;
    }

    private PickaxePowerCont_ChainLightningOrigin CreateOriginUnit(Vector3 position)
    {
        if (pf_LightningOrigin == null) return null;

        var newOrigin = Instantiate(pf_LightningOrigin, position, Quaternion.identity, transform);
        var newUnit = newOrigin.GetComponent<PickaxePowerCont_ChainLightningOrigin>();
        if (newUnit == null)
        {
            newUnit = newOrigin.AddComponent<PickaxePowerCont_ChainLightningOrigin>();
        }
        newUnit.PlaySpawn();
        list_originUnits.Add(newUnit);
        return newUnit;
    }

    private void StopAttack()
    {
        CancelAttack();
    }

    private void DeactivateAllLightningUnits()
    {
        foreach (var unit in list_lightningUnits)
        {
            if (unit == null) continue;
            unit.Deactivate();
        }
    }

    private void PlayEndAllOrigins()
    {
        foreach (var origin in list_originUnits)
        {
            if (origin == null) continue;
            origin.PlayEndAndDestroy();
        }
        list_originUnits.Clear();
    }

    private void DestroyAllOrigins()
    {
        foreach (var origin in list_originUnits)
        {
            if (origin == null) continue;
            Destroy(origin.gameObject);
        }
        list_originUnits.Clear();
    }

    private void CancelAttack()
    {
        if (attackCts == null) return;
        attackCts.Cancel();
        attackCts.Dispose();
        attackCts = null;
    }
}
