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

    void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var attackCont in attackConts)
        {
            attackCont.Init();
        }

        // 攻撃開始
        foreach (var attackCont in attackConts)
        {
            attackCont.Set_AttackTrigger(true);
        }
    }




}
