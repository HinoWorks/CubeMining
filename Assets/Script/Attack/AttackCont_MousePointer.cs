using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class AttackCont_MousePointer : AttackContBase
{
    [SerializeField] GameObject obj_pointerArea;
    //[SerializeField] TriggerSender triggerSender;

    //private HashSet<IDamagable> targets = new HashSet<IDamagable>();
    //private readonly List<IDamagable> removeBuffer = new();

    private IDamagable currentTarget;
    private float initialSize;


    protected override void AwakeCall()
    {
        GameEvent.Input.PointerDamage.Subscribe(target => TargetDamage(target)).AddTo(this);
        GameEvent.Input.PointerMove.Subscribe(pos => PointerMove(pos)).AddTo(this);
    }
    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
        CreateAttackRoop();
        initialSize = obj_pointerArea.transform.localScale.x;
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
                obj_pointerArea.transform.DOScale(1.1f * initialSize * Vector3.one, 0.075f).SetEase(Ease.OutBack);
                obj_pointerArea.transform.DOScale(initialSize * Vector3.one, 0.075f).SetEase(Ease.OutBack).SetDelay(0.075f);

                if (currentTarget != null)
                {
                    if (currentTarget.Damage(damage))
                    {
                        currentTarget = null;
                    }
                }
                /*
                                foreach (var t in targets)
                                {
                                    if (!t.isAlive) continue;
                                    if (t.Damage(damage))
                                    {
                                        removeBuffer.Add(t);
                                    }
                                }
                                foreach (var t in removeBuffer) targets.Remove(t);
                */
            })
            .AddTo(this); // Destroy で自動終了
    }



    #region -- position fix --
    private void TargetDamage(IDamagable target)
    {
        if (target == null)
        {
            currentTarget = null;
        }
        else if (target != currentTarget)
        {
            currentTarget = target;
        }
    }

    private void PointerMove(Vector3 pos)
    {
        obj_pointerArea.transform.position = pos;
    }
    #endregion





}
