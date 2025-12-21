using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class BlockGenerateManager : MonoBehaviour
{
    public static BlockGenerateManager Inst;

    [SerializeField] private GameObject pf_block;
    [SerializeField] Transform parentPool;
    [SerializeField] Vector2 range_x;
    [SerializeField] Vector2 range_y;
    [SerializeField] Vector2 range_z;
    private List<MiningTargetBase> list_targetBlocks = new List<MiningTargetBase>();

    // -- loc
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


    private void GenerateBlock()
    {
        var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false);
        if (targetBlock == null)
        {
            var newBlock = Instantiate(pf_block, parentPool) as GameObject;
            targetBlock = newBlock.GetComponent<MiningTargetBase>();
            list_targetBlocks.Add(targetBlock);
        }
        targetBlock.transform.position = GetRandomPosition();
        targetBlock.transform.rotation = Quaternion.identity;
        targetBlock.Init();
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(range_x.x, range_x.y), Random.Range(range_y.x, range_y.y), Random.Range(range_z.x, range_z.y));
    }


}
