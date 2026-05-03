using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

public class AttackCont_Thunder : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletCont_ThunderStrike> bullets = new List<BulletCont_ThunderStrike>();
    private MiningTarget_Cube targetBlock;
    private Vector3 velocity = new Vector3(0, -0.1f, 0);
    private EffectType effectType_base = EffectType.ThunderStrike;
    private EffectType effectType_cross = EffectType.ThunderStrike_Cross;
    private Vector3[] offsetPosition_cross = new Vector3[4]
        {
            new Vector3(1, 0, 0), new Vector3(-1, 0 , 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1)
        };

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
        targetBlock = BlockGenerateManager.Inst.Get_TopTarget();
        if (targetBlock == null) return;

        SetThunder_Level1();
        SetThunder_Level2();
    }

    /// <summary>
    /// 雷の基本攻撃、ターゲットブロックにダメージ
    /// </summary>
    private void SetThunder_Level1()
    {
        //SoundManager.Inst.PlaySE(201);
        // 基本のエフェクトを再生
        var effUnit = EffectManager.Inst.Get_Effect(effectType_base);
        effUnit.transform.position = targetBlock.transform.position;
        effUnit.SetActive(true);

        var freeBullet = GetBulletCont();
        freeBullet.transform.position = targetBlock.transform.position;
        freeBullet.Init(damage, 0.1f, velocity);
    }

    /// <summary>
    /// 雷の追加１、周囲十時エリアにダメージ
    /// </summary>
    private async void SetThunder_Level2()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        //SoundManager.Inst.PlaySE(201);

        // クロス用のエフェクト再生
        var effUnit = EffectManager.Inst.Get_Effect(effectType_cross);
        effUnit.transform.position = targetBlock.transform.position;
        effUnit.SetActive(true);

        // クロス用の弾を生成
        for (int i = 0; i < offsetPosition_cross.Length; i++)
        {
            var freeBullet = GetBulletCont();
            freeBullet.transform.position = targetBlock.transform.position + offsetPosition_cross[i];
            freeBullet.Init(damage, 0.1f, velocity);
        }
    }




    private BulletCont_ThunderStrike GetBulletCont()
    {
        var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
        if (freeBullet == null)
        {
            var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
            freeBullet = newBullet.GetComponent<BulletCont_ThunderStrike>();
            bullets.Add(freeBullet);
        }
        return freeBullet;
    }

}
