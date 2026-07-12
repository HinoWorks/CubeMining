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

    // -- loc
    private List<MiningTarget_Cube> list_targetBlocks = new List<MiningTarget_Cube>();
    private List<MiningTarget_Cube> list_targetBlocks_Max = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト（資源ブロック 大）
    private List<MiningTarget_Object> list_targetObjects = new List<MiningTarget_Object>(); // 生成されたオブジェクトのリスト


    // ブロック生成周りのパラ
    private float timer = 0f;
    private float checkInterval => GameParamManager.gameBaseParam.blockGenerate_duration;
    private int initialCreateCount => GameParamManager.gameBaseParam.blockGenerate_initialCount;
    private float createCount_delta => GameParamManager.gameBaseParam.blockGenerate_createCount_deltaTime;


    // tower generate param
    private bool isTowerGenerate => GameParamManager.gameBaseParam.isTowerUnlock;
    private float timer_towerGenerate = 0f;
    private int towerGenerateCount => GameParamManager.gameBaseParam.towerGenerate_count;
    private int towerCount_vertical => GameParamManager.gameBaseParam.towerGenerate_height;
    private float checkInterval_towerGenerate => GameParamManager.gameBaseParam.towerGenerate_duration;
    private Vector3 generatePosition_tower => new Vector3(Random.Range(-10, 10), Random.Range(3, 5), Random.Range(-10, 10));

    private bool isGenerate = false;
    private float randomBlockSizeRate => Random.Range(0.75f, 1.25f);

    public Vector3 generatePosition => new Vector3(Random.Range(-10, 10), Random.Range(3, 5), Random.Range(-10, 10));
    public Vector3 generateRotation => new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));




    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); return; }

        // EnsureGenerateManager<ArtifactGenerateManager>();
        // EnsureGenerateManager<EnhanceCoinGenerateManager>();
    }

    private void EnsureGenerateManager<T>() where T : MonoBehaviour
    {
        if (GetComponent<T>() == null)
        {
            gameObject.AddComponent<T>();
        }
    }


    /// <summary>
    /// インゲーム開始時の初期化
    /// </summary>
    public void Init()
    {
        ArtifactGenerateManager.Inst.Init();
        EnhanceCoinGenerateManager.Inst.Init();

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
        ArtifactGenerateManager.Inst.ResetAll();
        EnhanceCoinGenerateManager.Inst.ResetAll();
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

        // -- block generate --
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            Check_BlockCreate();
            timer = 0f;
        }

        // -- tower generate --
        if (isTowerGenerate)
        {
            timer_towerGenerate += Time.deltaTime;
            if (timer_towerGenerate >= checkInterval_towerGenerate)
            {
                Check_TowerGenerate();
                timer_towerGenerate = 0f;
            }
        }

        // -- enhance coin generate check --
        EnhanceCoinGenerateManager.Inst.UnityUpDate();

        // -- artifact generate check --
        ArtifactGenerateManager.Inst.UnityUpDate();
    }

    #region == Block Create Base ==
    private void Check_BlockCreate()
    {
        for (int i = 0; i < createCount_delta; i++)
        {
            GenerateBlock();
        }
        //ArtifactGenerateManager.Inst.Check_ArtifactGenerate();
        //EnhanceCoinGenerateManager.Inst.Check_EnhanceCoinGenerate();
    }
    private void GenerateBlock()
    {
        if (GameParamManager.IsOtherObjectGenerate())
        {
            GenerateOtherObject();
        }
        else
        {
            var targetBlock = GenerateRockBlock();
            targetBlock.transform.localPosition = generatePosition;
            targetBlock.transform.localRotation = Quaternion.Euler(generateRotation);
        }
    }
    public void CreateBlock(int _count)
    {
        for (int i = 0; i < _count; i++)
        {
            GenerateBlock();
        }
    }
    #endregion



    #region == Other Object Generate ==
    public MiningTarget_Object GenerateOtherObject(int _index = -1)
    {
        var objectData = _index == -1 ? GameParamManager.SelectOtherObject() : GameParamManager.SelectOtherObject(_index);
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
    public void Create_BonusChest()
    {
        GenerateOtherObject(1);
    }
    public void Create_Timer()
    {
        GenerateOtherObject(2);
    }
    public void Create_Bomb()
    {
        GenerateOtherObject(3);  //1:tresure, 2:timer, 3:bomb
    }
    #endregion


    #region == Block Generate ==
    private GameObject GenerateRockBlock(bool isNormalRate = true)
    {
        var blockGenerateParam = isNormalRate ? GameParamManager.Get_RandamBlockIndex()
                                            : GameParamManager.Get_RandamBlockIndex_OverIronUp();
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
                                  * (isMaxResource ? 2f : 1f) //フル鉱石の場合は2倍
                                  + GameParamManager.Get_ResourceUpCount(blockGenerateParam.resourceType) //個別の増加量
                                  + GameParamManager.Get_ResourceBaseUpCount() //共通の増加量
                                  );
        //Debug.Log($"baseValue: {blockGenerateParam.baseValue}, isMaxResource: {isMaxResource}, resourceUpCount: {GameParamManager.Get_ResourceUpCount(blockGenerateParam.resourceType)}, resourceBaseUpCount: {GameParamManager.Get_ResourceBaseUpCount()}, fixedResourceValue: {fixedResourceValue}");

        targetBlock.Init(blockGenerateParam.hp, fixedResourceValue, randomBlockSizeRate);
        targetBlock.Set_BlockType(blockGenerateParam.resourceType);

        return targetBlock.gameObject;
    }
    #endregion




    #region == Tower Generate ==
    private void Check_TowerGenerate()
    {
        Debug.Log("GenerateTower");
        //if (!isTowerGenerate) return;
        for (int i = 0; i < towerGenerateCount; i++)
        {
            GenerateTower();
        }
    }
    private void GenerateTower()
    {
        var generatePosition = generatePosition_tower;
        for (int i = 0; i < towerCount_vertical; i++)
        {
            var targetBlock = GenerateRockBlock(false);
            targetBlock.transform.localPosition = new Vector3(generatePosition.x, 1 + i * 1f, generatePosition.z);
            targetBlock.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
    #endregion



    #region == Block Regen ==
    public void Check_BlockRegen()
    {
        var isRegain = UnityEngine.Random.Range(0f, 1f) < GameParamManager.gameBaseParam.blockRegenRate;
        if (!isRegain) return;

        var targetBlock = GenerateRockBlock();
        targetBlock.transform.localPosition = generatePosition;
        targetBlock.transform.localRotation = Quaternion.Euler(generateRotation);

        var commonText = UI_PoolManager.Inst.Set_CommonText(UI_CommonTextType.BlockRegen);
        commonText.SetPosition_OneShot(targetBlock.transform.position);
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
