using UnityEngine;
using DG.Tweening;

public class MiningTargetBase : MonoBehaviour, IDamagable
{

    protected virtual int hp { get; set; } = 10;

    public virtual void Damage(int damage)
    {
        hp -= damage;
        DamageAction();
        if (hp <= 0)
        {
            Destroy();
        }
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    private void DamageAction()
    {
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
    }

}
