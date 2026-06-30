using UnityEngine;
using UniRx;
using System;


/// <summary>
/// ボーナスエリアを生成するスキル
/// 生成されたボーナスエリアは、インゲーム中常に存在する
/// </summary>
public class SubSkillCont_BonusArea : SubSkillCont_Base
{
    private float bonusRate => param.rate; // リソース増加量
    private float sizeRate => param.size; //

    [SerializeField] GameObject pf_bullet;
    private SubSkillCont_BonusArea_Bullet activeBullet;
    private Vector3 randomPosition => new Vector3(UnityEngine.Random.Range(-10f, 10f),
                                                0f, UnityEngine.Random.Range(-5f, 5f));




    protected override void AwakeCall()
    {
        base.AwakeCall();
    }

    public override void Set_AttackTrigger(bool isTrigger)
    {
        isActive = isTrigger;
        if (isTrigger)
        {
            CreateBullet();
        }
    }

    private void CreateBullet()
    {
        if (activeBullet != null)
        {
            Destroy(activeBullet.gameObject);
        }

        var newBullet = Instantiate(pf_bullet, InGameManager.Inst.ParentPool) as GameObject;
        activeBullet = newBullet.GetComponent<SubSkillCont_BonusArea_Bullet>();
        activeBullet.transform.position = randomPosition;
        activeBullet.Init(bonusRate, sizeRate);
    }

    public override void OnDestroy()
    {
        if (activeBullet != null)
        {
            Destroy(activeBullet.gameObject);
            activeBullet = null;
        }
        base.OnDestroy();
    }


}
