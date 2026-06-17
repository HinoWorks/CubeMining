using UnityEngine;

public class ForceFieldArea : MonoBehaviour
{
    public enum ForceType
    {
        Blast,    // 爆風: 中心から外へ
        Attract,  // 吸引: 中心へ
    }

    [SerializeField] private float radius = 3f;
    [SerializeField] private float force = 10f;
    [SerializeField] private LayerMask targetLayerMask;

    /// <summary>
    /// 現在位置を中心に爆風または吸引を発生させる
    /// </summary>
    public void Activate(ForceType _forceType, float _radiusRate = 1f, float _forceRate = 1f)
    {
        ApplyForceInRadius(_forceType, radius * _radiusRate, force * _forceRate);
    }

    /// <summary>
    /// 指定位置を中心に爆風または吸引を発生させる
    /// </summary>
    public void Activate(ForceType _forceType, Vector3 _position, float _radiusRate = 1f, float _forceRate = 1f)
    {
        transform.position = _position;
        ApplyForceInRadius(_forceType, radius * _radiusRate, force * _forceRate);
    }

    private void ApplyForceInRadius(ForceType _forceType, float _searchRadius, float _baseForce)
    {
        if (_searchRadius <= 0f || _baseForce <= 0f) return;

        var origin = transform.position;
        var colliders = Physics.OverlapSphere(origin, _searchRadius, targetLayerMask);

        foreach (var col in colliders)
        {
            if (col == null) continue;
            if (!col.TryGetComponent<IForce>(out var target)) continue;

            var offset = target.GetTransform().position - origin;
            var distance = offset.magnitude;
            if (distance <= Mathf.Epsilon || distance > _searchRadius) continue;

            var falloff = 1f - (distance / _searchRadius);
            var direction = _forceType == ForceType.Blast
                ? offset / distance
                : -offset / distance;

            target.ApplyForce(direction * (_baseForce * falloff));
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}
