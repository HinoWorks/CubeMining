using UnityEngine;



public enum AttackUnitState
{
    Attacking,
    CT,

}

public class AttackContBase : MonoBehaviour
{
    protected AttackParam attackParam;

    protected int damage => (int)attackParam.damage;
    protected float attackInterval => attackParam.attackInterval;
    protected float speed => attackParam.speed;
    protected float aliveTime => attackParam.aliveTime;
    protected int count => attackParam.count;
    protected float size => attackParam.size;


    void Awake()
    {
        AwakeCall();
    }
    protected virtual void AwakeCall() { } //一度だけ呼ばれる

    public virtual void Init(AttackParam _attackParam)
    {
        attackParam = _attackParam;
        isActive = false;
    }

    public virtual void Set_AttackTrigger(bool isTrigger)
    {
        isActive = isTrigger;
    }

    public virtual void OnDestroy()
    {
        Destroy(this.gameObject);
    }
    protected virtual bool isActive { get; set; } = false; //　Init後、攻撃開始タイミング同期用。trueになったら攻撃開始


}
