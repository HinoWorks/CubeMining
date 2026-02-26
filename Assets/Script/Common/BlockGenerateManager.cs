using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System.IO.Hashing;


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
    private int createdBlockCount = 0;

    public void Init(BlockGenerateParam_Layer _param, int _layerIndex)
    {
        param = _param;
        layerIndex = _layerIndex;
        breakBlockCount = 0;
        createdBlockCount = 0;
        GenerateBlock();
    }

    private void GenerateBlock()
    {
        for (int i = 0; i < blockCount; i++)
        {
            if (GameParamManager.IsOtherObjectGenerate())
            {
                var blockIndex = param.SelectBlockIndex();
                var blockData = SOLoader.BlockData.GetBlockData(blockIndex);

                var otherObject = GameParamManager.SelectOtherObject();
                var newObject = BlockGenerateManager.Inst.GenerateOtherObject(otherObject, blockData, layerIndex);
                newObject.transform.localPosition = GetBlockPosition(i);
                //newObject.Set_BreakCallback(BlockBreakCall); //重力で下層に落ちるので、layerカウントに含めない
            }
            else if (!BlockGenerateManager.Inst.isGenerateArtifact && GameParamManager.IsArtifactGenerate())
            {
                // アーティファクトを生成
                var newArtifact = BlockGenerateManager.Inst.GenerateArtifact(layerIndex);
                //newArtifact.Set_BreakCallback(BlockBreakCall);
                if (newArtifact == null)
                {
                    GenerateNormalBlock(i);
                }
                else
                {
                    newArtifact.transform.localPosition = GetBlockPosition(i);
                }
            }
            else
            {
                GenerateNormalBlock(i);
            }
        }
    }

    private void GenerateNormalBlock(int _blockCounter)
    {
        //現在の確率でブロックを抽選
        var blockIndex = param.SelectBlockIndex();
        var blockData = SOLoader.BlockData.GetBlockData(blockIndex);
        var newBlock = BlockGenerateManager.Inst.GenerateBlock(blockData, layerIndex);
        newBlock.transform.localPosition = GetBlockPosition(_blockCounter);
        newBlock.Set_BreakCallback(BlockBreakCall);
        createdBlockCount++;
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
        if (breakBlockCount >= createdBlockCount)
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
    private List<MiningTarget_Artifact> list_targetArtifacts = new List<MiningTarget_Artifact>(); // 生成されたアーティファクトのリスト
    private List<GenerateBlockLayerCont> list_layerConts = new List<GenerateBlockLayerCont>(); // 生成されたレイヤーのリスト
    private GenerateBlockLayerCont currentLayerCont; //最上層のレイヤー

    private int initialCreateLayer = 10;
    private int currentCreateLayer = 0;
    private int cameraTargetLayer => currentLayerCont == null ? 0 : currentLayerCont.layerIndex + 1;

    public bool isGenerateArtifact { get; private set; } = false; // アーティファクト生成フラグ　（ingame中一度しか生成しない）



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }


    /// <summary>
    /// インゲーム開始時の初期化
    /// </summary>
    public void Init()
    {
        list_layerConts.Clear();
        currentCreateLayer = 0;
        isGenerateArtifact = false;
        for (int i = 0; i < initialCreateLayer; i++)
        {
            CreateNewLayerCont();
        }
        GameEvent.UI.PublishDepthCount(0);
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
        foreach (var targetArtifact in list_targetArtifacts)
        {
            targetArtifact.NotActivate();
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

        if (currentLayerCont == null || currentLayerCont.layerIndex > newLayerCont.layerIndex)
        {
            currentLayerCont = newLayerCont;
        }
    }

    public void LayerClear(GenerateBlockLayerCont _layerCont)
    {
        list_layerConts.Remove(_layerCont);

        // 最上層 = 掘り進めている一番上 = layerIndex が最小のレイヤー
        var topLayerIndex = 99999;
        foreach (var layerCont in list_layerConts)
        {
            if (layerCont.layerIndex < topLayerIndex)
            {
                topLayerIndex = layerCont.layerIndex;
                currentLayerCont = layerCont;
            }
        }
        CameraManager.Inst.SetCameraPosition(cameraTargetLayer, currentLayerCont.param.so.layerSize);
        AroundLayerManager.Inst.CreateNewLayerCont(currentCreateLayer);
        CreateNewLayerCont();

        GameEvent.UI.PublishDepthCount(currentLayerCont.layerIndex);
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.Depth, currentLayerCont.layerIndex);
        Debug.Log($"currentLayerCont : {currentLayerCont.layerIndex}");
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

        var blockTypeData = GameParamManager.Get_BlockChangeRateParam(_blockData.blockIndex);
        var blockType = blockTypeData.SelectBlockType();
        targetBlock.Set_BlockType(_blockData.baseBlockType, blockType);
        return targetBlock;
    }
    #endregion


    #region == Other Object Generate ==
    public MiningTarget_Object GenerateOtherObject(ObjectGenerateParam _objectData, BlockData _blockData, int _layerIndex)
    {
        var targetObject = list_targetObjects.Find(x => x.isActiveAndEnabled == false && x.index == _objectData.so.objectIndex);
        if (targetObject == null)
        {
            var newObject = Instantiate(_objectData.so.pf, InGameManager.Inst.ParentPool) as GameObject;
            targetObject = newObject.GetComponent<MiningTarget_Object>();
            list_targetObjects.Add(targetObject);
        }
        targetObject.Init(_objectData, _blockData, _layerIndex);
        return targetObject;
    }
    #endregion

    #region == Artifact Generate ==
    public MiningTarget_Artifact GenerateArtifact(int _layerIndex)
    {
        var artifactIndexes = SaveLoader.Inst.Get_ArtifactIndex_NotGet();
        if (artifactIndexes.Length == 0)
        {
            //Debug.Log(" ===**** アーティファクトは全て所持 => 通常ブロック生成");
            return null;
        }

        var targetArtifact = list_targetArtifacts.Find(x => x.isActiveAndEnabled == false);
        if (targetArtifact == null)
        {
            var newArtifact = Instantiate(SOLoader.BlockData.pf_Artifact, InGameManager.Inst.ParentPool) as GameObject;
            targetArtifact = newArtifact.GetComponent<MiningTarget_Artifact>();
            list_targetArtifacts.Add(targetArtifact);
        }

        // 未所持のアーティファクトをランダムで選択
        var artifactIndex = artifactIndexes[Random.Range(0, artifactIndexes.Length)];
        targetArtifact.Init(artifactIndex, _layerIndex);
        isGenerateArtifact = true;
        return targetArtifact;
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
        return new Vector3(Random.Range(0, areaSize - 1), 0, Random.Range(0, -(areaSize - 1)));
    }
    /// <summary>
    /// 上層から指定した層数分までのうち、ランダムな外周ブロック位置を習得
    /// </summary>
    public (bool isShotLine_z, Vector3 position) Get_RandomTargetArea_Around(int _deltaLayer = 0)
    {
        var targetLayer = Random.Range(currentLayerCont.layerIndex, currentLayerCont.layerIndex + _deltaLayer);
        var targetLayerCont = list_layerConts.Find(x => x.layerIndex == targetLayer);
        if (targetLayerCont == null) targetLayerCont = currentLayerCont;
        var areaSize = currentLayerCont.param.so.layerSize;
        bool isRandomX = Random.Range(0, 2) == 0;
        if (isRandomX)
        {
            return (true, new Vector3(Random.Range(0, areaSize - 1), -targetLayer, -(areaSize - 1)));
        }
        else
        {
            return (false, new Vector3(areaSize - 1, -targetLayer, -Random.Range(0, areaSize - 1)));
        }
    }

    /// <summary>
    /// 最上層のランダムなブロック位置を習得
    /// </summary>
    public MiningTarget_Cube Get_TopTarget(int _deltaLayer = 0)
    {
        var topAreaBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled && x.layerIndex <= currentLayerCont.layerIndex + _deltaLayer);
        if (topAreaBlocks.Count == 0)
        {
            topAreaBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled && x.layerIndex <= currentLayerCont.layerIndex + _deltaLayer);
        }
        if (topAreaBlocks.Count == 0) return null;

        return topAreaBlocks[Random.Range(0, topAreaBlocks.Count)];
    }
    #endregion
}
