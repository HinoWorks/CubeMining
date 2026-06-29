using UnityEngine;


public enum SubUnitState
{
    Active,
    CT
}



public class SubSkillCont_Base : MonoBehaviour
{
    protected SubSkillParam param;
    protected virtual bool isActive { get; set; } = false; //　Init後、攻撃開始タイミング同期用。trueになったら攻撃開始
    protected virtual int unlockCheckIndex { get; set; } = 0; // スキルロック解除チェック用インデックス



    void Awake()
    {
        AwakeCall();
    }
    protected virtual void AwakeCall() { } //一度だけ呼ばれる

    public virtual void Init(SubSkillParam _subSkilParam) // スキル初期化
    {
        param = _subSkilParam;
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





}
