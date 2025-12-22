using UnityEngine;
using System.Collections.Generic;
using UniRx;



[System.Serializable]
public class GenerateBlockData
{
    public BlockData so;
    public float generateRate;
}


public class BlockGenerateManager : MonoBehaviour
{
    public static BlockGenerateManager Inst;
    [SerializeField] Transform parentPool;
    [SerializeField] Vector2 range_x;
    [SerializeField] Vector2 range_y;
    [SerializeField] Vector2 range_z;


    // -- loc
    private List<MiningTargetBase> list_targetBlocks = new List<MiningTargetBase>();
    private List<GenerateBlockData> list_generateBlockDatas = new List<GenerateBlockData>();
    private int[] unlockBlockIndexes;


    private bool isGenerate = false;
    private float timer = 0f;
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
        timer += Time.deltaTime;
        if (timer >= generateInterval)
        {
            GenerateBlock();
            timer = 0f;
        }
    }

    public void InitialGenerate()
    {
        Set_BlockGenerateDatas();

        for (int i = 0; i < initialGenerateCount; i++)
        {
            GenerateBlock();
        }
        timer = 0f;
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
        unlockBlockIndexes = new int[2] { 0, 1 };
        foreach (var index in unlockBlockIndexes)
        {
            var data = SOLoader.BlockData.GetBlockData(index);
            var blockData = new GenerateBlockData
            {
                so = data,
                generateRate = data.generateRate
            };
            blockData.generateRate += 0f; // TODO : ブロックの生成率を加算
            list_generateBlockDatas.Add(blockData);
        }
    }

    private void GenerateBlock()
    {
        var so = Get_RandomBlockData();
        var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == so.blockIndex);
        if (targetBlock == null)
        {
            var newBlock = Instantiate(so.pf, parentPool) as GameObject;
            targetBlock = newBlock.GetComponent<MiningTargetBase>();
            list_targetBlocks.Add(targetBlock);
        }
        targetBlock.transform.position = GetRandomPosition();
        targetBlock.transform.rotation = Quaternion.identity;
        targetBlock.Init(so);
    }

    private BlockData Get_RandomBlockData()
    {
        var totalRate = 0f;
        foreach (var data in list_generateBlockDatas)
        {
            totalRate += data.generateRate;
        }
        var randomValue = Random.Range(0f, totalRate);
        var currentRate = 0f;
        foreach (var data in list_generateBlockDatas)
        {
            currentRate += data.generateRate;
            if (randomValue <= currentRate)
            {
                return data.so;
            }
        }
        return null;
    }
    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(range_x.x, range_x.y), Random.Range(range_y.x, range_y.y), Random.Range(range_z.x, range_z.y));
    }




    #endregion

}
