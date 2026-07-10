using UnityEngine;
using System.Collections.Generic;

public class EnhanceCoinGenerateManager : MonoBehaviour
{
    public static EnhanceCoinGenerateManager Inst;

    private List<MiningTarget_EnhanceCoin> list_targetEnhanceCoins = new List<MiningTarget_EnhanceCoin>();
    public bool isGenerateEnhanceCoin { get; private set; } = false;
    private float enhanceCoinGenerateRate = 0.25f;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }



    public void Init()
    {
        isGenerateEnhanceCoin = false;
    }

    public void ResetAll()
    {
        foreach (var targetEnhanceCoin in list_targetEnhanceCoins)
        {
            targetEnhanceCoin.NotActivate();
        }
    }

    public void Check_EnhanceCoinGenerate()
    {
        if (ShouldGenerate())
        {
            Generate(BlockGenerateManager.Inst.generatePosition, Quaternion.Euler(BlockGenerateManager.Inst.generateRotation));
        }
    }

    private bool ShouldGenerate()
    {
        //return !isGenerateEnhanceCoin && GameParamManager.IsEnhanceCoinGenerate();

        var random = UnityEngine.Random.Range(0f, 1f);
        return random < enhanceCoinGenerateRate;
    }

    private MiningTarget_EnhanceCoin Generate(Vector3 position, Quaternion rotation)
    {
        var targetEnhanceCoin = list_targetEnhanceCoins.Find(x => x.isActiveAndEnabled == false);
        if (targetEnhanceCoin == null)
        {
            var newEnhanceCoin = Instantiate(SOLoader.BlockData.pf_EnhanceCoin, InGameManager.Inst.ParentPool) as GameObject;
            targetEnhanceCoin = newEnhanceCoin.GetComponent<MiningTarget_EnhanceCoin>();
            list_targetEnhanceCoins.Add(targetEnhanceCoin);
        }

        targetEnhanceCoin.Init();
        targetEnhanceCoin.transform.localPosition = position;
        targetEnhanceCoin.transform.localRotation = rotation;
        isGenerateEnhanceCoin = true;
        return targetEnhanceCoin;
    }
}
