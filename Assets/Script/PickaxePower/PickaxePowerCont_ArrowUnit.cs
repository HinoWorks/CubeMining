using UnityEngine;
using System.Collections.Generic;

public class PickaxePowerCont_ArrowUnit : MonoBehaviour
{

    private int damage;
    private float sizeRate;
    private Vector3 velocity;
    private Rigidbody rb;

    [SerializeField] TriggerSender triggerSender;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        triggerSender.OnEnter += OnTriggerEnter;
    }
    void OnDestroy()
    {
        triggerSender.OnEnter -= OnTriggerEnter;
    }



    public void Init(int _damage, float _sizeRate, Vector3 _velocity)
    {
        this.damage = _damage;
        this.sizeRate = _sizeRate;
        this.velocity = _velocity;

        rb.linearVelocity = Vector3.zero;
        this.gameObject.SetActive(true);
    }

    public void ShotArrow()
    {
        rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            if (!target.isAlive) return;
            rb.linearVelocity = Vector3.zero;
            target.Damage(damage);

            var eff_arrowHit = EffectManager.Inst.Get_Effect(EffectType.ArrowHit);
            eff_arrowHit.transform.position = transform.position;
            eff_arrowHit.SetActive(true);

            this.gameObject.SetActive(false);
        }
    }
}
