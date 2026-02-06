using UnityEngine;
using System.Collections.Generic;
using UniRx;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AroundLayerManager : MonoBehaviour
{
    public static AroundLayerManager Inst;
    [SerializeField] Transform parent_layerConts;
    [SerializeField] GameObject pf_layerCont;
    [SerializeField] SO_BlockLayerData so_blockLayerData;
    [SerializeField] List<AroundLayerCont> list_layerConts = new List<AroundLayerCont>();
    [SerializeField] int initialLayerCount = 10;


    [Space(10)]
    [Header(" -- side unit setting --")]
    [SerializeField] Transform parent_startAnim;
    private Vector3 startAnim_basePosition = new Vector3(1, 0, -1);



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

#if UNITY_EDITOR
    [ContextMenu("=== Create - LayerConts ===")]
    private void OnValidate_CreateLayerConts()
    {
        if (initialLayerCount <= list_layerConts.Count) return;

        NotActivate_AllLayers();
        for (int i = 0; i < initialLayerCount; i++)
        {
            var newLayerCont = CreateLayerCont();
            newLayerCont.Init(i, so_blockLayerData.GetBlockLayerData(i).layerSize);
        }
    }
#endif

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
    }

    private void Init_AllLayers()
    {
        for (int i = 0; i < initialLayerCount; i++)
        {
            var targetLayer = list_layerConts[i];
            targetLayer.Init(i, GameParamManager.Get_BlockGenerateParam_Layer(i).layerSize);
        }
    }


    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.Title:
                break;

            case GameStateType.InGame_Ready:
                Set_GroundAnimation(true);
                Init_AllLayers();
                break;
            case GameStateType.InGame:
            case GameStateType.InGame_End:
            case GameStateType.Result:
                break;
            case GameStateType.OutGame:
                NotActivate_AllLayers();
                Set_GroundAnimation(false);
                break;
        }
    }

    public void CreateNewLayerCont(int _layerIndex)
    {
        var newLayerCont = CreateLayerCont();
        newLayerCont.Init(_layerIndex, so_blockLayerData.GetBlockLayerData(_layerIndex).layerSize);
    }


    private AroundLayerCont CreateLayerCont()
    {
        var freeLayer = list_layerConts.Find(x => x.gameObject.activeSelf == false);
        if (freeLayer == null)
        {
            GameObject newLayerCont;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // エディタ内でプレハブ接続を保持したまま生成
                newLayerCont = PrefabUtility.InstantiatePrefab(pf_layerCont, parent_layerConts) as GameObject;
            }
            else
            {
                newLayerCont = Instantiate(pf_layerCont, parent_layerConts);
            }
#else
            newLayerCont = Instantiate(pf_layerCont, parent_layerConts);
#endif
            freeLayer = newLayerCont.GetComponent<AroundLayerCont>();
            list_layerConts.Add(freeLayer);
        }
        return freeLayer;
    }

    private void NotActivate_AllLayers()
    {
        foreach (var layerCont in list_layerConts)
        {
            layerCont.NotActivate();
        }
    }



    private void Set_GroundAnimation(bool _isOpen)
    {
        if (_isOpen)
        {
            parent_startAnim.gameObject.SetActive(true);
            parent_startAnim.DOLocalMoveY(-10, 1.5f).SetEase(Ease.InBack).SetDelay(0.25f)
                .OnComplete(() =>
                {
                    parent_startAnim.gameObject.SetActive(false);
                });
        }
        else
        {
            parent_startAnim.gameObject.SetActive(true);
            parent_startAnim.localPosition = startAnim_basePosition;
        }
    }


}
