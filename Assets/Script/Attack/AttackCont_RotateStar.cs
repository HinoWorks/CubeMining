using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;


public class AttackCont_RotateStar : AttackContBase
{
    [SerializeField] GameObject rotateStarUnit;
    private List<AttackCont_RotateStarUnit> rotateStarUnits = new List<AttackCont_RotateStarUnit>();
    private Vector3 pointerPosition;

    private float rate_verticalRotate = 0.3f;
    private bool isVerticalRotate => UnityEngine.Random.Range(0f, 1f) < rate_verticalRotate;




    protected override void AwakeCall()
    {
        GameEvent.Input.PointerMove.Subscribe(pos => Set_PointerPosition(pos)).AddTo(this);
    }
    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
        pointerPosition = transform.position;
        CreateAttackRoop();
    }

    public override void OnDestroy()
    {
        foreach (var cont in rotateStarUnits)
        {
            cont.OnDestroy();
        }
        rotateStarUnits.Clear();
        base.OnDestroy();
    }

    private void Set_PointerPosition(Vector3 pos)
    {
        pointerPosition = pos;
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
        // -- level1
        var freeRotateStarUnit = Get_FreeRotateStarUnit();
        freeRotateStarUnit.transform.position = pointerPosition;
        freeRotateStarUnit.Init(base.damage, base.aliveTime, base.speed, base.count);

        // -- level2 check
        if (base.exLevel < 2) return;
        if (!isVerticalRotate) return;

        var freeRotateStarUnit_vertical = Get_FreeRotateStarUnit();
        freeRotateStarUnit_vertical.transform.position = pointerPosition;
        freeRotateStarUnit_vertical.Init_Vertical(base.damage, base.aliveTime, base.speed, 2);
    }

    private AttackCont_RotateStarUnit Get_FreeRotateStarUnit()
    {
        var freeRotateStarUnit = rotateStarUnits.Find(x => !x.gameObject.activeSelf);
        if (freeRotateStarUnit == null)
        {
            var newRotateStarUnit = Instantiate(rotateStarUnit, InGameManager.Inst.ParentPool) as GameObject;
            freeRotateStarUnit = newRotateStarUnit.GetComponent<AttackCont_RotateStarUnit>();
            rotateStarUnits.Add(freeRotateStarUnit);
        }
        return freeRotateStarUnit;
    }

}
