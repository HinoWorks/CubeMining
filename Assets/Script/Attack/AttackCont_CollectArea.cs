using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;

public class AttackCont_CollectArea : MonoBehaviour
{
    [SerializeField] ForceFieldArea forceFieldArea;



    void Start()
    {
        CreateRoop();
    }


    private void CreateRoop()
    {
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => this.gameObject.activeSelf)
            .Subscribe(_ =>
            {
                Collect();
            })
            .AddTo(this);
    }


    #region --- DEBUG ---

    [ContextMenu("Collect")]
    private void Collect()
    {
        forceFieldArea.Activate(ForceFieldArea.ForceType.Attract, transform.position);
    }
    #endregion

}
