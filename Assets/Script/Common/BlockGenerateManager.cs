using UnityEngine;
using System.Collections.Generic;
using UniRx;


public enum BlockSize
{
    Normal,
    Big,
}



[System.Serializable]
public class GenerateBlockLayerCont
{
    public BlockGenerateParam_Layer param;
    public int layerIndex;
    public int blockCount => param.so.layerSize * param.so.layerSize;
    private int breakBlockCount = 0;

    public void Init(BlockGenerateParam_Layer _param, int _layerIndex)
    {
        param = _param;
        layerIndex = _layerIndex;
        breakBlockCount = 0;
        GenerateBlock();
    }

    private void GenerateBlock()
    {
        var blockData = SOLoader.BlockData.GetBlockData(param.SelectBlockIndex());
        for (int i = 0; i < blockCount; i++)
        {
            var newBlock = BlockGenerateManager.Inst.GenerateBlock(blockData, layerIndex);
            newBlock.transform.localPosition = GetBlockPosition(i);
            newBlock.Set_BreakCallback(BlockBreakCall);
        }
    }
    public Vector3 GetBlockPosition(int _blockIndex)
    {
        var row = _blockIndex / param.so.layerSize;
        var col = _blockIndex % param.so.layerSize;
        return new Vector3(row, -layerIndex, -col);
    }

    private void BlockBreakCall()
    {
        breakBlockCount++;
        if (breakBlockCount >= blockCount)
        {
            BlockGenerateManager.Inst.LayerClear(this);
        }
    }


}


public class BlockGenerateManager : MonoBehaviour
{
    public static BlockGenerateManager Inst;

    // -- loc
    private List<MiningTarget_Cube> list_targetBlocks = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト
    private List<MiningTarget_Object> list_targetObjects = new List<MiningTarget_Object>(); // 生成されたオブジェクトのリスト
    private List<GenerateBlockLayerCont> list_layerConts = new List<GenerateBlockLayerCont>(); // 生成されたレイヤーのリスト
    private GenerateBlockLayerCont currentLayerCont;


    private int initialCreateLayer = 10;
    private int currentCreateLayer = 0;
    private int cameraTargetLayer => currentLayerCont == null ? 0 : currentLayerCont.layerIndex + 1;

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }



    public void Init()
    {
        list_layerConts.Clear();
        currentCreateLayer = 0;
        for (int i = 0; i < initialCreateLayer; i++)
        {
            CreateNewLayerCont();
        }
    }
    public void Set_GenerateState(bool _state)
    {
        //isGenerate = _state;
    }
    public void ResetAllBlocks()
    {
        foreach (var targetBlock in list_targetBlocks)
        {
            targetBlock.NotActivate();
        }
        foreach (var targetObject in list_targetObjects)
        {
            targetObject.NotActivate();
        }
        list_layerConts.Clear();
    }

    private void CreateNewLayerCont()
    {
        var blockLayerData = GameParamManager.Get_BlockGenerateParam_Layer(currentCreateLayer);
        var newLayerCont = new GenerateBlockLayerCont();
        newLayerCont.Init(blockLayerData, currentCreateLayer);
        list_layerConts.Add(newLayerCont);
        currentCreateLayer++;

        if (currentLayerCont == null || currentLayerCont.layerIndex < newLayerCont.layerIndex)
        {
            currentLayerCont = newLayerCont;
        }
    }

    public void LayerClear(GenerateBlockLayerCont _layerCont)
    {
        list_layerConts.Remove(_layerCont);

        var topLayerIndex = 99999;
        foreach (var layerCont in list_layerConts)
        {
            if (layerCont.layerIndex < topLayerIndex)
            {
                topLayerIndex = layerCont.layerIndex;
                currentLayerCont = layerCont;
            }
        }
        CameraManager.Inst.SetCameraPosition(cameraTargetLayer);

        CreateNewLayerCont();
    }



    #region == Block Generate ==
    public MiningTarget_Cube GenerateBlock(BlockData _blockData, int _layerIndex)
    {
        var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == _blockData.blockIndex);
        if (targetBlock == null)
        {
            var newBlock = Instantiate(_blockData.pf, InGameManager.Inst.ParentPool) as GameObject;
            targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
            list_targetBlocks.Add(targetBlock);
        }
        targetBlock.Init(_blockData.hp, _blockData.baseValue, _blockData.blockIndex, _layerIndex);
        return targetBlock;
    }
    #endregion






    #region == Random Target ==
    /// <summary>
    /// ランダムに生成されたブロックを取得
    /// </summary>
    public MiningTargetBase Get_RandomTargetBlock()
    {
        var activeBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled);
        if (activeBlocks.Count == 0) return null;
        return activeBlocks[Random.Range(0, activeBlocks.Count)];
    }
    /// <summary>
    /// 上層のランダムなブロック位置を習得
    /// </summary>
    public Vector3 Get_RandomTargetArea()
    {
        var areaSize = currentLayerCont.param.so.layerSize;
        return new Vector3(Random.Range(0, areaSize), 0, Random.Range(0, areaSize));
    }

    /// <summary>
    /// 最上層のランダムなブロック位置を習得
    /// </summary>
    public MiningTarget_Cube Get_TopTarget()
    {
        var topAreaBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled && x.layerIndex == currentLayerCont.layerIndex);
        if (topAreaBlocks.Count == 0) return null;
        return topAreaBlocks[Random.Range(0, topAreaBlocks.Count)];
    }

    #endregion
}
