using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Inst;
    [SerializeField] private AttackContBase[] attackConts;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }

        foreach (var attackCont in attackConts)
        {
            attackCont.AwakeCall();
        }
    }


    public void Set_Ready()
    {
        foreach (var attackCont in attackConts)
        {
            attackCont.Init();
        }
    }

    public void Set_AttackState(bool isStart)
    {
        // 攻撃開始
        foreach (var attackCont in attackConts)
        {
            attackCont.Set_AttackTrigger(isStart);
        }
    }




}
