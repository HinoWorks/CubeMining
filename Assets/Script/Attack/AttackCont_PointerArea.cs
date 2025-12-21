using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;


public class AttackCont_PointerArea : AttackContBase
{
    [SerializeField] GameObject obj_pointerArea;
    [SerializeField] TriggerSender triggerSender;



    private HashSet<IDamagable> targets = new HashSet<IDamagable>();

    // loc
    private int damage = 1;
    private float attackInterval = 0.5f;
    private Vector3 offsetPosition = new Vector3(0, 0.1f, 0);




    public override void AwakeCall()
    {
        GameEvent.Input.PointerAreaIn.Subscribe(isAreaIn => PointerAreaIn(isAreaIn)).AddTo(this);
        GameEvent.Input.PointerMove.Subscribe(pos => PointerMove(pos)).AddTo(this);
        triggerSender.OnEnter += OnEnter;
        triggerSender.OnExit += OnExit;
    }
    public override void Init()
    {
        base.Init();
        targets.Clear();
        CreateAttackRoop();
    }

    public override void OnDestroy()
    {
        triggerSender.OnEnter -= OnEnter;
        triggerSender.OnExit -= OnExit;

        base.OnDestroy();
    }

    private void CreateAttackRoop()
    {
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .Where(_ => base.isActive)
            .Subscribe(_ =>
            {
                foreach (var t in targets)
                {
                    t.Damage(damage);
                }
            })
            .AddTo(this); // Destroy で自動終了
    }



    #region -- position fix --
    private void PointerAreaIn(bool isAreaIn)
    {
        if (isAreaIn == obj_pointerArea.activeSelf) return;
        obj_pointerArea.SetActive(isAreaIn);
    }

    private void PointerMove(Vector3 pos)
    {
        obj_pointerArea.transform.position = pos + offsetPosition;
    }
    #endregion


    #region -- target fix --

    private void OnEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            targets.Add(target);
        }
    }

    private void OnExit(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            targets.Remove(target);
        }
    }

    #endregion
}
