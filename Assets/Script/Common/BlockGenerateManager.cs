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
        for (int i = 0; i < blockCount; i++)
        {
            var newBlock = BlockGenerateManager.Inst.GenerateBlock(param.SelectBlockIndex());
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


    private int initialCreateLayer = 10;
    private int currentCreateLayer = 0;
    private int cameraTargetLayer = 0;

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }



    public void Init()
    {
        list_layerConts.Clear();
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
    }

    public void LayerClear(GenerateBlockLayerCont _layerCont)
    {
        list_layerConts.Remove(_layerCont);
        CreateNewLayerCont();

        cameraTargetLayer = _layerCont.layerIndex + 1;
        CameraManager.Inst.SetCameraPosition(cameraTargetLayer);
    }



    #region == Block Generate ==
    public MiningTarget_Cube GenerateBlock(int _blockIndex)
    {
        var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == _blockIndex);
        if (targetBlock == null)
        {
            var blockData = SOLoader.BlockData.GetBlockData(_blockIndex);
            var newBlock = Instantiate(blockData.pf, InGameManager.Inst.ParentPool) as GameObject;
            targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
            targetBlock.Init(blockData.hp, blockData.baseValue, _blockIndex);
            list_targetBlocks.Add(targetBlock);
        }
        return targetBlock;
    }
    #endregion





    public MiningTargetBase Get_RandomTargetBlock()
    {
        var activeBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled);
        if (activeBlocks.Count == 0) return null;
        return activeBlocks[Random.Range(0, activeBlocks.Count)];
    }

}
