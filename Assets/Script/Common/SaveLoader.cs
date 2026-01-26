using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Numerics;
using System;
using System.Linq;



[System.Serializable]
public class SkillTreeData
{
    public string key = "";
    public int level = 0;
}


[System.Serializable]
public class ItemData
{
    public string key = ""; //不要だが一旦保持
    public int count = 0;

    // -- for future use --
    //
    //
}

[System.Serializable]
public class ArtifactData
{
    public int artifactIndex;
    public int level = 1;
}


public enum state
{
    InitialLoad, Idling, Doing
}


public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Inst;
    public state currentState { get; private set; } = state.InitialLoad;


    private string KEY_CREATE_INITIAL_DATA = "key_createInitialData";

    private const string KEY_COIN = "key_coin";
    private BigInteger coin;
    public BigInteger Coin { get => coin; }

    private const string KEY_UNLOCK_EVENTINDEX = "key_unlockEventIndex";
    private int unlockEventIndex;
    public int UnlockEventIndex { get => unlockEventIndex; }



    #region -- result param --
    private const string KEY_BLOCKCOUNT = "key_blockCount";
    private int blockCount;
    public int BlockCount { get => blockCount; }

    private const string KEY_INGAME_COUNT = "key_ingameCount";
    private int ingameCount;
    public int IngameCount { get => ingameCount; }

    private const string KEY_PLAYER_LEVEL = "key_playerLevel";
    private int playerLevel;
    public int PlayerLevel { get => playerLevel; }

    private const string KEY_PLAYER_EXP = "key_playerExp";
    private int playerExp;
    public int PlayerExp { get => playerExp; }
    #endregion



    private Queue<Action> allQueue = new();
    private bool isProcessingQueue = false;



    void Awake()
    {
        if (Inst == null) Inst = this;
        else { Destroy(this); }
    }


    async void Start()
    {
        currentState = state.InitialLoad;

        // initial load 
        await SaveData_InitialLoad();
    }



    private async UniTask SaveData_InitialLoad()
    {
        var createdInitialData = ES3.KeyExists(KEY_CREATE_INITIAL_DATA);
        if (!createdInitialData)
        {
            InitialData_Create();
            ES3.Save(KEY_CREATE_INITIAL_DATA, true);
        }

        // === Initial Load Data
        coin = ES3.KeyExists(KEY_COIN) ? BigInteger.Parse(ES3.Load<string>(KEY_COIN)) : 0;
        unlockEventIndex = ES3.KeyExists(KEY_UNLOCK_EVENTINDEX) ? ES3.Load<int>(KEY_UNLOCK_EVENTINDEX) : 1;
        blockCount = ES3.KeyExists(KEY_BLOCKCOUNT) ? ES3.Load<int>(KEY_BLOCKCOUNT) : 0;
        ingameCount = ES3.KeyExists(KEY_INGAME_COUNT) ? ES3.Load<int>(KEY_INGAME_COUNT) : 0;
        playerLevel = ES3.KeyExists(KEY_PLAYER_LEVEL) ? ES3.Load<int>(KEY_PLAYER_LEVEL) : 0;
        playerExp = ES3.KeyExists(KEY_PLAYER_EXP) ? ES3.Load<int>(KEY_PLAYER_EXP) : 0;

        currentState = state.Idling;
    }

    private void InitialData_Create()
    {


    }


    /// <summary>
    /// データのロード、順番を保証
    /// </summary>
    public UniTask<(bool success, T data)> LoadAsync<T>(string key)
    {
        var tcs = new UniTaskCompletionSource<(bool, T)>();

        if (string.IsNullOrEmpty(key))
        {
            tcs.TrySetResult((false, default));
            return tcs.Task;
        }

        EnqueueMethod(async () =>
        {
            await UniTask.Yield();
            try
            {
                if (ES3.KeyExists(key))
                {
                    var data = ES3.Load<T>(key);
                    tcs.TrySetResult((true, data));
                }
                else
                {
                    tcs.TrySetResult((false, default));
                }
            }
            catch
            {
                tcs.TrySetResult((false, default));
            }
        });
        return tcs.Task;
    }




    void Update()
    {
        //処理中またはキューにアイテムがない場合、何もしない
        if (isProcessingQueue || allQueue.Count <= 0) return;
        ProcessSaveQueue();
    }

    private void EnqueueMethod(Action saveAction)
    {
        allQueue.Enqueue(saveAction);
    }
    private void ProcessSaveQueue()
    {
        isProcessingQueue = true;

        var action = allQueue.Dequeue();
        action.Invoke();
        //await UniTask.DelayFrame(1);// 1フレーム待機（競合防止

        isProcessingQueue = false;
    }




    #region -- coin data --
    /// <summary>
    /// コインセーブリクエスト - デルタを加算してセーブ
    /// </summary>
    public void Request_SaveCoin(BigInteger _delta)
    {
        EnqueueMethod(() => { SavePendeingCoin(_delta); });
    }
    private void SavePendeingCoin(BigInteger _delta)
    {
        coin += _delta;
        ES3.Save(KEY_COIN, coin.ToString());
        GameEvent.UI.PublishCoinMod(coin);
    }
    #endregion


    #region -- unlock event index data --
    /// <summary>
    /// イベントインデックスセーブリクエスト
    /// </summary>
    public void Request_SaveUnlockEventIndex(int _index)
    {
        EnqueueMethod(() => { SaveUnlockEventIndex(_index); });
    }
    private void SaveUnlockEventIndex(int _index)
    {
        unlockEventIndex = _index;
        ES3.Save(KEY_UNLOCK_EVENTINDEX, unlockEventIndex);
    }
    #endregion


    #region -- Ingame result data --
    /// <summary>
    /// ブロック破壊カウントセーブリクエスト
    /// </summary>
    public void Request_SaveBlockBreakCount(int _count)
    {
        EnqueueMethod(() => { SaveBlockBreakCount(_count); });
    }
    private void SaveBlockBreakCount(int _count)
    {
        blockCount += _count;
        ES3.Save(KEY_BLOCKCOUNT, blockCount);
    }
    /// <summary>
    /// インゲームプレイカウント
    /// </summary>
    public void Request_SaveIngameCount(int _count)
    {
        EnqueueMethod(() => { SaveIngameCount(_count); });
    }
    private void SaveIngameCount(int _count)
    {
        ingameCount += _count;
        ES3.Save(KEY_INGAME_COUNT, ingameCount);
    }
    /// <summary>
    /// プレイヤーレベルセーブリクエスト
    /// </summary>
    public void Request_SavePlayerLevel(int _level)
    {
        EnqueueMethod(() => { SavePlayerLevel(_level); });
    }
    private void SavePlayerLevel(int _level)
    {
        playerLevel += _level;
        ES3.Save(KEY_PLAYER_LEVEL, playerLevel);
    }
    /// <summary>
    /// プレイヤー経験値セーブリクエスト
    /// </summary>
    public void Request_SavePlayerExp(int _exp)
    {
        EnqueueMethod(() => { SavePlayerExp(_exp); });
    }
    private void SavePlayerExp(int _exp)
    {
        playerExp += _exp;
        ES3.Save(KEY_PLAYER_EXP, playerExp);
    }
    #endregion





    #region -- SkillTree --
    public async UniTask<SkillTreeData> Get_SkillTreeData(int _skillIndex)
    {
        string saveKey = GetSkillTreeDataKey(_skillIndex);
        var loadData = await LoadAsync<SkillTreeData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SaveSkillTreeData(int _skillIndex, int _level)
    {
        EnqueueMethod(() => { SaveSkillTreeData(_skillIndex, _level); });
    }
    private void SaveSkillTreeData(int _skillIndex, int _level)
    {
        var saveKey = GetSkillTreeDataKey(_skillIndex);
        var newData = new SkillTreeData()
        {
            key = saveKey,
            level = _level
        };
        ES3.Save(saveKey, newData);
    }
    private string GetSkillTreeDataKey(int _skillIndex)
    {
        return $"SkillTreeData-{_skillIndex}";
    }
    #endregion




    #region -- Item --
    public async UniTask<ItemData> Get_ItemData(int _itemIndex)
    {
        string saveKey = GetItemDataKey(_itemIndex);
        var loadData = await LoadAsync<ItemData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SaveItemData(int _itemIndex, int _count)
    {
        EnqueueMethod(() => { SaveItemData(_itemIndex, _count); });
    }
    private void SaveItemData(int _itemIndex, int _count)
    {
        var saveKey = GetItemDataKey(_itemIndex);
        var newData = new ItemData()
        {
            key = saveKey,
            count = _count
        };
        ES3.Save(saveKey, newData);
    }
    private string GetItemDataKey(int _itemIndex)
    {
        return $"item-{_itemIndex}";
    }
    #endregion




    #region -- Artifact --
    public async UniTask<ArtifactData> Get_ArtifactData(int _artifactIndex)
    {
        string saveKey = GetArtifactDataKey(_artifactIndex);
        var loadData = await LoadAsync<ArtifactData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SaveArtifactData(int _artifactIndex, int _level)
    {
        EnqueueMethod(() => { SaveArtifactData(_artifactIndex, _level); });
    }
    private void SaveArtifactData(int _artifactIndex, int _level)
    {
        var saveKey = GetArtifactDataKey(_artifactIndex);
        var newData = new ArtifactData()
        {
            artifactIndex = _artifactIndex,
            level = _level
        };
        ES3.Save(saveKey, newData);
    }
    private string GetArtifactDataKey(int _artifactIndex)
    {
        return $"ArtifactData-{_artifactIndex}";
    }
    #endregion




}
