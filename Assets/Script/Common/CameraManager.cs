using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;
using UniRx;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Inst;
    [SerializeField] Transform parent_camera;
    private CinemachineCamera vcam;
    private Vector3 initialPosition_parent; // 主にlayer移動用
    private Vector3 initialPosition; //主にshake用
    private float initialZoom;
    private Tween shakeTween;

    // -- caemraMoveParam --
    [Space(10)]
    [Header("CameraMoveParam")]
    [SerializeField] float cameraMoveDelta_XZ = 4f / 7f;
    [SerializeField] float cameraZoomDelta = 1.5f / 7f;
    private int initialAreaSize = 3;


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
        vcam = GetComponentInChildren<CinemachineCamera>();
    }

    void Start()
    {
        initialPosition_parent = parent_camera.position;
        initialPosition = vcam.transform.localPosition;
        initialZoom = vcam.Lens.OrthographicSize;
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
    }


    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.Title:
                break;

            case GameStateType.InGame_Ready:
                return;

                vcam.transform.localPosition = initialPosition;
                vcam.Lens.OrthographicSize = initialZoom;
                parent_camera.position = initialPosition_parent;
                break;
            case GameStateType.InGame:
            case GameStateType.InGame_End:
            case GameStateType.Result:
            case GameStateType.OutGame:
                break;
        }
    }

    public void SetCameraPosition(int _layerIndex, int _areaSize)
    {
        return;


        var deltaPosition_XZ = cameraMoveDelta_XZ * (_areaSize - initialAreaSize);
        var deltaPosition = new Vector3(deltaPosition_XZ, -_layerIndex, -deltaPosition_XZ);
        parent_camera.DOMove(initialPosition_parent + deltaPosition, 0.2f).SetEase(Ease.InOutSine);

        var deltaZoom = cameraZoomDelta * (_areaSize - initialAreaSize);
        DOTween.To(() => vcam.Lens.OrthographicSize, x => vcam.Lens.OrthographicSize = x, initialZoom + deltaZoom, 0.2f).SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// カメラ振動 :小さめ（ブロック破壊時など）
    /// </summary>
    public void ShakeCamera_BlockBreak()
    {
        if (vcam == null) return;

        shakeTween?.Kill();
        shakeTween = vcam.transform
            .DOShakePosition(0.12f, 0.06f, 15, 60, false, true);
    }

    /// <summary>
    /// カメラ振動:大きめ
    /// </summary>
    public void ShakeCamera_Large()
    {
        if (vcam == null) return;

        shakeTween?.Kill();
        shakeTween = vcam.transform
            .DOShakePosition(0.2f, 0.1f, 15, 60, false, true);
    }
}
