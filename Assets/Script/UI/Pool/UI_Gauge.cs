using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;


public class UI_Gauge : MonoBehaviour
{
    protected Transform target;
    protected Vector3 offset;
    private RectTransform uiRectTransform;
    [SerializeField] protected bool ToOFF_atScreenOut = false;
    private CancellationTokenSource CTS;


    void Start()
    {
        if (!ToOFF_atScreenOut) return;
        uiRectTransform = GetComponent<RectTransform>();
    }

    // -- poolして使いまわすため、active時にfollowRestart
    void OnEnable()
    {
        if (CTS == null) CTS = new CancellationTokenSource();
        StartFollowTarget();
    }

    void OnDestroy()
    {
        if (CTS != null) CTS.Cancel();
    }

    public virtual void Initialize(Transform _target, Vector3 _offset)
    {
        this.target = _target;
        this.offset = _offset;
        SetPosition();
        this.gameObject.SetActive(true);
    }


    // --- CinemachineBrainのSmartUpdateは実行順序が最遅に設定
    // --- UniTaskを用いてPostLateUpdateで更新
    protected virtual void StartFollowTarget()
    {
        var CT = CTS.Token;
        var cancel = false;
        UniTask.Void(async () =>
        {
            while (this.gameObject.activeSelf)
            {
                if (target == null)
                {
                    await UniTask.Yield();
                    continue;
                }
                ;
                SetPosition();
                cancel = await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken: CT).SuppressCancellationThrow();
                if (cancel) return;
                OffScreenCheck();
            }
        });
    }

    //void LateUpdate()
    //{
    //    if (target == null) return;
    //   SetPosition();
    //}

    protected virtual void SetPosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position) + offset;// * CameraManager.Inst.zoomRate;
    }

    protected virtual void OffScreenCheck()
    {
        if (ToOFF_atScreenOut)
        {
            // 画面外にあるかどうかを判断
            bool isOffScreen = uiRectTransform.position.x < UI_PoolManager.Inst.screenOut_min_w ||
                                uiRectTransform.position.x > UI_PoolManager.Inst.screenOut_max_w ||
                                uiRectTransform.position.y < UI_PoolManager.Inst.screenOut_min_h ||
                                uiRectTransform.position.y > UI_PoolManager.Inst.screenOut_max_h;
            if (isOffScreen)
            {
                Return();
            }
        }
    }

    public virtual void Return()
    {
        target = null;
        this.gameObject?.SetActive(false);
    }





}
