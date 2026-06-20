using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;


public class AttackCont_RotateStar : AttackContBase
{
    [SerializeField] GameObject rotateStarUnit;
    [SerializeField] Vector3 offsetPosition = new Vector3(0, 0.5f, 0);
    private List<AttackCont_RotateStarUnit> rotateStarUnits = new List<AttackCont_RotateStarUnit>();
    private Vector3 pointerPosition;



    [Space(5)]
    [Header("Level2 checkRate")]
    [SerializeField] private float rate_Level2Unit = 0.3f;
    private bool isLevel2Unit => UnityEngine.Random.Range(0f, 1f) < rate_Level2Unit;

    private float size_Level2Unit = 1.5f;
    private float size_NormalUnit = 1f;




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
        if (base.exLevel >= 2 && isLevel2Unit)
        {
            // -- level2
            var freeRotateStarUnit = Get_FreeRotateStarUnit();
            freeRotateStarUnit.transform.position = pointerPosition + offsetPosition;
            freeRotateStarUnit.Init(base.damage, base.aliveTime, base.speed, base.count, size_Level2Unit);
        }
        else
        {
            // -- level1
            var freeRotateStarUnit = Get_FreeRotateStarUnit();
            freeRotateStarUnit.transform.position = pointerPosition + offsetPosition;
            freeRotateStarUnit.Init(base.damage, base.aliveTime, base.speed, base.count, size_NormalUnit);
        }

        /*
        // -- level2 vertical action
        if (base.exLevel < 2) return;
        if (!isLevel2Unit) return;

        var freeRotateStarUnit_vertical = Get_FreeRotateStarUnit();
        freeRotateStarUnit_vertical.transform.position = pointerPosition + offsetPosition;
        freeRotateStarUnit_vertical.Init_Level2(base.damage, base.aliveTime, base.speed, 2);
        */
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
