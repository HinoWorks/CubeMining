using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PickaxePowerCont_ArrowUnit : MonoBehaviour
{
    private int damage;
    private int remainHitCount;
    private Vector3 velocity;
    private Rigidbody rb;
    private TrailRenderer trail;
    private CancellationTokenSource lifetimeCts;
    private readonly HashSet<IDamagable> hitTargets = new HashSet<IDamagable>();

    [SerializeField] TriggerSender triggerSender;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        triggerSender.OnEnter += OnTriggerEnter;
    }

    void OnDestroy()
    {
        CancelLifetime();
        triggerSender.OnEnter -= OnTriggerEnter;
    }

    public void Init(int _damage, int _damageCount, float _lifetime, Vector3 _velocity)
    {
        CancelLifetime();
        hitTargets.Clear();

        damage = _damage;
        remainHitCount = Mathf.Max(1, _damageCount);
        velocity = _velocity;

        if (trail != null)
        {
            trail.Clear();
        }

        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(true);
        rb.linearVelocity = velocity;

        StartLifetime(_lifetime).Forget();
    }

    public void Deactivate()
    {
        if (!gameObject.activeSelf) return;

        CancelLifetime();
        hitTargets.Clear();
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private async UniTaskVoid StartLifetime(float lifetime)
    {
        lifetimeCts = new CancellationTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            lifetimeCts.Token,
            this.GetCancellationTokenOnDestroy());

        var canceled = await UniTask.Delay(TimeSpan.FromSeconds(lifetime), cancellationToken: linkedCts.Token)
            .SuppressCancellationThrow();
        if (canceled) return;

        Deactivate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeSelf) return;
        if (!other.TryGetComponent(out IDamagable target)) return;
        if (!target.isAlive) return;
        if (!hitTargets.Add(target)) return;

        target.Damage(damage);

        var eff_arrowHit = EffectManager.Inst.Get_Effect(EffectType.ArrowHit);
        if (eff_arrowHit != null)
        {
            eff_arrowHit.transform.position = transform.position;
            eff_arrowHit.SetActive(true);
        }

        remainHitCount--;
        if (remainHitCount <= 0)
        {
            Deactivate();
        }
    }

    private void CancelLifetime()
    {
        if (lifetimeCts == null) return;
        lifetimeCts.Cancel();
        lifetimeCts.Dispose();
        lifetimeCts = null;
    }
}
