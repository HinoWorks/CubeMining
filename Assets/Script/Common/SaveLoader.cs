using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Numerics;
using System;
using System.Linq;


public static class InitialCreateData
{
    public static int skillTreeIndex_first = 1;

}



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



public enum state
{
    InitialLoad, Idling, Doing
}


public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Inst;
    private string key_playerSaveData = "key_playerSaveData";
    public state currentState { get; private set; } = state.InitialLoad;



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
        var createdInitialData = ES3.KeyExists(StaticManager.KEY_CREATE_INITIAL_DATA);
        if (!createdInitialData)
        {
            InitialData_Create();
            ES3.Save(StaticManager.KEY_CREATE_INITIAL_DATA, true);
        }

        // Initial Load Data
        coin = ES3.KeyExists(KEY_COIN) ? BigInteger.Parse(ES3.Load<string>(KEY_COIN)) : 0;

        //blockCount = ES3.KeyExists(KEY_BLOCKCOUNT) ? ES3.Load<int>(KEY_BLOCKCOUNT) : 0;
        //GoalCount = ES3.KeyExists(KEY_GoalCount) ? ES3.Load<int>(KEY_GoalCount) : 0;
        //GoalLevel = ES3.KeyExists(KEY_GoalLevel) ? ES3.Load<int>(KEY_GoalLevel) : 1;

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
