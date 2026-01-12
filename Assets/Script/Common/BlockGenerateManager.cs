using UnityEngine;
using System.Collections.Generic;
using UniRx;



[System.Serializable]
public class GenerateBlockData
{
    public BlockGenerateParam param { get; private set; }
    private float timer = 0f;

    public void Init(BlockGenerateParam _param)
    {
        param = _param;
    }

    public void UnityUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= param.generateInterval)
        {
            BlockGenerateManager.Inst.GenerateBlock(this);
            timer = 0f;
        }
    }

}


public class BlockGenerateManager : MonoBehaviour
{
    public static BlockGenerateManager Inst;
    [SerializeField] Transform parentPool;
    [SerializeField] Vector2 range_x;
    [SerializeField] Vector2 range_y;
    [SerializeField] Vector2 range_z;


    // -- loc
    private List<MiningTargetBase> list_targetBlocks = new List<MiningTargetBase>(); // 生成されたブロックのリスト
    private List<GenerateBlockData> list_generateBlockDatas = new List<GenerateBlockData>(); // 生成されるブロックのデータリスト
    private int[] unlockBlockIndexes;

    private bool isGenerate = false;
    private float generateInterval = 1f;
    private int initialGenerateCount = 15;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }



    void Update()
    {
        if (!isGenerate) return;

        for (int i = 0; i < list_generateBlockDatas.Count; i++)
        {
            list_generateBlockDatas[i].UnityUpdate();
        }
    }

    public void Init()
    {
        Set_BlockGenerateDatas();


        for (int i = 0; i < initialGenerateCount; i++)
        {
            GenerateBlock(list_generateBlockDatas[0]);
        }

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
    }




    #region local method 
    private void Set_BlockGenerateDatas()
    {
        list_generateBlockDatas.Clear();

        foreach (var blockData in GameParamManager.list_blockGenerateParam)
        {
            if (blockData.blockIndex == 1 || blockData.blockIndex == 2) { } // DEBUG == block index 1 and 2 is always generate
            else if (!blockData.isActive) continue;

            var generateBlockData = new GenerateBlockData();
            generateBlockData.Init(blockData);
            list_generateBlockDatas.Add(generateBlockData);
        }
    }

    public void GenerateBlock(GenerateBlockData _blockData)
    {
        for (int i = 0; i < _blockData.param.count; i++)
        {
            var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == _blockData.param.blockIndex);
            if (targetBlock == null)
            {
                var newBlock = Instantiate(_blockData.param.so.pf, parentPool) as GameObject;
                targetBlock = newBlock.GetComponent<MiningTargetBase>();
                list_targetBlocks.Add(targetBlock);
            }

            targetBlock.transform.position = GetRandomPosition();
            targetBlock.transform.rotation = Quaternion.identity;
            targetBlock.transform.localScale = _blockData.param.size * Vector3.one;
            targetBlock.Init(_blockData.param.hp, _blockData.param.baseValue, _blockData.param.blockIndex);
        }
    }
    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(range_x.x, range_x.y), Random.Range(range_y.x, range_y.y), Random.Range(range_z.x, range_z.y));
    }
    #endregion

}
