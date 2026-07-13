using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PickaxePowerCont_ChainLightningUnit : MonoBehaviour
{
    [SerializeField] TriggerSender triggerSender;

    private int damage;
    private float sizeRate;
    private Vector3 velocity;
    private Vector3 startPosition;
    private float maxDistanceSqr;
    private Rigidbody rb;
    private readonly HashSet<IDamagable> hitTargets = new HashSet<IDamagable>();
    private bool isLaunched;
    private CancellationTokenSource shotCts;
    private float delayDamageTime = 0.35f;

    private Vector3 baseScale = Vector3.one;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseScale = transform.localScale;
        if (triggerSender != null)
        {
            triggerSender.OnEnter += OnTriggerEnter;
        }
    }

    void OnDestroy()
    {
        CancelShotDelay();
        if (triggerSender != null)
        {
            triggerSender.OnEnter -= OnTriggerEnter;
        }
    }

    public void Init(int _damage, float _sizeRate, Vector3 _velocity, Vector3 targetPosition)
    {
        CancelShotDelay();

        damage = _damage;
        sizeRate = _sizeRate > 0f ? _sizeRate : 1f;
        velocity = _velocity;
        startPosition = transform.position;
        maxDistanceSqr = (targetPosition - startPosition).sqrMagnitude;
        hitTargets.Clear();
        isLaunched = false;

        transform.localScale = baseScale * sizeRate;
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(true);

        PlayChainEffect(startPosition, targetPosition);
        ShotAfterDelay().Forget();
    }

    private async UniTaskVoid ShotAfterDelay()
    {
        shotCts = new CancellationTokenSource();
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            shotCts.Token,
            this.GetCancellationTokenOnDestroy());

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayDamageTime), cancellationToken: linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            linkedCts.Dispose();
        }

        if (!gameObject.activeSelf) return;

        isLaunched = true;
        rb.linearVelocity = velocity;
    }

    void FixedUpdate()
    {
        if (!gameObject.activeSelf || !isLaunched) return;
        if ((transform.position - startPosition).sqrMagnitude >= maxDistanceSqr)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLaunched) return;
        if (!other.TryGetComponent(out IDamagable target)) return;
        if (!target.isAlive) return;
        if (!hitTargets.Add(target)) return;

        target.Damage(damage);

        var effect = EffectManager.Inst?.Get_Effect(EffectType.LasaerHit);
        if (effect != null)
        {
            effect.transform.position = target.GetTransform().position;
            effect.SetActive(true);
        }
    }

    private void PlayChainEffect(Vector3 from, Vector3 to)
    {
        var effChain = EffectManager.Inst?.Get_EffectCont(EffectType.ThunderStrike_Chain);
        if (effChain == null) return;

        // AttackCont_Thunder と同様: 終点に置き、起点方向へ伸ばす
        effChain.transform.position = new Vector3(to.x, to.y, to.z);

        var direction = from - to;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        var distance = direction.magnitude;
        direction /= distance;

        var angleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        effChain.transform.rotation = Quaternion.Euler(90f, angleY, 0f);

        // 弾の localScale（baseScale × sizeRate）に合わせてエフェクト太さを追従
        var boltScale = transform.localScale;
        effChain.SetParticle3DSize(0.5f * boltScale.x, distance / 2f, 1f * boltScale.z);
        effChain.gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        CancelShotDelay();
        isLaunched = false;
        rb.linearVelocity = Vector3.zero;
        hitTargets.Clear();
        gameObject.SetActive(false);
    }

    private void CancelShotDelay()
    {
        if (shotCts == null) return;
        shotCts.Cancel();
        shotCts.Dispose();
        shotCts = null;
    }
}
