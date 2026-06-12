using UnityEngine;
using System.Collections.Generic;
using UniRx;

public enum BlockSize
{
    Normal,
    Big,
}


public class BlockGenerateManager : MonoBehaviour
{
    public static BlockGenerateManager Inst;

    [SerializeField] GameObject[] array_blockPrefabs;
    // -- loc
    private List<MiningTarget_Cube> list_targetBlocks = new List<MiningTarget_Cube>();
    private List<MiningTarget_Object> list_targetObjects = new List<MiningTarget_Object>(); // 生成されたオブジェクトのリスト
    private List<MiningTarget_Artifact> list_targetArtifacts = new List<MiningTarget_Artifact>(); // 生成されたアーティファクトのリスト
    public bool isGenerateArtifact { get; private set; } = false; // アーティファクト生成フラグ　（ingame中一度しか生成しない）

    private float timer = 0f;
    private float checkInterval = 1f;

    private int initialCreateCount = 20;
    private int createCount_delta = 5;

    private bool isGenerate = false;
    private float randomBlockSizeRate => Random.Range(0.75f, 1.25f);

    private Vector3 generatePosition => new Vector3(Random.Range(-10, 10), Random.Range(7, 12), Random.Range(-10, 10));
    private Vector3 generateRotation => new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));




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
        InitialBlockCreate();
    }
    public void Set_GenerateState(bool _state)
    {
        isGenerate = _state;
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
    }

    private void InitialBlockCreate()
    {
        for (int i = 0; i < initialCreateCount; i++)
        {
            GenerateBlock();
        }
    }
    private void Check_BlockCreate()
    {
        var randomCount = Random.Range(1, createCount_delta + 1);
        for (int i = 0; i < randomCount; i++)
        {
            GenerateBlock();
        }
    }
    private void GenerateBlock()
    {
        var blockGenerateParam = GameParamManager.Get_RandamBlockIndex();

        var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == blockGenerateParam.blockIndex);
        if (targetBlock == null)
        {
            var newBlock = Instantiate(blockGenerateParam.pf, InGameManager.Inst.ParentPool) as GameObject;
            targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
            list_targetBlocks.Add(targetBlock);
        }

        targetBlock.Init(blockGenerateParam.hp, blockGenerateParam.baseValue, randomBlockSizeRate);
        targetBlock.Set_BlockType(blockGenerateParam.resourceType);
        targetBlock.transform.localPosition = generatePosition;
        targetBlock.transform.localRotation = Quaternion.Euler(generateRotation);
    }
    void Update()
    {
        if (!isGenerate) return;
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            Check_BlockCreate();
            timer = 0f;
        }
    }


    private GameObject Get_RandomBlockPrefab()
    {
        return array_blockPrefabs[Random.Range(0, array_blockPrefabs.Length)];
    }



    /*

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
    */

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
    public MiningTarget_Cube Get_RandomTargetCube()
    {
        var activeBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled);
        if (activeBlocks.Count == 0) return null;
        return activeBlocks[Random.Range(0, activeBlocks.Count)];
    }

    #endregion
}
