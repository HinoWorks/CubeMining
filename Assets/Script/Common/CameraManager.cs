using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;
using UniRx;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Inst;
    private CinemachineCamera vcam;
    private Vector3 initialPosition;
    private Tween shakeTween;


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
        vcam = GetComponentInChildren<CinemachineCamera>();
    }

    void Start()
    {
        initialPosition = vcam.transform.position;
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
    }

    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.Title:
                break;

            case GameStateType.InGame_Ready:
                vcam.transform.position = initialPosition;
                break;
            case GameStateType.InGame:
            case GameStateType.InGame_End:
            case GameStateType.Result:
            case GameStateType.OutGame:
                break;
        }
    }

    public void SetCameraPosition(int _layerIndex)
    {
        vcam.transform.DOMove(initialPosition + new Vector3(0, -_layerIndex, 0), 0.2f).SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// ブロック破壊時などに呼ぶ軽いカメラ振動
    /// </summary>
    public void ShakeBlockBreak()
    {
        if (vcam == null) return;

        shakeTween?.Kill();
        shakeTween = vcam.transform
            .DOShakePosition(0.12f, 0.06f, 15, 60, false, true);
    }
}
