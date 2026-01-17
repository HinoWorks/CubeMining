using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;



public class AttackCont_LightningStrike : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<bulletCont_ThunderStrike> bullets = new List<bulletCont_ThunderStrike>();
    private Vector3 offsetPosition = new Vector3(0, 0.25f, 0); // 雷発生位置のオフセット



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
            var targetBlock = BlockGenerateManager.Inst.Get_RandomTargetBlock();
            if (targetBlock == null) continue;


            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<bulletCont_ThunderStrike>();
                bullets.Add(freeBullet);
            }
            freeBullet.transform.position = targetBlock.transform.position + offsetPosition;
            freeBullet.Init(damage, targetBlock);
        }
    }




}
