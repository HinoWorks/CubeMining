using UnityEngine;
using UniRx;
using System;
using UnityEngine.Animations;


public class PickaxePowerCont_CollectAreaUnit : MonoBehaviour
{
    [SerializeField] ForceFieldArea forceFieldArea;
    [SerializeField] SimpleAnimation anim;
    private float sizeRate;
    private float aliveTime;
    private float collectPower;


    public void Init(float _sizeRate, float _aliveTime, float _collectPower)
    {
        sizeRate = _sizeRate;
        aliveTime = _aliveTime;
        collectPower = _collectPower;

        CreateRoop();
        anim.Play("Default");

        Observable.Timer(TimeSpan.FromSeconds(aliveTime))
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }


    private void CreateRoop()
    {
        Collect();

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => gameObject.activeSelf)
            .Subscribe(_ => Collect())
            .AddTo(this);
    }

    private void Collect()
    {
        forceFieldArea.Activate(ForceFieldArea.ForceType.Attract, sizeRate, collectPower);
    }

    public void DestroyCall()
    {
        Destroy(gameObject);
    }

}
