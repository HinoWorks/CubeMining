using UnityEngine;

public class BulletCont_BoundShot : BulletBase
{
    [SerializeField] GameObject obj_level1;
    [SerializeField] GameObject obj_level2;


    private TrailRenderer trailRenderer;
    private const float ViewportMargin = 0.02f;





    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        /*
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
        */

        base.SetBulletType(BulletType.Piercing);
        base.Init(_damage, _lifetime, _direction);
    }

    public void SetLevelUnit_Level2(bool _isLevel2)
    {
        obj_level1.SetActive(!_isLevel2);
        obj_level2.SetActive(_isLevel2);
    }


    public override void ReturnToPool()
    {
        base.ReturnToPool();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    void FixedUpdate()
    {
        if (!gameObject.activeSelf || rb == null) return;
        ReflectIfOutOfCamera();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
            if (bulletType == BulletType.Piercing) return;
            base.ReturnToPool();
        }
    }




    void ReflectIfOutOfCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;

        var viewport = cam.WorldToViewportPoint(transform.position);
        var velocity = rb.linearVelocity;
        var reflected = false;

        var camRight = GetFlattenedDirection(cam.transform.right);
        var camForward = GetFlattenedDirection(cam.transform.forward);

        if (viewport.x < ViewportMargin)
        {
            velocity = ReflectOnPlane(velocity, camRight);
            reflected = true;
        }
        else if (viewport.x > 1f - ViewportMargin)
        {
            velocity = ReflectOnPlane(velocity, camRight);
            reflected = true;
        }

        if (viewport.y < ViewportMargin)
        {
            velocity = ReflectOnPlane(velocity, camForward);
            reflected = true;
        }
        else if (viewport.y > 1f - ViewportMargin)
        {
            velocity = ReflectOnPlane(velocity, camForward);
            reflected = true;
        }

        if (!reflected) return;

        rb.linearVelocity = velocity;
        ClampPositionInViewport(cam, viewport);
    }

    static Vector3 GetFlattenedDirection(Vector3 direction)
    {
        direction.y = 0f;
        return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.zero;
    }

    static Vector3 ReflectOnPlane(Vector3 velocity, Vector3 axis)
    {
        if (axis.sqrMagnitude < 0.001f) return velocity;
        var projected = Vector3.Dot(velocity, axis) * axis;
        return velocity - 2f * projected;
    }

    void ClampPositionInViewport(Camera cam, Vector3 viewport)
    {
        var clampedViewport = viewport;
        clampedViewport.x = Mathf.Clamp(viewport.x, ViewportMargin, 1f - ViewportMargin);
        clampedViewport.y = Mathf.Clamp(viewport.y, ViewportMargin, 1f - ViewportMargin);

        var worldPos = cam.ViewportToWorldPoint(clampedViewport);
        transform.position = new Vector3(worldPos.x, transform.position.y, worldPos.z);
    }
}
