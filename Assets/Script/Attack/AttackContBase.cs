using UnityEngine;



public enum AttackUnitState
{
    Attacking,
    CT,

}

public class AttackContBase : MonoBehaviour
{
    protected AttackParam attackParam;


    protected AttackUnitState attackUnitState;
    protected float timer_attackInterval;
    protected float timer_aliveTime;
    protected float timer_ct;




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


    /*

        #region -- UnityUpdate --
        public void UnityUpdate()
        {
            if (!isActive) return;
            switch (attackParam.so.attackType)
            {
                case AttackType.Always:
                    UnityUpdate_Always();
                    break;
                case AttackType.Shot:
                    UnityUpdate_Shot();
                    break;
                case AttackType.AreaLoop:
                    UnityUpdate_AreaLoop();
                    break;
            }
        }


        /// <summary>
        /// 常時発生 => アタックインターバルの間隔で攻撃
        /// </summary>
        protected virtual void UnityUpdate_Always()
        {
            timer_attackInterval += Time.deltaTime;
            if (timer_attackInterval >= attackParam.attackInterval)
            {
                timer_attackInterval = 0f;
                AttackInterval();
            }
        }
        protected virtual void AttackInterval() { }



        protected virtual void UnityUpdate_Shot()
        {
            timer_ct += Time.deltaTime;
            if (timer_ct >= attackParam.ct)
            {
                timer_ct = 0f;
                Shot();
            }
        }
        protected virtual void Shot() { }



        protected virtual void UnityUpdate_AreaLoop()
        {
            switch (attackUnitState)
            {
                case AttackUnitState.Attacking:
                    timer_aliveTime += Time.deltaTime;
                    if (timer_aliveTime >= attackParam.aliveTime)
                    {
                        timer_aliveTime = 0f;
                        attackUnitState = AttackUnitState.CT;
                        AreaLoop_ToCT();
                    }
                    break;
                case AttackUnitState.CT:
                    timer_ct += Time.deltaTime;
                    if (timer_ct >= attackParam.ct)
                    {
                        timer_ct = 0f;
                        attackUnitState = AttackUnitState.Attacking;
                        AreaLoop_ToAttacking();
                    }
                    break;
            }
        }

        protected virtual void AreaLoop_ToCT() { }
        protected virtual void AreaLoop_ToAttacking() { }

        #endregion -- UnityUpdate --
    */





}
