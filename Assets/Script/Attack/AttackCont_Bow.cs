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
    private int deltaLayer = 3;

    private Vector3 offsetPosition_vertical = new Vector3(0, 3f, 0); // 発射位置オフセット
    private float offsetPosition_horizontal = 3.5f; // 発射位置オフセット
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
            setPosition = targetBlock.transform.position + offsetPosition_vertical;
            shotDirection = (targetBlock.transform.position - setPosition).normalized;
            freeBowUnit.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.forward);
        }
        else
        {
            return;
            /*
            var (isShotLine_z, targetPosition) = BlockGenerateManager.Inst.Get_RandomTargetBlock();
            setPosition = targetPosition + (isShotLine_z ?
                new Vector3(0, 0, -offsetPosition_horizontal) : new Vector3(offsetPosition_horizontal, 0, 0));
            shotDirection = (targetPosition - setPosition).normalized;
            freeBowUnit.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.up);
        */
        }
        freeBowUnit.transform.position = setPosition;
        freeBowUnit.Init(damage, aliveTime, speed, shotDirection, IsAddArrow);
    }
}
