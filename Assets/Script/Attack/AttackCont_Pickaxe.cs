using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class AttackCont_Pickaxe : MonoBehaviour
{
    public int slotIndex { get; private set; } = 0;
    protected PickaxeParam pickaxeParam;

    protected bool isSelectPickaxe = false;
    protected bool isActive = false;//　Init後、攻撃開始タイミング同期用。trueになったら攻撃開始

    protected int damage => (int)pickaxeParam.damage;
    protected float attackInterval => pickaxeParam.attackInterval;
    protected float criticalRate => pickaxeParam.criticalRate;
    protected float resourceRate => pickaxeParam.resourceRate;
    protected float size => pickaxeParam.size;


    [SerializeField] GameObject obj_pointerArea;
    [SerializeField] TriggerSender[] triggerSender;

    private HashSet<IDamagable> targets = new HashSet<IDamagable>();
    private readonly List<IDamagable> removeBuffer = new();

    private Vector3 offsetPosition = new Vector3(0, 0.1f, 0);


    protected void Awake()
    {
        GameEvent.Input.PointerAreaIn.Subscribe(isAreaIn => PointerAreaIn(isAreaIn)).AddTo(this);
        GameEvent.Input.PointerMove.Subscribe(pos => PointerMove(pos)).AddTo(this);
        foreach (var sender in triggerSender)
        {
            sender.OnEnter += OnEnter;
            sender.OnExit += OnExit;
        }
    }
    public void Init(int _slotIndex, PickaxeParam _pickaxeParam)
    {
        slotIndex = _slotIndex;
        pickaxeParam = _pickaxeParam;
        targets.Clear();
        CreateAttackRoop();
        this.gameObject.SetActive(false);
    }

    public virtual void Set_SelectPickaxe(int _activeSlotIndex)
    {
        isSelectPickaxe = slotIndex == _activeSlotIndex;
        this.gameObject.SetActive(isSelectPickaxe);
    }
    public virtual void Set_AttackTrigger(bool isTrigger)
    {
        isActive = isTrigger;
    }

    public void OnDestroy()
    {
        foreach (var sender in triggerSender)
        {
            sender.OnEnter -= OnEnter;
            sender.OnExit -= OnExit;
        }
        Destroy(this.gameObject);
    }

    private void CreateAttackRoop()
    {
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .Where(_ => isActive && isSelectPickaxe)
            .Subscribe(_ =>
            {
                removeBuffer.Clear();
                obj_pointerArea.transform.DOScale(1.1f * size * Vector3.one, 0.075f).SetEase(Ease.OutBack);
                obj_pointerArea.transform.DOScale(size * Vector3.one, 0.075f).SetEase(Ease.OutBack).SetDelay(0.075f);

                foreach (var t in targets)
                {
                    if (!t.isAlive) continue;
                    if (t.Damage(damage))
                    {
                        removeBuffer.Add(t);
                    }
                }
                foreach (var t in removeBuffer) targets.Remove(t);
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
