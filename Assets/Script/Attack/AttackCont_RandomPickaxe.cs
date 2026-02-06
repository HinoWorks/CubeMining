using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;

public class AttackCont_RandomPickaxe : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletCont_RandomPickaxe> bullets = new List<BulletCont_RandomPickaxe>();
    private Vector3 offsetPosition = new Vector3(0, 2.5f, 0); // 発射位置オフセット


    protected override void AwakeCall() { }
    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
        CreateAttackRoop();
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

    private void CreateAttackRoop()
    {
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
                    .Where(_ => base.isActive)
                    .Subscribe(_ =>
                    {
                        CreateBullet();
                    })
                    .AddTo(this); // Destroy で自動終了
    }
    private void CreateBullet()
    {
        for (int i = 0; i < count; i++)
        {
            var targetBlock = BlockGenerateManager.Inst.Get_RandomTargetArea();
            if (targetBlock == null) continue;

            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<BulletCont_RandomPickaxe>();
                bullets.Add(freeBullet);
            }
            freeBullet.transform.position = targetBlock + offsetPosition;
            freeBullet.Init(damage, aliveTime, Vector3.zero);
        }
    }


}
