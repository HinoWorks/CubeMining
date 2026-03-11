using UnityEngine;
using UniRx;
using System;

public class MiningTarget_BombAttackArea : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private LayerMask targetLayerMask;

    public void Explode(float _damage, float _sizeDelta = 1f)
    {
        var size_calc = radius * _sizeDelta;
        Debug.Log($"bomb size_calc => radius:{radius} _sizeDelta:{_sizeDelta} size_calc:{size_calc}");
        this.transform.localScale = new Vector3(size_calc, size_calc, size_calc);

        Observable.Timer(TimeSpan.FromSeconds(0.25f)).Subscribe(_ =>
        {
            var colliders = Physics.OverlapSphere(transform.position, size_calc, targetLayerMask);

            foreach (var col in colliders)
            {
                if (col == null) continue;

                if (col.TryGetComponent<IDamagable>(out var damagable) && damagable.isAlive)
                {
                    if (_damage <= 0) _damage = 1;
                    damagable.Damage((int)_damage);
                }
            }
        }).AddTo(this);

        Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
        {
            Destroy(this.gameObject);
        }).AddTo(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}

