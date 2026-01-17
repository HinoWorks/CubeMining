using System.Runtime.CompilerServices;
using UnityEngine;

public class BulletCont_Bound : BulletBase
{


    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        base.Init(_damage, _lifetime, _direction);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("AroundWall"))
        {
            Reflect(other);
            return;
        }

        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
            if (bulletType == BulletType.Piercing) return;
            base.ReturnToPool();
        }
    }


    void Reflect(Collider wall)
    {
        Vector3 dir = rb.linearVelocity.normalized;
        float speed = rb.linearVelocity.magnitude;

        // レイキャストが失敗した場合は、壁の最も近い点を使用して法線を推定
        Vector3 closestPoint = wall.ClosestPoint(transform.position);
        Vector3 toWall = (closestPoint - (transform.position - dir * 2f)).normalized;
        Vector3 estimatedNormal = -toWall;
        Vector3 reflectedDir = Vector3.Reflect(dir, estimatedNormal);

        // 壁の向きに合わせて反射方向を修正
        float mod_x = 1f;
        float mod_z = 1f;
        if (wall.transform.position.x < -1f || wall.transform.position.x > 1f) mod_z = -1f;
        else if (wall.transform.position.z < -1f || wall.transform.position.z > 1f) mod_x = -1f;

        rb.linearVelocity = new Vector3(reflectedDir.x * mod_x, reflectedDir.y, reflectedDir.z * mod_z) * speed;
        //Debug.Log($" Fallback: dir: {dir} // estimatedNormal: {estimatedNormal} // reflectedDir: {reflectedDir}");

    }

}
