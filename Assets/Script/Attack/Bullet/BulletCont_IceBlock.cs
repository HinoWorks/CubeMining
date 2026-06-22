using System;
using System.Runtime.CompilerServices;
using UnityEngine;
public class BulletCont_IceBlock : BulletBase
{
    [SerializeField] GameObject obj_Ice_1;
    [SerializeField] Gradient gradient_Ice_1;
    [SerializeField] GameObject obj_Ice_2;
    [SerializeField] Gradient gradient_Ice_2;
    private TrailRenderer trailRenderer;

    private int colCount = 0;
    private int attackableCount = 1;
    private int attackableCount_level2 = 3;

    private int level = 1;
    private Action<Vector3, int> onGenerateIceCircle;



    public void Init_IceBlock(int _damage, Vector3 _direction, int _setLevel, Action<Vector3, int> _onGenerateIceCircle)
    {
        onGenerateIceCircle = _onGenerateIceCircle;
        level = _setLevel;
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        trailRenderer.Clear();

        trailRenderer.colorGradient = _setLevel == 1 ? gradient_Ice_1 : gradient_Ice_2;
        obj_Ice_1.SetActive(_setLevel == 1);
        obj_Ice_2.SetActive(_setLevel == 2);
        colCount = 0;
        attackableCount = _setLevel == 1 ? 1 : attackableCount_level2;

        base.Init(_damage, 25, _direction);
    }

    public override void ReturnToPool()
    {
        base.ReturnToPool();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StaticManager.tag_WorldBase))
        {
            GenerateIceCircle();
        }

        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
            ColCheck();
        }
    }

    private void GenerateIceCircle()
    {
        onGenerateIceCircle?.Invoke(transform.position, level);
        base.ReturnToPool();
    }

    private void ColCheck()
    {
        colCount++;
        if (colCount >= attackableCount)
        {
            GenerateIceCircle();
        }
    }


    /*
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
    */


}
