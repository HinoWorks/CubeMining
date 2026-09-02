using UnityEngine;
using DG.Tweening;

public class MiningTargetBase : MonoBehaviour, IDamagable, IForce
{
    public int index { get; private set; }
    protected virtual int hp { get; set; } = 10;
    protected virtual int hp_max { get; set; } = 10;
    //public int layerIndex { get; protected set; }
    protected float hp_rate => (float)hp / hp_max;
    public int value { get; private set; }

    protected float areaBonusRate;//ボーナスエリアの上昇分
    public bool isAlive => hp > 0;

    public Transform GetTransform() => transform;

    public virtual void ApplyForce(Vector3 _force)
    {
        if (rb == null || !isAlive) return;
        rb.WakeUp();
        rb.AddForce(_force, ForceMode.Impulse);
    }

    protected float animScale_rate = 1f;
    private Vector3 animScale_1 => animScale_rate * new Vector3(1.05f, 0.95f, 1.05f);
    private Vector3 animScale_2 => animScale_rate * new Vector3(0.95f, 1.05f, 0.95f);
    private float animDuration = 0.05f;
    private Sequence seq_anim;

    private Collider col;
    protected Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }


    public virtual void Init(int _hp, int _value, float _sizeRate, int _index = 0)
    {
        hp_max = _hp;
        areaBonusRate = 0f;
        value = _value;
        hp = hp_max;
        index = _index;
        animScale_rate = _sizeRate;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
        }
        transform.localScale = Vector3.one * animScale_rate;
        col.enabled = true;

        this.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        this.transform.DOScale(Vector3.one * animScale_rate, 0.15f).SetEase(Ease.OutBack).Play();
    }

    public virtual bool Damage(int damage, float _resourceUpRate = 1f)
    {
        hp -= damage;
        //Set_DamageText(damage);
        DamageAction();
        if (hp <= 0)
        {
            col.enabled = false;
            BreakFromDamage(_resourceUpRate);
            PlaySE_Break();
            return true;
        }
        PlaySE_Damage();
        return false;
    }

    protected virtual void PlaySE_Damage() { }
    protected virtual void PlaySE_Break() { }



    public virtual void BreakFromDamage(float _resourceUpRate = 0f) { }

    /// <summary>
    /// ボーナスエリアの上昇分を追加
    /// </summary>
    public void AddValueBonusRate(float _bonusRate)
    {
        if (areaBonusRate > 0f) return;
        areaBonusRate += _bonusRate;
    }
    /// <summary>
    /// ボーナスエリアの上昇分を削除
    /// </summary>
    public void RemoveValueBonusRate()
    {
        areaBonusRate = 0f;
    }


    public virtual void NotActivate()
    {
        gameObject.SetActive(false);
    }

    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 5.0f, 0.5f); // 柱の「半分のサイズ」
    [SerializeField] private LayerMask blockLayer;

    protected virtual void WakeUpNeighborBlocks()
    {
        Collider[] hitColliders = new Collider[8];

        // 柱の中心点を「自分の少し上」にずらす計算
        Vector3 boxCenter = transform.position + Vector3.up * boxHalfExtents.y;
        // 四角い柱の傾き（自分の回転と合わせる、あるいは Quaternion.identity で世界軸に固定）
        Quaternion boxRotation = Quaternion.identity;

        int numColliders = Physics.OverlapBoxNonAlloc(
            boxCenter,
            boxHalfExtents,
            hitColliders,
            boxRotation,
            blockLayer
        );

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].gameObject == gameObject) continue;
            Rigidbody neighborRb = hitColliders[i].attachedRigidbody;
            if (neighborRb != null)
            {
                neighborRb.WakeUp();
            }
        }
    }

    // インスペクターで範囲を確認しやすくするためのデバッグ表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 boxCenter = transform.position + Vector3.up * boxHalfExtents.y;
        Gizmos.DrawWireCube(boxCenter, boxHalfExtents * 2);
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

}
