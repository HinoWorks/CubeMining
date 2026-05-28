using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;


public class PickaxePowerCont_LaserUnit : MonoBehaviour
{
    [SerializeField] TriggerSender triggerSender;
    private HashSet<IDamagable> list_targetBlocks = new HashSet<IDamagable>();
    private float duration = 0.75f;
    private int damage;
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


    public void Init(int _damage, int _maxCount)
    {
        damage = _damage;
        maxCount = _maxCount;
        this.transform.localRotation = quaternion.Euler(angle_start);
        LaserStart();
    }


    private void LaserStart()
    {
        transform.DOLocalRotate(angle_end, duration).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            Debug.Log($"<color=green> ==Laser== target:{list_targetBlocks.Count} // Damage:{damage} </color>");
            foreach (var target in list_targetBlocks)
            {
                if (target.isAlive)
                {
                    target.Damage(damage);
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
