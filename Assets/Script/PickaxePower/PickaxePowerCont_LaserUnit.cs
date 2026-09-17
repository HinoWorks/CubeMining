using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


public class PickaxePowerCont_LaserUnit : MonoBehaviour
{
    [SerializeField] TriggerSender triggerSender;

    private HashSet<IDamagable> list_targetBlocks = new HashSet<IDamagable>();
    private float duration = 0.75f;
    private int damage;
    private float laserSize;
    private float rotateAngle;
    private Vector3 offsetPosi_trigger; // laserSizeと同じ分だけオフセットする
    private int maxCount;

    private Vector3 angle_start = new Vector3(0f, 0f, 0f);
    private Vector3 angle_end = new Vector3(0f, 180f, 0f);


    void Awake()
    {
        triggerSender.OnEnter += OnTriggerEnter;
    }
    void OnDestroy()
    {
        triggerSender.OnEnter -= OnTriggerEnter;
    }


    public void Init(int _damage, int _maxCount, float _laserSize, float _rotateAngle)
    {
        damage = _damage;
        maxCount = _maxCount;
        laserSize = _laserSize;
        rotateAngle = _rotateAngle;

        SetLaserLength();
        SetRotateAngles();
        transform.localRotation = Quaternion.Euler(angle_start);
        LaserStart();
    }

    private void SetLaserLength()
    {
        var triggerTf = triggerSender.transform;
        var scale = triggerTf.localScale;
        scale.y = laserSize;
        triggerTf.localScale = scale;

        offsetPosi_trigger = new Vector3(0f, 0f, -laserSize);
        triggerTf.localPosition = offsetPosi_trigger;
    }

    private void SetRotateAngles()
    {
        // レーザーは local -Z 向き。-X を基準に、回転量の半分だけ -Z 側へ倒した位置から開始する
        var halfAngle = rotateAngle * 0.5f;
        const float baseAngleY = 90f; // -X
        angle_start = new Vector3(0f, baseAngleY - halfAngle, 0f);
        angle_end = new Vector3(0f, baseAngleY + halfAngle, 0f);
    }


    private void LaserStart()
    {
        transform.DOLocalRotate(angle_end, duration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            Debug.Log($"<color=green> ==Laser== target:{list_targetBlocks.Count} // Damage:{damage} </color>");
            foreach (var target in list_targetBlocks)
            {
                if (target.isAlive)
                {
                    target.Damage(damage);
                    var effect = EffectManager.Inst?.Get_Effect(EffectType.LasaerHit);
                    if (effect != null)
                    {
                        effect.transform.position = target.GetTransform().position;
                        effect.SetActive(true);
                    }
                }
            }
            list_targetBlocks.Clear();
            StaticManager.SlowGameTime_PickaxePower();
            CameraManager.Inst.ShakeCamera_Large();
            Destroy(this.gameObject);
        }).Play();
    }


    void OnTriggerEnter(Collider other)
    {
        if (list_targetBlocks.Count >= maxCount) return;
        if (other.TryGetComponent(out IDamagable target))
        {
            list_targetBlocks.Add(target);
        }
    }

}
