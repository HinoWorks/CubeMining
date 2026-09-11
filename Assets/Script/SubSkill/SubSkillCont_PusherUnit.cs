using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Pusher本体。待機し、Active呼び出しで押し出し→復帰する
/// </summary>
public class SubSkillCont_PusherUnit : MonoBehaviour
{
    private Rigidbody rb;
    private Collider[] pushColliders;
    private readonly Collider[] overlapHits = new Collider[32];

    private Vector3 homePosition;
    private Sequence moveSeq;
    private bool isMoving;
    private Vector3 lastRbPosition;

    private float moveDuration = 1f;
    private Vector3 moveDistance;


    void Awake()
    {
        EnsureRigidbody();
    }

    public void Init(float _moveDistance, bool isUpper, Vector3 spawnPosition)
    {
        EnsureRigidbody();
        homePosition = spawnPosition;
        moveDistance = new Vector3(0f, 0f, isUpper ? -_moveDistance : _moveDistance);
        isMoving = false;
        StopMove(true);
    }

    public void Active()
    {
        if (isMoving || rb == null) return;
        isMoving = true;

        KillMove();
        SetPosition(homePosition);
        lastRbPosition = homePosition;
        WakeOverlappingCubes();

        var targetPosition = homePosition + moveDistance;
        var oneWayDuration = moveDuration;
        moveSeq = DOTween.Sequence().SetTarget(rb).SetUpdate(UpdateType.Fixed);
        moveSeq.Append(rb.DOMove(targetPosition, oneWayDuration).SetEase(Ease.InOutSine));
        moveSeq.Append(rb.DOMove(homePosition, oneWayDuration).SetEase(Ease.InOutSine));
        moveSeq.OnComplete(() =>
        {
            isMoving = false;
            SetPosition(homePosition);
        });
        moveSeq.Play();
    }

    void FixedUpdate()
    {
        if (!isMoving || rb == null) return;
        PushOverlappingCubes();
    }

    public void StopMove(bool resetPosition = false)
    {
        KillMove();
        isMoving = false;
        if (resetPosition)
        {
            SetPosition(homePosition);
        }
    }

    public void Delete()
    {
        KillMove();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        KillMove();
    }

    private void EnsureRigidbody()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        pushColliders = rb.GetComponentsInChildren<Collider>();
    }

    private void SetPosition(Vector3 position)
    {
        if (rb == null) return;
        rb.position = position;
        transform.position = position;
        lastRbPosition = position;
    }

    private void PushOverlappingCubes()
    {
        var step = rb.position - lastRbPosition;
        lastRbPosition = rb.position;
        if (Vector3.Dot(step, moveDistance) <= 0f) return;

        var pusherSpeed = step.magnitude / Time.fixedDeltaTime;
        if (pusherSpeed < 0.01f) return;

        var dir = moveDistance.normalized;
        ForEachOverlappingRigidbody(otherRb =>
        {
            otherRb.WakeUp();
            var velocity = otherRb.linearVelocity;
            var along = Vector3.Dot(velocity, dir);
            if (along >= pusherSpeed) return;
            otherRb.linearVelocity = velocity + dir * (pusherSpeed - along);
        });
    }

    private void WakeOverlappingCubes()
    {
        ForEachOverlappingRigidbody(otherRb => otherRb.WakeUp());
    }

    private void ForEachOverlappingRigidbody(Action<Rigidbody> onHit)
    {
        if (pushColliders == null) return;

        foreach (var pushCol in pushColliders)
        {
            if (pushCol == null || !pushCol.enabled) continue;
            var hitCount = Physics.OverlapBoxNonAlloc(
                pushCol.bounds.center,
                pushCol.bounds.extents,
                overlapHits,
                Quaternion.identity);
            for (int i = 0; i < hitCount; i++)
            {
                var hit = overlapHits[i];
                if (hit == null) continue;
                var otherRb = hit.attachedRigidbody;
                if (otherRb == null || otherRb == rb || otherRb.isKinematic) continue;
                onHit(otherRb);
            }
        }
    }

    private void KillMove()
    {
        if (moveSeq != null && moveSeq.IsActive())
        {
            moveSeq.Kill();
        }
        moveSeq = null;
        if (rb != null) rb.DOKill();
        transform.DOKill();
    }
}
