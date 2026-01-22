using UnityEngine;
using DG.Tweening;

public class MiningTargetBase : MonoBehaviour, IDamagable
{
    public int index { get; private set; }
    protected virtual int hp { get; set; } = 10;
    protected virtual int hp_max { get; set; } = 10;
    protected float hp_rate => (float)hp / hp_max;
    public int value;
    public bool isAlive => hp > 0;


    protected float animScale_rate;
    private Vector3 animScale_1 => animScale_rate * new Vector3(1.1f, 0.9f, 1.1f);
    private Vector3 animScale_2 => animScale_rate * new Vector3(0.9f, 1.1f, 0.9f);
    private float animDuration = 0.075f;
    private Sequence seq_anim;

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
        animScale_rate = transform.localScale.x;
        gameObject.SetActive(true);
    }

    public virtual bool Damage(int damage)
    {
        hp -= damage;
        Set_DamageText(damage);
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
        if (seq_anim == null)
        {
            seq_anim = DOTween.Sequence();
            seq_anim.Append(transform.DOScale(animScale_1, animDuration).SetEase(Ease.OutBack));
            seq_anim.Append(transform.DOScale(animScale_2, animDuration).SetEase(Ease.OutBack));
            seq_anim.Append(transform.DOScale(animScale_rate * Vector3.one, animDuration).SetEase(Ease.OutBack));
            seq_anim.SetAutoKill(false).SetLink(this.gameObject).Pause();
        }
        seq_anim.Restart();
    }
    private void Set_DamageText(int _damage)
    {
        var ui_damageText = UI_PoolManager.Inst.Get_TextDamage();
        ui_damageText.SetPosition(transform.position);
        ui_damageText.SetText(_damage.ToString());
    }
}
