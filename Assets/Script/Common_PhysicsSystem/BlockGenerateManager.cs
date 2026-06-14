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
    private List<MiningTarget_Cube> list_targetBlocks_Max = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト（資源ブロック 大）
    private List<MiningTarget_Object> list_targetObjects = new List<MiningTarget_Object>(); // 生成されたオブジェクトのリスト
    private List<MiningTarget_Artifact> list_targetArtifacts = new List<MiningTarget_Artifact>(); // 生成されたアーティファクトのリスト
    public bool isGenerateArtifact { get; private set; } = false; // アーティファクト生成フラグ　（ingame中一度しか生成しない）
    private bool isArtifactAllGet = false; // アーティファクトが全て所持されたかどうか

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
        isArtifactAllGet = SaveLoader.Inst.Get_ArtifactIndex_NotGet().Length == 0 ? true : false;

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
        if (GameParamManager.IsOtherObjectGenerate())
        {
            GenerateOtherObject();
        }
        else if (!isArtifactAllGet && !isGenerateArtifact && GameParamManager.IsArtifactGenerate())
        {
            GenerateArtifact();
        }
        else
        {
            GenerateRockBlock();
        }
    }

    #region == Other Object Generate ==
    public MiningTarget_Object GenerateOtherObject()
    {
        var objectData = GameParamManager.SelectOtherObject();
        var blockData = SOLoader.BlockData.GetBlockData(objectData.so.objectIndex);

        var targetObject = list_targetObjects.Find(x => x.isActiveAndEnabled == false && x.index == objectData.so.objectIndex);
        if (targetObject == null)
        {
            var newObject = Instantiate(objectData.so.pf, InGameManager.Inst.ParentPool) as GameObject;
            targetObject = newObject.GetComponent<MiningTarget_Object>();
            list_targetObjects.Add(targetObject);
        }
        targetObject.Init(objectData, blockData);
        targetObject.transform.localPosition = generatePosition;
        targetObject.transform.localRotation = Quaternion.Euler(generateRotation);
        return targetObject;
    }
    #endregion


    #region == Artifact Generate ==
    public MiningTarget_Artifact GenerateArtifact()
    {
        var artifactIndexes = SaveLoader.Inst.Get_ArtifactIndex_NotGet();
        if (artifactIndexes.Length == 0) return null;

        var targetArtifact = list_targetArtifacts.Find(x => x.isActiveAndEnabled == false);
        if (targetArtifact == null)
        {
            var newArtifact = Instantiate(SOLoader.BlockData.pf_Artifact, InGameManager.Inst.ParentPool) as GameObject;
            targetArtifact = newArtifact.GetComponent<MiningTarget_Artifact>();
            list_targetArtifacts.Add(targetArtifact);
        }

        // 未所持のアーティファクトをランダムで選択
        var artifactIndex = artifactIndexes[Random.Range(0, artifactIndexes.Length)];
        targetArtifact.Init(artifactIndex);
        targetArtifact.transform.localPosition = generatePosition;
        targetArtifact.transform.localRotation = Quaternion.Euler(generateRotation);
        isGenerateArtifact = true;
        return targetArtifact;
    }
    #endregion


    #region == Block Generate ==
    private void GenerateRockBlock()
    {
        var blockGenerateParam = GameParamManager.Get_RandamBlockIndex();
        var isMaxResource = GameParamManager.IsMaxResource(blockGenerateParam.resourceType);

        MiningTarget_Cube targetBlock = null;
        if (isMaxResource) // フル鉱石
        {
            targetBlock = list_targetBlocks_Max.Find(x => x.isActiveAndEnabled == false && x.index == blockGenerateParam.blockIndex);
            if (targetBlock == null)
            {
                var newBlock = Instantiate(blockGenerateParam.pf_max, InGameManager.Inst.ParentPool) as GameObject;
                targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                list_targetBlocks_Max.Add(targetBlock);
            }
        }
        else
        {
            targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == blockGenerateParam.blockIndex);
            if (targetBlock == null)
            {
                var newBlock = Instantiate(blockGenerateParam.pf, InGameManager.Inst.ParentPool) as GameObject;
                targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                list_targetBlocks.Add(targetBlock);
            }
        }

        var fixedResourceValue = (int)(blockGenerateParam.baseValue
                                  * (isMaxResource ? 2f : 1f)
                                  + GameParamManager.Get_ResourceUpCount(blockGenerateParam.resourceType) //個別の増加量
                                  + GameParamManager.Get_ResourceBaseUpCount() //共通の増加量
                                  );

        targetBlock.Init(blockGenerateParam.hp, blockGenerateParam.baseValue, randomBlockSizeRate);
        targetBlock.Set_BlockType(blockGenerateParam.resourceType);
        targetBlock.transform.localPosition = generatePosition;
        targetBlock.transform.localRotation = Quaternion.Euler(generateRotation);



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


    /// <summary>
    /// ランダムなポイントを取得
    /// </summary>
    public Vector3 Get_RandomTargetPoint()
    {
        return generatePosition;
    }
    #endregion
}
