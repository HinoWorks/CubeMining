using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

public class AttackCont_Thunder : AttackContBase
{
    public enum ThunderType
    {
        Top, Random
    }

    [SerializeField] ThunderType thunderType;
    [SerializeField] GameObject bulletPrefab;
    private List<BulletCont_ThunderStrike> bullets = new List<BulletCont_ThunderStrike>();
    private MiningTarget_Cube targetBlock;
    private Vector3 velocity = new Vector3(0, -0.1f, 0);


    // thunderType = top
    private EffectType effectType_base = EffectType.ThunderStrike;
    private EffectType effectType_cross = EffectType.ThunderStrike_Cross;
    private Vector3[] offsetPosition_cross = new Vector3[4]
            {
                new Vector3(1, 0, 0), new Vector3(-1, 0 , 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1)
            };

    // thunderType = random
    private EffectType effectType_red = EffectType.ThunderStrike_Red;
    private EffectType effectType_red_circle = EffectType.ThunderStrike_Red_Circle;
    private Vector3[] offsetPosition_red_circle = new Vector3[8]
               {
                    new Vector3(1, 0, 0), new Vector3(-1, 0 , 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1),
                    new Vector3(1, 0, 1), new Vector3(-1, 0 , -1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1)
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
                        if (thunderType == ThunderType.Top)
                        {
                            CreateBullet_Top();
                        }
                        else if (thunderType == ThunderType.Random)
                        {
                            CreateBullet_Random();
                        }
                    })
                    .AddTo(this); // Destroy で自動終了
    }



    #region thunderType = top
    private void CreateBullet_Top()
    {
        targetBlock = BlockGenerateManager.Inst.Get_TopTarget();
        if (targetBlock == null) return;

        SetThunder_Top_Level1();
        SetThunder_Top_Level2();
    }

    /// <summary>
    /// 雷の基本攻撃、ターゲットブロックにダメージ
    /// </summary>
    private void SetThunder_Top_Level1()
    {
        //SoundManager.Inst.PlaySE(201);
        // 基本のエフェクトを再生
        var effUnit = EffectManager.Inst.Get_Effect(effectType_base);
        effUnit.transform.position = targetBlock.transform.position;
        effUnit.SetActive(true);

        ActivateThunderBullet(targetBlock.transform.position);
    }

    /// <summary>
    /// 雷の追加１、周囲十時エリアにダメージ
    /// </summary>
    private async void SetThunder_Top_Level2()
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
            ActivateThunderBullet(targetBlock.transform.position + offsetPosition_cross[i]);
        }
    }
    #endregion




    #region thunderType = random
    private void CreateBullet_Random()
    {
        targetBlock = BlockGenerateManager.Inst.Get_TopTarget();
        if (targetBlock == null) return;

        SetThunder_Random_Level1();
        SetThunder_Random_Level2();
    }

    private void SetThunder_Random_Level1()
    {
        var effUnit = EffectManager.Inst.Get_Effect(effectType_red);
        effUnit.transform.position = targetBlock.transform.position;
        effUnit.SetActive(true);

        ActivateThunderBullet(targetBlock.transform.position);
    }

    private async void SetThunder_Random_Level2()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        var effUnit = EffectManager.Inst.Get_Effect(effectType_red_circle);
        effUnit.transform.position = targetBlock.transform.position;
        effUnit.SetActive(true);

        for (int i = 0; i < offsetPosition_red_circle.Length; i++)
        {
            ActivateThunderBullet(targetBlock.transform.position + offsetPosition_red_circle[i]);
        }
    }

    #endregion



    // == Common ==
    private void ActivateThunderBullet(Vector3 _position)
    {
        var freeBullet = GetBulletCont();
        freeBullet.transform.position = _position;
        freeBullet.Init(damage, 0.1f, velocity);
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
