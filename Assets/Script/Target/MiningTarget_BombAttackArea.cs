using UnityEngine;
using UniRx;
using System;

public class MiningTarget_BombAttackArea : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;

    public void Explode(float _damage, float _bomSize)
    {
        this.transform.localScale = _bomSize * Vector3.one;
        Debug.Log($"bomb size_ => {_bomSize}");
        Observable.Timer(TimeSpan.FromSeconds(0.25f)).Subscribe(_ =>
        {
            var origin = transform.position;
            var colliders = Physics.OverlapSphere(origin, _bomSize, targetLayerMask);

            foreach (var col in colliders)
            {
                if (col == null) continue;
                if (!col.TryGetComponent<IDamagable>(out var damagable) || !damagable.isAlive) continue;
                if (!IsWithinRadius(origin, damagable.GetTransform().position, _bomSize)) continue;

                if (_damage <= 0) _damage = 1;
                damagable.Damage((int)_damage);
            }
        }).AddTo(this);

        Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
        {
            Destroy(this.gameObject);
        }).AddTo(this);
    }

    private static bool IsWithinRadius(Vector3 _origin, Vector3 _targetPosition, float _radius)
    {
        return (_targetPosition - _origin).sqrMagnitude <= _radius * _radius;
    }

}

