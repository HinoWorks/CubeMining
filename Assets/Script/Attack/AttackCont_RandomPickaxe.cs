using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public class AttackCont_RandomPickaxe : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletCont_IceBlock> bullets = new List<BulletCont_IceBlock>();
    private Vector3 offsetPosition = new Vector3(0, 5.5f, 0); // 発射位置オフセット
    private float createDelay = 0.1f;
    private CancellationTokenSource CTS;

    [Space(5)]
    [Header("Level2 checkRate")]
    [SerializeField] private float changeRate_level2 = 0.5f; // レベル2になる確率
    private bool isLevel2 => UnityEngine.Random.Range(0f, 1f) < changeRate_level2;


    protected override void AwakeCall() { }
    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
        CTS = new CancellationTokenSource();
        CreateAttackRoop();
    }

    public override void OnDestroy()
    {
        foreach (var bullet in bullets)
        {
            bullet.OnDestroy();
        }
        bullets.Clear();
        CTS.Cancel();
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
    private async void CreateBullet()
    {
        SoundManager.Inst.PlaySE(201);
        for (int i = 0; i < count; i++)
        {
            if (CTS.IsCancellationRequested) return;
            var targetBlock = BlockGenerateManager.Inst.Get_RandomTargetArea();
            if (targetBlock == null) continue;

            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<BulletCont_IceBlock>();
                bullets.Add(freeBullet);
            }
            freeBullet.transform.position = targetBlock + offsetPosition;

            var selectLevel = 1;
            if (base.exLevel >= 2)
            {
                selectLevel = isLevel2 ? 2 : 1;
            }
            freeBullet.Init_IceBlock(CalculateDamage(), Vector3.zero, selectLevel);
            await UniTask.Delay(TimeSpan.FromSeconds(createDelay), cancellationToken: CTS.Token);
        }
    }


}
