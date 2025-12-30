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
    public string key = ""; //不要だが一旦保持
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
public class BallCountData
{
    public int count;
    public int level = 0;
}


public enum state
{
    InitialLoad, Idling, Doing
}


public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Inst;
    private string key_playerSaveData = "key_playerSaveData";
    public state currentState { get; private set; } = state.InitialLoad;



    private const string KEY_GoalCount = "key_GoalCount";
    private int goalCount;
    public int GoalCount
    {
        get => goalCount;
        private set
        {
            goalCount = value;
            ES3.Save(KEY_GoalCount, goalCount);
        }
    }

    private const string KEY_GoalLevel = "key_GoalLevel";
    private int goalLevel;
    public int GoalLevel
    {
        get => goalLevel;
        private set
        {
            goalLevel = value;
            ES3.Save(KEY_GoalLevel, goalLevel);
        }
    }


    private const string KEY_COIN = "key_coin";
    private BigInteger coin;
    public BigInteger Coin
    {
        get => coin;
    }



    private const string KEY_BLOCKCOUNT = "key_blockCount";
    private int blockCount;
    public int BlockCount
    {
        get => blockCount;
        private set
        {
            blockCount = value;
            ES3.Save(KEY_BLOCKCOUNT, blockCount);
        }
    }

    private Queue<Action> allQueue = new();
    private bool isProcessingQueue = false;


    // -- auto save --
    private float saveDelay_coin = 1f; // 1秒後に保存
    private float saveTimer_coin = -1f;
    private bool isSavePending_coin = false;
    private BigInteger pendingCoin = 0;



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
        // --- Load --
        blockCount = ES3.KeyExists(KEY_BLOCKCOUNT) ? ES3.Load<int>(KEY_BLOCKCOUNT) : 0;
        coin = ES3.KeyExists(KEY_COIN) ? BigInteger.Parse(ES3.Load<string>(KEY_COIN)) : 0;
        GoalCount = ES3.KeyExists(KEY_GoalCount) ? ES3.Load<int>(KEY_GoalCount) : 0;
        GoalLevel = ES3.KeyExists(KEY_GoalLevel) ? ES3.Load<int>(KEY_GoalLevel) : 1;


        // -- Initial set --
        if (blockCount == 0)
        {
            InitialData_Create();
        }

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
        /*
        // -- coin --
        if (isSavePending_coin)
        {
            saveTimer_coin -= Time.deltaTime;
            if (saveTimer_coin <= 0f)
            {
                EnqueueMethod(SavePendeingCoin);
                isSavePending_coin = false;
            }
        }
        */

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
    /// コインセーブリクエスト - デルタ
    /// </summary>
    /// <param name="_delta"></param>
    public void Request_SaveCoin(BigInteger _delta)
    {
        pendingCoin += _delta;
        saveTimer_coin = saveDelay_coin;
        isSavePending_coin = true;
    }
    private void SavePendeingCoin()
    {
        coin += pendingCoin;
        pendingCoin = 0;

        ES3.Save(KEY_COIN, coin.ToString());

        GameEvent.UI.PublishCoinMod(coin);
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





}
