using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;

public class AttackCont_Thunder : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletCont_ThunderStrike> bullets = new List<BulletCont_ThunderStrike>();
    //private Vector3 offsetPosition = new Vector3(0, 2.5f, 0); // 雷発生位置のオフセット



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
        // Init() 内で即ダメージが入るため、先にターゲットだけ集めてから弾を出す
        var targets = new List<MiningTarget_Cube>();
        for (int i = 0; i < count; i++)
        {
            var targetBlock = BlockGenerateManager.Inst.Get_TopTarget();
            if (targetBlock == null) break;
            targets.Add(targetBlock);
        }
        foreach (var targetBlock in targets)
        {
            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<BulletCont_ThunderStrike>();
                bullets.Add(freeBullet);
            }
            freeBullet.transform.position = targetBlock.transform.position;
            freeBullet.Init(damage, targetBlock);
        }
    }

}
