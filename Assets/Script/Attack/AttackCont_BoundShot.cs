using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;


public class AttackCont_BoundShot : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletBase> bullets = new List<BulletBase>();

    // loc
    private Vector3 pointerPosition;
    private Vector3 offsetPosition = new Vector3(0, 0.35f, 0);




    protected override void AwakeCall()
    {
        //GameEvent.Input.PointerAreaIn.Subscribe(isAreaIn => PointerAreaIn(isAreaIn)).AddTo(this);
        GameEvent.Input.PointerMove.Subscribe(pos => PointerMove(pos)).AddTo(this);
    }
    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
    }

    public override void OnDestroy()
    {
        foreach (var bullet in bullets)
        {
            bullet.OnDestroy();
        }
        bullets.Clear();
        base.OnDestroy();
    }

    public override void Set_AttackTrigger(bool isTrigger)
    {
        base.Set_AttackTrigger(isTrigger);

        if (!base.isActive) return;
        CreateBullet();
    }

    private void CreateBullet()
    {
        var randomAngle = UnityEngine.Random.Range(0f, 360f);
        for (int i = 0; i < count; i++)
        {
            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<BulletBase>();
                bullets.Add(freeBullet);
            }
            var direction = new Vector3(Mathf.Cos((randomAngle) * Mathf.Deg2Rad), 0,
                                        Mathf.Sin((randomAngle) * Mathf.Deg2Rad));
            freeBullet.transform.position = transform.position;
            freeBullet.Init(damage, aliveTime, direction * speed);
            freeBullet.SetBulletType(BulletType.Piercing);
        }
    }


    #region -- position fix --
    private void PointerMove(Vector3 pos)
    {
        pointerPosition = pos + offsetPosition;
        transform.position = pointerPosition;
    }
    #endregion
}
