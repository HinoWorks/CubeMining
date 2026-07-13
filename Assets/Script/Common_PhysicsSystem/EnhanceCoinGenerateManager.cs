using UnityEngine;
using System.Collections.Generic;

public class EnhanceCoinGenerateManager : MonoBehaviour
{
    public static EnhanceCoinGenerateManager Inst;

    private List<MiningTarget_EnhanceCoin> list_targetEnhanceCoins = new List<MiningTarget_EnhanceCoin>();
    public bool isGenerateEnhanceCoin { get; private set; } = false;
    private float enhanceCoinGenerateRate = 0f;



    // GameCounter - ゲーム終了時、にリセットされる
    private int gameCounter_inGame = 0; // ゲーム起動中にインゲームを開始した回数、
    private int gameCounter_inGame_noCoin = 0; // ゲーム起動中にインゲームを開始した回数、コインを獲得していない回数


    // １ゲーム中の最大の生成数
    private float timer = 0f;
    private float createTime;
    private float Set_CreateTime => Random.Range(GameParamManager.gameBaseParam.ingameTime * 0.3f, GameParamManager.gameBaseParam.ingameTime * 0.7f);
    private int createCount_thisGame = 0;
    private bool isGenerateFin = true;



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }


    public void Init()
    {
        createCount_thisGame = 0;
        isGenerateFin = true;

        if (!UnlockStateManager.Inst.isUnlock_PickaxePower) return;

        Set_GenerateCheck();
    }

    private void Set_GenerateCheck()
    {
        gameCounter_inGame++;
        gameCounter_inGame_noCoin++;

        var currentTotalCoin = SaveLoader.Inst.EnhanceCoinCount_Total;
        var so = SOLoader.EnhanceCoinData.Get_EnhanceCoinRateData(currentTotalCoin);
        enhanceCoinGenerateRate = so.baseRate + so.deltaRate * (gameCounter_inGame_noCoin - 1);

        var random = UnityEngine.Random.Range(0f, 1f);
        Debug.Log($"<color=green> == EnhanceCoin ==  totalGetCoin:{currentTotalCoin} / inGameCount-NoCoin:{gameCounter_inGame_noCoin} / so.base:{so.baseRate} / so.delta:{so.deltaRate} => GenerateRate: {enhanceCoinGenerateRate} => isGenerate???: {random < enhanceCoinGenerateRate}</color>");
        if (random >= enhanceCoinGenerateRate) return;

        // set Generate Parameter
        createCount_thisGame = Random.Range(1, so.createMax + 1);
        createTime = Set_CreateTime;
        isGenerateFin = false;
        Debug.Log($"<color=green> == EnhanceCoin ==   createCount_thisGame:{createCount_thisGame} / createTime:{createTime}</color>");
    }

    public void ResetAll()
    {
        foreach (var targetEnhanceCoin in list_targetEnhanceCoins)
        {
            targetEnhanceCoin.NotActivate();
        }
    }

    public void UnityUpDate()
    {
        if (isGenerateFin) return;
        if (createCount_thisGame <= 0) return;

        timer += Time.deltaTime;
        if (timer < createTime) return;

        timer = 0f;
        EnhanceCoinGenerate();
    }

    private void EnhanceCoinGenerate()
    {
        for (int i = 0; i < createCount_thisGame; i++)
        {
            Generate(BlockGenerateManager.Inst.generatePosition, Quaternion.Euler(BlockGenerateManager.Inst.generateRotation));
        }
        gameCounter_inGame_noCoin = 0;
        isGenerateFin = true;
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
