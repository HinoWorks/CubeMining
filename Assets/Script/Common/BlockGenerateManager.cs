using UnityEngine;
using System.Collections.Generic;
using UniRx;


public enum BlockSize
{
    Normal,
    Big,
}



[System.Serializable]
public class GenerateBlockData
{
    public BlockGenerateParam param { get; private set; }
    private float timer = 0f;
    public BlockSize sizeType => Random.Range(0f, 1f) < param.bigBlockRate ? BlockSize.Big : BlockSize.Normal;


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
    [SerializeField] Vector2 range_x;
    [SerializeField] Vector2 range_y;
    [SerializeField] Vector2 range_z;


    // -- loc
    private List<MiningTarget_Cube> list_targetBlocks = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト
    private List<GenerateBlockData> list_generateBlockDatas = new List<GenerateBlockData>(); // 生成されるブロックのデータリスト

    private bool isGenerate = false;
    private int initialGenerateCount = 15;


    private float bigBlockSizeRate = 2f;
    private Vector3[] array_position = new Vector3[6]
    {
        new Vector3(1, 0, -1),
        new Vector3(-1, 1, 1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 1, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 1, 1),
    };



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
            if (!blockData.isActive) continue;
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
                var newBlock = Instantiate(_blockData.param.so.pf, InGameManager.Inst.ParentPool) as GameObject;
                targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                list_targetBlocks.Add(targetBlock);
            }

            targetBlock.transform.position = GetRandomPosition();
            targetBlock.transform.rotation = Quaternion.identity;

            var blockSizeType = _blockData.sizeType;
            var blockSize = blockSizeType == BlockSize.Big ? _blockData.param.size * bigBlockSizeRate : _blockData.param.size;
            targetBlock.transform.localScale = blockSize * Vector3.one;
            targetBlock.Init(_blockData.param.hp, _blockData.param.baseValue, _blockData.param.blockIndex);
            targetBlock.Set_BlockSize(blockSizeType);
        }
    }
    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(range_x.x, range_x.y), Random.Range(range_y.x, range_y.y), Random.Range(range_z.x, range_z.y));
    }

    public void BreakBigBlock(int _blockIndex, Vector3 _position)
    {
        var blockData = GameParamManager.list_blockGenerateParam.Find(x => x.blockIndex == _blockIndex);
        if (blockData == null) return;
        for (int i = 0; i < blockData.separateBlockCount; i++)
        {
            var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == blockData.blockIndex);
            if (targetBlock == null)
            {
                var newBlock = Instantiate(blockData.so.pf, InGameManager.Inst.ParentPool) as GameObject;
                targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                list_targetBlocks.Add(targetBlock);
            }

            targetBlock.transform.position = _position + 0.3f * array_position[i];
            targetBlock.transform.rotation = Quaternion.identity;
            targetBlock.transform.localScale = blockData.size * Vector3.one;
            targetBlock.Init(blockData.hp, blockData.baseValue, blockData.blockIndex);
            targetBlock.Set_BlockSize(BlockSize.Normal);
        }
    }


    #endregion



    /// <summary>
    /// アクティブなブロックをランダムに取得 存在しない場合はnullを返す
    /// </summary>
    public MiningTargetBase Get_RandomTargetBlock()
    {
        var activeBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled);
        if (activeBlocks.Count == 0) return null;
        return activeBlocks[Random.Range(0, activeBlocks.Count)];
    }

}
