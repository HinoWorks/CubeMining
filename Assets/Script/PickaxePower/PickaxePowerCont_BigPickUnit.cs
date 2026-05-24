using UnityEngine;
using DG.Tweening;

public class PickaxePowerCont_BigPickUnit : MonoBehaviour
{
    [SerializeField] GameObject obj_damageArea;
    [SerializeField] TriggerSender triggerSender;
    [SerializeField] ParticleSystem eff_Attack;
    private int damage;
    private float sizeRate;
    private Vector3 targetPosition;


    void OnDestroy()
    {
        triggerSender.OnEnter -= OnEnter;
    }

    public void Init(int _damage, float _sizeRate, Vector3 _targetPosition)
    {
        damage = _damage;
        sizeRate = _sizeRate;
        targetPosition = _targetPosition;

        triggerSender.OnEnter += OnEnter;
    }


    private void OnEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            if (!target.isAlive) return;
            target.Damage(damage);
        }
    }

    /// <summary>
    /// アニメからcall
    /// </summary>
    public void Set_DamageArea()
    {
        obj_damageArea.transform.localScale = Vector3.zero;
        obj_damageArea.transform.position = targetPosition;
        obj_damageArea.SetActive(true);
        var targetSize = sizeRate * Vector3.one;

        eff_Attack.Play();
        CameraManager.Inst.ShakeCamera_Large();
        obj_damageArea.transform.DOScale(targetSize, 0.15f).SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                Destroy(this.gameObject);
            });
        });
    }
}
