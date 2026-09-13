using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PickaxePowerCont_ArrowShots : PickaxePowerCont_Base
{
    [SerializeField] GameObject obj_bow;
    [SerializeField] GameObject pf_Arrow;

    private SimpleAnimation anim_bow;
    private Quaternion bowBaseRotation = Quaternion.identity;
    private Tweener bowRotateTween;
    private CancellationTokenSource shootCts;

    private float damageRate => EquippedLevelData.value_1;
    private int damageCount => Mathf.Max(1, (int)EquippedLevelData.value_2);
    private int arrowCount => Mathf.Max(1, (int)EquippedLevelData.value_3);

    private int damage => (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
    private const float arrowSpeed = 30f;
    private const float arrowLifetime = 2.5f;
    private const float rotateDuration = 0.75f;
    private const float spawnDelay = 0.2f;
    private const float returnDelay = 0.25f;
    private static readonly Vector3 bowOffset = new Vector3(0f, 0.35f, 0f);

    private readonly List<PickaxePowerCont_ArrowUnit> list_arrowUnits = new List<PickaxePowerCont_ArrowUnit>();

    private void Awake()
    {
        anim_bow = obj_bow.GetComponent<SimpleAnimation>();
        bowBaseRotation = obj_bow.transform.rotation;
        obj_bow.SetActive(false);
    }

    public override void Activate()
    {
        Debug.Log("Power == ArrowShots");

        var bowPosition = AttackManager.Inst.currentPickaxePosition + bowOffset;
        obj_bow.transform.position = bowPosition;
        obj_bow.transform.rotation = bowBaseRotation;
        obj_bow.SetActive(false);
        obj_bow.SetActive(true);

        CameraManager.Inst.ShakeCamera_Large();
        StaticManager.SlowGameTime_PickaxePower();

        RotateAndShoot(bowPosition).Forget();
    }

    public override void GameEndCall()
    {
        StopBow();
        DeactivateAllArrows();
    }

    public override void OnDestroyCall()
    {
        StopBow();
        DeactivateAllArrows();
        base.OnDestroyCall();
    }

    void OnDestroy()
    {
        StopBow();
        list_arrowUnits.Clear();
    }

    private async UniTaskVoid RotateAndShoot(Vector3 bowPosition)
    {
        CancelShoot();
        shootCts = new CancellationTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            shootCts.Token,
            this.GetCancellationTokenOnDestroy());
        var token = linkedCts.Token;

        var canceled = await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay), cancellationToken: token)
            .SuppressCancellationThrow();
        if (canceled || obj_bow == null || !obj_bow.activeSelf) return;

        var count = arrowCount;
        var angleStep = 360f / count;
        var startAngle = UnityEngine.Random.Range(0f, 360f);
        var shotInterval = rotateDuration / count;

        obj_bow.transform.rotation = Quaternion.AngleAxis(startAngle, Vector3.up) * bowBaseRotation;
        KillBowRotate();
        bowRotateTween = obj_bow.transform
            .DORotate(new Vector3(0f, 360f, 0f), rotateDuration, RotateMode.WorldAxisAdd)
            .SetEase(Ease.Linear)
            .SetLink(obj_bow);

        anim_bow.Play("Shot");

        for (int i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested) return;

            var angle = startAngle + angleStep * i;
            var direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            ShotArrow(bowPosition, direction);

            if (i >= count - 1) break;

            canceled = await UniTask.Delay(TimeSpan.FromSeconds(shotInterval), cancellationToken: token)
                .SuppressCancellationThrow();
            if (canceled) return;
        }

        anim_bow.Play("Return");
        canceled = await UniTask.Delay(TimeSpan.FromSeconds(returnDelay), cancellationToken: token)
            .SuppressCancellationThrow();
        if (canceled) return;

        obj_bow.SetActive(false);
    }

    private void ShotArrow(Vector3 bowPosition, Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        var arrowUnit = Get_FreeArrowUnit();
        arrowUnit.transform.position = bowPosition;
        arrowUnit.transform.rotation = Quaternion.LookRotation(direction);
        arrowUnit.Init(damage, damageCount, arrowLifetime, direction * arrowSpeed);
    }

    private PickaxePowerCont_ArrowUnit Get_FreeArrowUnit()
    {
        var freeArrowUnit = list_arrowUnits.Find(x => !x.gameObject.activeSelf);
        if (freeArrowUnit == null)
        {
            freeArrowUnit = CreateArrowUnit();
        }
        return freeArrowUnit;
    }

    private PickaxePowerCont_ArrowUnit CreateArrowUnit()
    {
        var newArrow = Instantiate(pf_Arrow, transform) as GameObject;
        var newArrowUnit = newArrow.GetComponent<PickaxePowerCont_ArrowUnit>();
        list_arrowUnits.Add(newArrowUnit);
        return newArrowUnit;
    }

    private void StopBow()
    {
        CancelShoot();
        KillBowRotate();
        if (obj_bow != null)
        {
            obj_bow.SetActive(false);
        }
    }

    private void DeactivateAllArrows()
    {
        foreach (var arrowUnit in list_arrowUnits)
        {
            if (arrowUnit == null) continue;
            arrowUnit.Deactivate();
        }
    }

    private void CancelShoot()
    {
        if (shootCts == null) return;
        shootCts.Cancel();
        shootCts.Dispose();
        shootCts = null;
    }

    private void KillBowRotate()
    {
        if (bowRotateTween == null) return;
        bowRotateTween.Kill();
        bowRotateTween = null;
    }
}
