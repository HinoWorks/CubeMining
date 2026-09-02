using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

public class AttackCont_Thunder : AttackContBase
{
    public enum ThunderType
    {
        Random, Near
    }

    [SerializeField] ThunderType thunderType;
    private MiningTarget_Cube targetBlock;
    private Vector3 effectPosition => new Vector3(targetBlock.transform.position.x, 0, targetBlock.transform.position.z);

    private float damageRate_chain = 0.5f;


    [Space]
    [Space(10)]
    [Header("Chain Settings")]
    [SerializeField] private float searchRadius = 5f;
    private int chainCount = 3;
    [SerializeField] LayerMask searchLayer;





    protected override void AwakeCall() { }
    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
        CreateAttackRoop();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void CreateAttackRoop()
    {
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
                    .Where(_ => base.isActive)
                    .Subscribe(_ =>
                    {
                        if (thunderType == ThunderType.Random)
                        {
                            CreateThunder_Random();
                        }
                        else if (thunderType == ThunderType.Near)
                        {
                            CreateThunder_Random();
                        }
                    })
                    .AddTo(this); // Destroy で自動終了
    }


    private async void CreateThunder_Random()
    {
        PlayAttackSound();
        for (int i = 0; i < base.count; i++)
        {
            targetBlock = BlockGenerateManager.Inst.Get_RandomTargetCube();
            if (targetBlock == null) break;
            ActivateBaseThunder(EffectType.ThunderStrike);

            if (base.exLevel >= 2)
            {
                ActivateChainThunder();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
    }


    /// <summary>
    /// 雷の基本攻撃、ターゲットブロックにダメージ
    /// </summary>
    private void ActivateBaseThunder(EffectType _effectType)
    {
        var effUnit = EffectManager.Inst.Get_Effect(_effectType);
        effUnit.transform.position = effectPosition;
        effUnit.SetActive(true);

        var damageFixed = (int)(this.damage);
        if (damageFixed < 1) damageFixed = 1;

        targetBlock.Damage(damage);
    }

    /// <summary>
    /// 雷の追加１、周囲をサーチしてchainダメージ
    /// </summary>
    private async void ActivateChainThunder()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        //SoundManager.Inst.PlaySE(201);

        var searchTargets = Physics.OverlapSphere(effectPosition, searchRadius, searchLayer);
        Array.Sort(searchTargets, (a, b) =>
        {
            var distA = (a.transform.position - effectPosition).sqrMagnitude;
            var distB = (b.transform.position - effectPosition).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        var counter = 0;
        foreach (var col in searchTargets)
        {
            if (counter >= chainCount) break;
            if (col == null) continue;
            if (!col.TryGetComponent<MiningTarget_Cube>(out var chainTarget)) continue;

            if (chainTarget == targetBlock) continue; // すでに直撃した本体は除外し
            ActivateChainEffect(effectPosition, chainTarget.transform.position, EffectType.ThunderStrike_Chain);
            chainTarget.Damage((int)(damage * damageRate_chain));
            counter++;
        }
    }

    private void ActivateChainEffect(Vector3 _positionBase, Vector3 _positionTarget, EffectType _effectType)
    {
        var eff_chain = EffectManager.Inst.Get_EffectCont(_effectType);
        eff_chain.transform.position = new Vector3(_positionTarget.x, 0.5f, _positionTarget.z);

        var direction = _positionBase - _positionTarget;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        var distance = direction.magnitude;
        direction /= distance;

        // prefab の初期回転(X=90)に合わせ、起点(ターゲット)から base 方向へ Y 軸で向ける
        var angleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        eff_chain.transform.rotation = Quaternion.Euler(90f, angleY, 0f);

        eff_chain.SetParticle3DSize(0.5f, distance / 2f, 1f);

        eff_chain.gameObject.SetActive(true);
    }


    /*
        #region thunderType = random
        private void CreateBullet_Random()
        {
            targetBlock = BlockGenerateManager.Inst.Get_RandomTargetCube();
            if (targetBlock == null) return;

            SetThunder_Random_Level1();

            if (base.exLevel < 2) return;
            SetThunder_Random_Level2();
        }

        private void SetThunder_Random_Level1()
        {
            var effUnit = EffectManager.Inst.Get_Effect(effectType_red);
            effUnit.transform.position = effectPosition;
            effUnit.SetActive(true);

            ActivateThunderBullet(effectPosition);
        }

        private async void SetThunder_Random_Level2()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));


        }
        #endregion
        */



    /*
        // == Common ==
        private void ActivateThunderBullet(Vector3 _position, float _damageRate = 1f)
        {
            var freeBullet = GetBulletCont();
            freeBullet.transform.position = _position;
            var damageFixed = (int)(this.damage * _damageRate);
            if (damageFixed < 1) damageFixed = 1;
            freeBullet.Init(damageFixed, 0.1f, velocity);
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
        */

}
