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
    private Vector3 offsetPosition = new Vector3(0, 7.5f, 0); // 発射位置オフセット
    private Vector3 initialSpeed = new Vector3(0, -2f, 0);
    private float createDelay = 0.1f;
    private CancellationTokenSource CTS;

    [Space(5)]
    [Header("Level2 checkRate")]
    [SerializeField] private float changeRate_level2 = 0.5f; // レベル2になる確率
    private bool isLevel2 => UnityEngine.Random.Range(0f, 1f) < changeRate_level2;


    [Space(10)]
    [Header("Ice circle")]
    [SerializeField] GameObject obj_IceCircle;
    private List<BulletCont_IceCircle> bullets_IceCircle = new List<BulletCont_IceCircle>();

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

            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<BulletCont_IceBlock>();
                bullets.Add(freeBullet);
            }
            var targetPosition = BlockGenerateManager.Inst.Get_RandomTargetPoint();
            freeBullet.transform.position = new Vector3(targetPosition.x, 0, targetPosition.z) + offsetPosition;

            var selectLevel = 1;
            if (base.exLevel >= 2)
            {
                selectLevel = isLevel2 ? 2 : 1;
            }
            freeBullet.Init_IceBlock(CalculateDamage(), initialSpeed, selectLevel, CreateIceCircle);
            await UniTask.Delay(TimeSpan.FromSeconds(createDelay), cancellationToken: CTS.Token);
        }
    }

    private void CreateIceCircle(Vector3 _position, int _level)
    {
        var freeBullet = bullets_IceCircle.Find(x => !x.gameObject.activeSelf);
        if (freeBullet == null)
        {
            var newBullet = Instantiate(obj_IceCircle, InGameManager.Inst.ParentPool) as GameObject;
            freeBullet = newBullet.GetComponent<BulletCont_IceCircle>();
            bullets_IceCircle.Add(freeBullet);
        }
        freeBullet.transform.position = _position;
        freeBullet.Init(CalculateDamage(), 1f, _level);
    }


}
