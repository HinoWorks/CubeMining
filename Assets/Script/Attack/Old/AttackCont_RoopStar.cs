using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;

public class AttackCont_RoopStar : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform rotateTarget;
    private List<BulletBase> bullets = new List<BulletBase>();

    // loc
    private Vector3 pointerPosition;
    private Vector3 offsetPosition = new Vector3(0, 0.35f, 0);
    private float redius = 1.5f;




    protected override void AwakeCall()
    {
        //GameEvent.Input.PointerAreaIn.Subscribe(isAreaIn => PointerAreaIn(isAreaIn)).AddTo(this);
        GameEvent.Input.PointerMove.Subscribe(pos => PointerMove(pos)).AddTo(this);
    }
    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
    }


    public override void Set_AttackTrigger(bool isTrigger)
    {
        base.Set_AttackTrigger(isTrigger);

        if (isTrigger)
        {
            CreateBullet();
        }
        else
        {
            foreach (var bullet in bullets)
            {
                bullet.OnDestroy();
            }
            bullets.Clear();
        }
    }

    private void CreateBullet()
    {
        var deltaAngle = 360f / count * Mathf.Deg2Rad;
        for (int i = 0; i < count; i++)
        {
            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, rotateTarget) as GameObject;
                freeBullet = newBullet.GetComponent<BulletBase>();
                bullets.Add(freeBullet);
            }

            freeBullet.transform.localPosition = new Vector3(Mathf.Cos(i * deltaAngle), 0f, Mathf.Sin(i * deltaAngle)) * redius;
            freeBullet.Init(damage, aliveTime, Vector3.zero);
            freeBullet.SetBulletType(BulletType.Piercing);
        }
    }

    void Update()
    {
        if (!isActive) return;
        rotateTarget.Rotate(Vector3.up, speed * Time.deltaTime);
    }

    #region -- position fix --
    private void PointerMove(Vector3 pos)
    {
        pointerPosition = pos + offsetPosition;
        transform.position = pointerPosition;
    }
    #endregion


}
