using UnityEngine;
using System.Collections.Generic;
using UniRx;

public enum BlockSize
{
    Normal,
    Big,
}

public class BlockGenerateManager_Time : MonoBehaviour
{
    public static BlockGenerateManager_Time Inst;
    // -- loc
    private List<MiningTarget_Cube> list_targetBlocks = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト（石ブロック）
    private List<MiningTarget_Cube> list_targetBlocks_Min = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト（資源ブロック 小）
    private List<MiningTarget_Cube> list_targetBlocks_Max = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト（資源ブロック 大）
    private List<MiningTarget_Cube> list_topTargetScratch = new List<MiningTarget_Cube>();
    private List<MiningTarget_Object> list_targetObjects = new List<MiningTarget_Object>(); // 生成されたオブジェクトのリスト
    private List<MiningTarget_Artifact> list_targetArtifacts = new List<MiningTarget_Artifact>(); // 生成されたアーティファクトのリスト
    public bool isGenerateArtifact { get; private set; } = false; // アーティファクト生成フラグ　（ingame中一度しか生成しない）

    private float timer = 0f;
    private float checkInterval = 1f;





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
        isGenerateArtifact = false;
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
        foreach (var targetBlock in list_targetBlocks_Max)
        {
            targetBlock.NotActivate();
        }
        foreach (var targetBlock in list_targetBlocks_Min)
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
    }

    private void InitialBlockCreate()
    {

    }
    private void Check_BlockCreate()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            Check_BlockCreate();
            timer = 0f;
        }
    }




    #region == Block Generate ==
    public MiningTarget_Cube GenerateBlock(BlockData _blockData, int _layerIndex)
    {
        // リソースタイプ抽選
        var resourceType = GameParamManager.Get_RandamBlockType(_blockData.blockIndex);
        MiningTarget_Cube targetBlock = null;

        // リソースなし
        if (resourceType == ResourceType.Stone)
        {
            targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == _blockData.blockIndex);
            if (targetBlock == null)
            {
                var newBlock = Instantiate(_blockData.pf, InGameManager.Inst.ParentPool) as GameObject;
                targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                list_targetBlocks.Add(targetBlock);
            }
            targetBlock.Init(_blockData.hp, _blockData.baseValue, _blockData.blockIndex, _layerIndex);
            targetBlock.Set_BlockType(_blockData.baseBlockType, resourceType);
            return targetBlock;
        }

        else //リソース入り
        {
            var resourceBlockIndex = 100 + (int)resourceType;
            var resourceBlockData = SOLoader.BlockData.GetBlockData(resourceBlockIndex);

            var isResourceMax = GameParamManager.IsMaxResource(resourceType);
            if (isResourceMax)//リソース最大サイズかチェック
            {
                targetBlock = list_targetBlocks_Max.Find(x => x.isActiveAndEnabled == false);
                if (targetBlock == null)
                {
                    var newBlock = Instantiate(SOLoader.BlockData.pf_Block_ResourceMax, InGameManager.Inst.ParentPool) as GameObject;
                    targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                    list_targetBlocks_Max.Add(targetBlock);
                }
            }
            else
            {
                targetBlock = list_targetBlocks_Min.Find(x => x.isActiveAndEnabled == false);
                if (targetBlock == null)
                {
                    var newBlock = Instantiate(SOLoader.BlockData.pf_Block_ResourceMin, InGameManager.Inst.ParentPool) as GameObject;
                    targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                    list_targetBlocks_Min.Add(targetBlock);
                }
            }
            var fixedResourceValue = (int)(resourceBlockData.baseValue
                                        * (isResourceMax ? 2f : 1f)
                                        + GameParamManager.Get_ResourceUpCount(resourceType) //個別の増加量
                                        + GameParamManager.Get_ResourceBaseUpCount() //共通の増加量
                                        );
            //Debug.Log($"{resourceType} => hp: {resourceBlockData.hp}, value: {fixedResourceValue}");
            targetBlock.Init(resourceBlockData.hp, fixedResourceValue, resourceBlockIndex, _layerIndex);
            targetBlock.Set_BlockType(_blockData.baseBlockType, resourceType);
            return targetBlock;
        }
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
        targetObject.transform.localPosition = Vector3.zero;
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

    #endregion
}