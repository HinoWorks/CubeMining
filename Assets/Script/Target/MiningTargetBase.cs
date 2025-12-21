using UnityEngine;
using DG.Tweening;

public class MiningTargetBase : MonoBehaviour, IDamagable
{

    protected virtual int hp { get; set; } = 10;
    protected virtual int hp_max { get; set; } = 10;


    public virtual void Init()
    {
        gameObject.SetActive(true);
        hp = hp_max;
    }

    public virtual void Damage(int damage)
    {
        hp -= damage;
        DamageAction();
        if (hp <= 0)
        {
            NotActivate();
        }
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
