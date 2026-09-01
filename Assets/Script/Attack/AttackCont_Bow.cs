using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;

public class AttackCont_Bow : AttackContBase
{
    [SerializeField] GameObject bowUnit;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] bool isVertical = false;
    private List<BulletCont_Bow> bullets = new List<BulletCont_Bow>();
    private List<AttackCont_BowUnit> bowUnits = new List<AttackCont_BowUnit>();


    // bow position
    private float offsetPosition_tate = 9f;
    private float offsetPosition_yoko = 11f;
    private float offsetPosition_y = 0.75f;

    public int ExLevel => base.exLevel;

    [Space(5)]
    [Header("Level2 checkRate")]
    [SerializeField] float rate_addArrow = 0.5f;
    private bool IsAddArrow => ExLevel >= 2 && UnityEngine.Random.Range(0f, 1f) < rate_addArrow;
    public bool IsVertical => isVertical;



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

    public BulletCont_Bow Get_FreeArrow()
    {
        var freeArrow = bullets.Find(x => !x.gameObject.activeSelf);
        if (freeArrow == null)
        {
            var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
            freeArrow = newBullet.GetComponent<BulletCont_Bow>();
        }
        return freeArrow;
    }

    private void CreateBullet()
    {
        PlayAttackSound();
        var freeBowUnit = bowUnits.Find(x => !x.gameObject.activeSelf);
        if (freeBowUnit == null)
        {
            var newBowUnit = Instantiate(bowUnit, InGameManager.Inst.ParentPool) as GameObject;
            freeBowUnit = newBowUnit.GetComponent<AttackCont_BowUnit>();
            bowUnits.Add(freeBowUnit);
        }
        freeBowUnit.Set_BowCont(this);

        //var freeArrow = Get_FreeArrow();
        var setPosition = Vector3.zero;
        var shotDirection = Vector3.forward;
        if (isVertical)
        {
            var targetBlock = BlockGenerateManager.Inst.Get_RandomTargetBlock();
            if (targetBlock == null) return;

            var isTargetTop = targetBlock.transform.position.z > 0f;
            setPosition = new Vector3(targetBlock.transform.position.x, offsetPosition_y, isTargetTop ? -offsetPosition_tate : offsetPosition_tate);
            shotDirection = (targetBlock.transform.position - setPosition).normalized;
            shotDirection.y = 0f;
            freeBowUnit.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.forward);
        }
        else
        {
            var targetBlock = BlockGenerateManager.Inst.Get_RandomTargetBlock();
            if (targetBlock == null) return;

            var isTargetRight = targetBlock.transform.position.x > 0f;
            setPosition = new Vector3(isTargetRight ? -offsetPosition_yoko : offsetPosition_yoko, offsetPosition_y, targetBlock.transform.position.z);
            shotDirection = (targetBlock.transform.position - setPosition).normalized;
            shotDirection.y = 0f;
            freeBowUnit.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.forward);
        }
        freeBowUnit.transform.position = setPosition;
        freeBowUnit.Init(damage, aliveTime, speed, shotDirection, IsAddArrow);
    }
}
