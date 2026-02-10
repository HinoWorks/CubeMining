using UnityEngine;
using UniRx;
using System.Numerics;
using System.Collections.Generic;



public class ResourceData
{
    public ResourceType resourceType;
    public BigInteger resourceCount;
}


public class InGameManager : MonoBehaviour
{
    public static InGameManager Inst;
    [SerializeField] Transform parentPool;
    public Transform ParentPool => parentPool;


    private float timer = 0;
    private float timeLimit => GameParamManager.gameBaseParam.ingameTime + exTime;
    private float exTime = 0f;
    private BigInteger getCoin;
    private List<ResourceData> resourceDataList = new List<ResourceData>();


    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);
    }



    private void SetGameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.InGame_Ready:
                AttackManager.Inst.Set_Ready();
                BlockGenerateManager.Inst.Init();
                //getCoin = 0;
                resourceDataList.Clear();
                exTime = 0f;
                GameEvent.UI.PublishCoinMod(getCoin);
                GameEvent.UI.PublishTimeLimit(timeLimit);
                break;
            case GameStateType.InGame:
                timer = 0;
                AttackManager.Inst.Set_AttackState(true);
                BlockGenerateManager.Inst.Set_GenerateState(true);
                break;
            case GameStateType.InGame_End:
                AttackManager.Inst.Set_AttackState(false);
                BlockGenerateManager.Inst.Set_GenerateState(false);
                break;
            case GameStateType.Result:
                AttackManager.Inst.AttackUnitDelete();
                break;
            case GameStateType.OutGame:
                BlockGenerateManager.Inst.ResetAllBlocks();
                Save_IngameResult();
                break;
        }
    }

    void Update()
    {
        if (!GameWatcher.Inst.isInGameNow) return;
        timer += Time.deltaTime;
        GameEvent.UI.PublishTimeLimit(timeLimit - timer);
        if (timer >= timeLimit)
        {
            GameEvent.UI.PublishTimeLimit(0f);
            GameWatcher.Inst.SetGameState(GameStateType.InGame_End);
        }
    }

    public void AddGetCoin(BigInteger _deltaCoin)
    {
        getCoin += _deltaCoin;
        GameEvent.UI.PublishCoinMod(getCoin);
    }

    public void AddGetResource(ResourceType _resourceType, BigInteger _deltaResource)
    {
        var targetData = resourceDataList.Find(d => d.resourceType == _resourceType);
        if (targetData == null)
        {
            targetData = new ResourceData()
            {
                resourceType = _resourceType,
                resourceCount = 0
            };
            resourceDataList.Add(targetData);
        }
        targetData.resourceCount += _deltaResource;
        GameEvent.UI.PublishResourceMod(_resourceType, targetData.resourceCount);
        //Debug.Log($"AddGetResource: {_resourceType} {targetData.resourceCount}");
    }

    public void AddGetExTime(float _deltaExTime)
    {
        exTime += _deltaExTime;
        GameEvent.UI.PublishTimeLimit(timeLimit);
        GameEvent.UI.PublishTimeLimit(timeLimit - timer);
    }


    private void Save_IngameResult()
    {
        SaveLoader.Inst.Request_SaveCoin(getCoin);
    }


}
