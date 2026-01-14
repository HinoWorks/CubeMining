using UnityEngine;
using DG.Tweening;

public class MiningTargetBase : MonoBehaviour, IDamagable
{
    public int index { get; private set; }
    protected virtual int hp { get; set; } = 10;
    protected virtual int hp_max { get; set; } = 10;
    public int value;
    public bool isAlive => hp > 0;


    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }


    public virtual void Init(int _hp, int _value, int _index)
    {
        hp_max = _hp;
        index = _index;
        value = _value;
        hp = hp_max;

        col.enabled = true;
        gameObject.SetActive(true);
    }

    public virtual bool Damage(int damage)
    {
        hp -= damage;
        DamageAction();
        if (hp <= 0)
        {
            col.enabled = false;
            BreakFromDamage();
            return true;
        }
        return false;
    }



    public virtual void BreakFromDamage()
    {
        gameObject.SetActive(false);
    }
    public virtual void NotActivate()
    {
        gameObject.SetActive(false);
    }

    private void DamageAction()
    {
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
    }

}
