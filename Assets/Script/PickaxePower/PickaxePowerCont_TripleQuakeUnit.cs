using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PickaxePowerCont_TripleQuakeUnit : MonoBehaviour
{
    [SerializeField] ParticleSystem eff_Attack;
    [SerializeField] float baseRadius = 1.5f;

    private static readonly float[] SizeMultipliers = { 1f, 1.5f, 2f };
    private const float AttackInterval = 0.75f;
    private const int AttackCount = 3;

    private int damage;
    private float sizeRate;
    private Vector3 centerPosition;
    private CancellationTokenSource sequenceCts;
    private Action onFinished;

    public void Init(int _damage, float _sizeRate, Vector3 _centerPosition, Action _onFinished = null)
    {
        CancelSequence();

        damage = _damage;
        sizeRate = _sizeRate > 0f ? _sizeRate : 1f;
        centerPosition = _centerPosition;
        onFinished = _onFinished;

        sequenceCts = new CancellationTokenSource();
        AttackSequence(sequenceCts.Token).Forget();
    }

    public void CancelAndDestroy()
    {
        CancelSequence();
        Destroy(gameObject);
    }

    private async UniTaskVoid AttackSequence(CancellationToken token)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            token,
            this.GetCancellationTokenOnDestroy());

        for (int i = 0; i < AttackCount; i++)
        {
            ExecuteAttack(SizeMultipliers[i]);
            if (i < AttackCount - 1)
            {
                var canceled = await UniTask.Delay(TimeSpan.FromSeconds(AttackInterval), cancellationToken: linkedCts.Token)
                    .SuppressCancellationThrow();
                if (canceled) return;
            }
        }

        CameraManager.Inst.ShakeCamera_Large();
        //StaticManager.SlowGameTime_PickaxePower();

        var endCanceled = await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: linkedCts.Token)
            .SuppressCancellationThrow();
        if (endCanceled) return;

        onFinished?.Invoke();
        Destroy(gameObject);
    }

    private void ExecuteAttack(float sizeMultiplier)
    {
        var areaScale = sizeRate * sizeMultiplier;
        var radius = baseRadius * areaScale;

        if (eff_Attack != null)
        {
            eff_Attack.transform.localScale = Vector3.one * areaScale;
            eff_Attack.Stop();
            eff_Attack.Play();
        }

        ApplyDamageInRadius(radius);
        CameraManager.Inst.ShakeCamera_Large();
    }

    private void ApplyDamageInRadius(float radius)
    {
        var colliders = Physics.OverlapSphere(centerPosition, radius);
        var hitTargets = new HashSet<IDamagable>();

        foreach (var col in colliders)
        {
            if (!col.TryGetComponent(out IDamagable target)) continue;
            if (!target.isAlive) continue;
            if (!hitTargets.Add(target)) continue;

            target.Damage(damage);
        }
    }

    void OnDestroy()
    {
        CancelSequence();
    }

    private void CancelSequence()
    {
        if (sequenceCts == null) return;
        sequenceCts.Cancel();
        sequenceCts.Dispose();
        sequenceCts = null;
    }
}
