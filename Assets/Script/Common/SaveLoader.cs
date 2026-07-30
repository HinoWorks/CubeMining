using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Numerics;
using System;
using System.Linq;


#region -- GameRecordData --
[System.Serializable]
public class GameRecordData
{
    // -- total record --
    public BigInteger total_ingameCount = 0;
    public BigInteger total_blockBreakCount = 0;
    public BigInteger total_playerLevel = 1;
    public BigInteger total_treasureCount = 0;
    public BigInteger total_playerExp = 0;
    public BigInteger total_totalDamage = 0;
    public BigInteger total_skillTreeCount = 0;
    public BigInteger total_artifactCount = 0;
    public BigInteger total_pickaxeCount = 0;
    // -- one game record --
    public BigInteger oneGame_blockBreakCount = 0;
    public BigInteger oneGame_treasureCount = 0;
    public BigInteger oneGame_playerExp = 0;
    public BigInteger oneGame_totalDamage = 0;
    public BigInteger oneGame_maxDepth = 0;
}

/// <summary>
/// GameRecordDataのセーブ用（BigIntegerをstringに変換）
/// </summary>
[System.Serializable]
public class GameRecordDataSave
{
    public string total_ingameCount;
    public string total_blockBreakCount;
    public string total_treasureCount;
    public string total_playerExp;
    public string total_totalDamage;
    public string total_skillTreeCount;
    public string total_artifactCount;
    public string total_pickaxeCount;

    public string oneGame_blockBreakCount;
    public string oneGame_treasureCount;
    public string oneGame_playerExp;
    public string oneGame_totalDamage;
    public string oneGame_maxDepth;
}
#endregion



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
}

#region -- User Settings --
[System.Serializable]
public class UserSettingsData
{
    public float volumeMaster = 100f;
    public float volumeBGM = 80f;
    public float volumeSE = 80f;
    public bool muteMaster;
    public bool muteBGM;
    public bool muteSE;

    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public int fullScreenMode = (int)FullScreenMode.Windowed;

    public UserSettingsData Copy()
    {
        return new UserSettingsData
        {
            volumeMaster = volumeMaster,
            volumeBGM = volumeBGM,
            volumeSE = volumeSE,
            muteMaster = muteMaster,
            muteBGM = muteBGM,
            muteSE = muteSE,
            resolutionWidth = resolutionWidth,
            resolutionHeight = resolutionHeight,
            fullScreenMode = fullScreenMode,
        };
    }
}

/// <summary>旧フォーマット（key_soundSettings からの移行用）</summary>
[System.Serializable]
public class SoundSettingsData
{
    public float volumeBGM = 80f;
    public float volumeSE = 80f;
    public bool muteBGM;
    public bool muteSE;
}
#endregion


#region -- Artifact --
[System.Serializable]
public class ArtifactData
{
    public int artifactIndex;
    public int level = 1;
    //public int equipSlotIndex = -1;
}
[System.Serializable]
public class ArtifactSlotData
{
    public int slotIndex;
    public bool isOpen = false;
    public int equipedArtifactIndex;
}
#endregion



#region -- Pickaxe --
[System.Serializable]
public class PickaxeData
{
    public int pickaxeIndex;
    public int level = 0;
}
[System.Serializable]
public class PickaxeSlotData
{
    public int slotIndex;
    public int equipedPickaxeIndex;
}
#endregion


#region -- PlayerLevel --
[System.Serializable]
public class PlayerLevelData
{
    public int level = 1;
    public BigInteger totalExp = 0;
    public BigInteger expInCurrentLevel = 0;
    public int points = 0;
}
#endregion


#region -- PickaxePower --
[System.Serializable]
public class PickaxePowerData
{
    public int pickaxePowerIndex;
    public int level = 0;
}
#endregion




public enum state
{
    InitialLoad, Idling, Doing
}


public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Inst;
    public state currentState { get; private set; } = state.InitialLoad;


    private string KEY_CREATE_INITIAL_DATA = "key_createInitialData"; // 初期データ作成フラグ
    private string KEY_GAME_RECORD_DATA = "key_gameRecordData"; // ゲーム記録データ

    /*
        private const string KEY_COIN = "key_coin";
        private BigInteger coin;
        public BigInteger Coin { get => coin; }
    */



    private const string KEY_ENHANCE_COIN_COUNT = "key_enhanceCoinCount";
    private int enhanceCoinCount;
    public int EnhanceCoinCount { get => enhanceCoinCount; }
    private const string KEY_ENHANCE_COIN_COUNT_TOTAL = "key_enhanceCoinCountTotal";
    public int EnhanceCoinCount_Total { get => enhanceCoinCount_Total; }
    private int enhanceCoinCount_Total = 0;


    #region -- resource data --
    private const string KEY_RESOURCE_STONE = "key_resource_stone";
    private BigInteger resourceStone;
    private const string KEY_RESOURCE_IRON = "key_resource_iron";
    private BigInteger resourceIron;
    private const string KEY_RESOURCE_GOLD = "key_resource_gold";
    private BigInteger resourceGold;
    private const string KEY_RESOURCE_EMERALD = "key_resource_emerald";
    private BigInteger resourceEmerald;
    private const string KEY_RESOURCE_RUBY = "key_resource_ruby";
    private BigInteger resourceRuby;
    private const string KEY_RESOURCE_SAPPHIRE = "key_resource_sapphire";
    private BigInteger resourceSapphire;
    private const string KEY_RESOURCE_DIAMOND = "key_resource_diamond";
    private BigInteger resourceDiamond;
    public BigInteger Get_ResourceCount(ResourceType _resourceType)
    {
        switch (_resourceType)
        {
            case ResourceType.Stone: return resourceStone;
            case ResourceType.Iron: return resourceIron;
            case ResourceType.Gold: return resourceGold;
            case ResourceType.Emerald: return resourceEmerald;
            case ResourceType.Ruby: return resourceRuby;
            case ResourceType.Sapphire: return resourceSapphire;
            case ResourceType.Diamond: return resourceDiamond;
            default: return 0;
        }
    }
    #endregion



    private const string KEY_ARTIFACT_CURRENTBLOCKCOUNT = "key_artifactCurrentBlockCount"; // アーティファクト用生成ブロック数
    private int artifactCurrentBlockCount;
    public int ArtifactCurrentBlockCount { get => artifactCurrentBlockCount; }



    private const string KEY_PICKAXEPOWER_EQUIPEDINDEX = "key_pickaxePowerEquipedIndex"; // ピッケルパワー装備インデックス
    private int pickaxePowerEquipedIndex = 0;
    public int PickaxePowerEquipedIndex { get => pickaxePowerEquipedIndex; }


    private const string KEY_USER_SETTINGS = "key_userSettings";

    private const string KEY_TUTORIAL_SHOWN = "key_tutorialShown";
    private List<int> tutorialShownIndices = new();


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
        //coin = ES3.KeyExists(KEY_COIN) ? BigInteger.Parse(ES3.Load<string>(KEY_COIN)) : 0;
        resourceStone = ES3.KeyExists(KEY_RESOURCE_STONE) ? BigInteger.Parse(ES3.Load<string>(KEY_RESOURCE_STONE)) : 0;
        resourceIron = ES3.KeyExists(KEY_RESOURCE_IRON) ? BigInteger.Parse(ES3.Load<string>(KEY_RESOURCE_IRON)) : 0;
        resourceGold = ES3.KeyExists(KEY_RESOURCE_GOLD) ? BigInteger.Parse(ES3.Load<string>(KEY_RESOURCE_GOLD)) : 0;
        resourceEmerald = ES3.KeyExists(KEY_RESOURCE_EMERALD) ? BigInteger.Parse(ES3.Load<string>(KEY_RESOURCE_EMERALD)) : 0;
        resourceRuby = ES3.KeyExists(KEY_RESOURCE_RUBY) ? BigInteger.Parse(ES3.Load<string>(KEY_RESOURCE_RUBY)) : 0;
        resourceSapphire = ES3.KeyExists(KEY_RESOURCE_SAPPHIRE) ? BigInteger.Parse(ES3.Load<string>(KEY_RESOURCE_SAPPHIRE)) : 0;
        resourceDiamond = ES3.KeyExists(KEY_RESOURCE_DIAMOND) ? BigInteger.Parse(ES3.Load<string>(KEY_RESOURCE_DIAMOND)) : 0;

        artifactCurrentBlockCount = ES3.KeyExists(KEY_ARTIFACT_CURRENTBLOCKCOUNT) ? ES3.Load<int>(KEY_ARTIFACT_CURRENTBLOCKCOUNT) : 0;
        pickaxePowerEquipedIndex = ES3.KeyExists(KEY_PICKAXEPOWER_EQUIPEDINDEX) ? ES3.Load<int>(KEY_PICKAXEPOWER_EQUIPEDINDEX) : 0;

        enhanceCoinCount = ES3.KeyExists(KEY_ENHANCE_COIN_COUNT) ? ES3.Load<int>(KEY_ENHANCE_COIN_COUNT) : 0;
        enhanceCoinCount_Total = ES3.KeyExists(KEY_ENHANCE_COIN_COUNT_TOTAL) ? ES3.Load<int>(KEY_ENHANCE_COIN_COUNT_TOTAL) : 0;

        tutorialShownIndices = ES3.KeyExists(KEY_TUTORIAL_SHOWN)
            ? ES3.Load<List<int>>(KEY_TUTORIAL_SHOWN)
            : new List<int>();

        currentState = state.Idling;
        Debug.Log($" == SaveData_InitialLoad: End == ");
    }


    private void InitialData_Create()
    {
        // 初期データ作成
        Request_SavePickaxeData(1, 1); // 初期つるはしゲット
        Request_SavePickaxeSlotData(0, 1); // 初期スロット0番目に装備

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



    /*
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
    */



    #region -- resource data --
    /// <summary>
    /// リソースセーブリクエスト - デルタを加算してセーブ
    /// </summary>
    public void Request_SaveResource(ResourceType _resourceType, BigInteger _delta)
    {
        EnqueueMethod(() => { SavePendeingResource(_resourceType, _delta); });
    }
    private void SavePendeingResource(ResourceType _resourceType, BigInteger _delta)
    {
        ref var value = ref GetResourceRef(_resourceType);
        value += _delta;
        ES3.Save(GetResourceKey(_resourceType), value.ToString());
        GameEvent.UI.PublishResourceMod(_resourceType, value);
    }
    private static string GetResourceKey(ResourceType _resourceType)
    {
        switch (_resourceType)
        {
            case ResourceType.Stone: return KEY_RESOURCE_STONE;
            case ResourceType.Iron: return KEY_RESOURCE_IRON;
            case ResourceType.Gold: return KEY_RESOURCE_GOLD;
            case ResourceType.Emerald: return KEY_RESOURCE_EMERALD;
            case ResourceType.Ruby: return KEY_RESOURCE_RUBY;
            case ResourceType.Sapphire: return KEY_RESOURCE_SAPPHIRE;
            case ResourceType.Diamond: return KEY_RESOURCE_DIAMOND;
            default: return null;
        }
    }
    private ref BigInteger GetResourceRef(ResourceType _resourceType)
    {
        switch (_resourceType)
        {
            case ResourceType.Stone: return ref resourceStone;
            case ResourceType.Iron: return ref resourceIron;
            case ResourceType.Gold: return ref resourceGold;
            case ResourceType.Emerald: return ref resourceEmerald;
            case ResourceType.Ruby: return ref resourceRuby;
            case ResourceType.Sapphire: return ref resourceSapphire;
            case ResourceType.Diamond: return ref resourceDiamond;
            default: throw new System.ArgumentOutOfRangeException(nameof(_resourceType));
        }
    }

    public bool Check_ResourceKeyExists(ResourceType _resourceType)
    {
        var key = GetResourceKey(_resourceType);
        return key != null && ES3.KeyExists(key);
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
        //unlockEventIndex = _index;
        // ES3.Save(KEY_UNLOCK_EVENTINDEX, unlockEventIndex);
    }
    #endregion


    #region -- Tutorial --
    public bool IsTutorialShown(int _index)
    {
        return tutorialShownIndices != null && tutorialShownIndices.Contains(_index);
    }

    public void Request_SaveTutorialShown(int _index)
    {
        EnqueueMethod(() => { SaveTutorialShown(_index); });
    }

    private void SaveTutorialShown(int _index)
    {
        if (tutorialShownIndices == null) tutorialShownIndices = new List<int>();
        if (tutorialShownIndices.Contains(_index)) return;

        tutorialShownIndices.Add(_index);
        ES3.Save(KEY_TUTORIAL_SHOWN, tutorialShownIndices);
    }
    #endregion




    #region -- EnhanceCoin --
    public void Request_SaveEnhanceCoinCount(int _deltaCount)
    {
        EnqueueMethod(() => { SaveEnhanceCoinCount(_deltaCount); });
    }
    private void SaveEnhanceCoinCount(int _deltaCount)
    {
        enhanceCoinCount += _deltaCount;
        ES3.Save(KEY_ENHANCE_COIN_COUNT, enhanceCoinCount);

        if (_deltaCount > 0)
        {
            enhanceCoinCount_Total += _deltaCount;
            ES3.Save(KEY_ENHANCE_COIN_COUNT_TOTAL, enhanceCoinCount_Total);
        }
    }
    #endregion



    #region -- Ingame result data --
    #endregion





    #region -- PlayerLevel --
    public async UniTask<PlayerLevelData> Get_PlayerLevelData()
    {
        var saveKey = GetPlayerLevelDataKey();
        var loadData = await LoadAsync<PlayerLevelData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SavePlayerLevelData(PlayerLevelData _playerLevelData)
    {
        EnqueueMethod(() => { SavePlayerLevelData(_playerLevelData); });
    }
    private void SavePlayerLevelData(PlayerLevelData _playerLevelData)
    {
        var saveKey = GetPlayerLevelDataKey();
        ES3.Save(saveKey, _playerLevelData);
    }
    /// <summary>
    /// ポイントセーブリクエスト - デルタを加算してセーブ
    /// </summary>
    public void Request_SavePlayerLevelData(int _deltaPoints)
    {
        EnqueueMethod(async () => { await SavePlayerLevelData(_deltaPoints); });
    }
    private async UniTask SavePlayerLevelData(int _deltaPoints)
    {
        var saveKey = GetPlayerLevelDataKey();
        var currentData = await LoadAsync<PlayerLevelData>(saveKey);
        if (currentData.success)
        {
            currentData.data.points += _deltaPoints;
            ES3.Save(saveKey, currentData.data);
        }
    }
    private string GetPlayerLevelDataKey()
    {
        return $"PlayerLevelData";
    }
    #endregion




    #region -- SkillTree --
    private readonly Dictionary<int, SkillTreeData?> skillTreeDataCache = new();

    public async UniTask<SkillTreeData> Get_SkillTreeData(int _skillIndex)
    {
        if (skillTreeDataCache.TryGetValue(_skillIndex, out var cached))
        {
            return cached;
        }

        string saveKey = GetSkillTreeDataKey(_skillIndex);
        var loadData = await LoadAsync<SkillTreeData>(saveKey);
        if (loadData.success)
        {
            skillTreeDataCache[_skillIndex] = loadData.data;
            return loadData.data;
        }
        skillTreeDataCache[_skillIndex] = null;
        return null;
    }
    public void Request_SaveSkillTreeData(int _skillIndex, int _level)
    {
        var saveKey = GetSkillTreeDataKey(_skillIndex);
        var newData = new SkillTreeData()
        {
            key = saveKey,
            level = _level
        };
        skillTreeDataCache[_skillIndex] = newData;

        EnqueueMethod(() => { SaveSkillTreeData(newData); });
    }
    private void SaveSkillTreeData(SkillTreeData _newData)
    {
        ES3.Save(_newData.key, _newData);
    }
    private string GetSkillTreeDataKey(int _skillIndex)
    {
        return $"SkillTreeData-{_skillIndex}";
    }
    public int Get_SkillTreeTotalCount()
    {
        var skillTreeTotalCount = 0;
        foreach (var skillTreeData in SOLoader.SkillTreeData.skillTreeUnits)
        {
            var key = GetSkillTreeDataKey(skillTreeData.skillTreeIndex);
            if (ES3.KeyExists(key))
            {
                skillTreeTotalCount++;
            }
        }
        return skillTreeTotalCount;
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

    /// <summary>
    /// アーティファクトの所持数を取得
    /// </summary>
    public int Get_ArtifactTotalCount()
    {
        var artifactCount = 0;
        foreach (var artifactData in SOLoader.ArtifactData.artifactDatas)
        {
            var key = GetArtifactDataKey(artifactData.artifactIndex);
            if (ES3.KeyExists(key))
            {
                artifactCount++;
            }
        }
        return artifactCount;
    }

    /// <summary>
    /// アーティファクト未所持のインデックスを全て取得
    /// </summary>
    public int[] Get_ArtifactIndex_NotGet()
    {
        var list = new List<int>();
        foreach (var artifactData in SOLoader.ArtifactData.artifactDatas)
        {
            var key = GetArtifactDataKey(artifactData.artifactIndex);
            if (!ES3.KeyExists(key))
            {
                list.Add(artifactData.artifactIndex);
            }
        }
        return list.ToArray();
    }
    /// <summary>
    /// アーティファクト確率計算用、破壊したブロック数をカウント
    /// </summary>
    public void Request_ArtifactCurrentBlockCount(int _deltaCount, bool _isReset = false)
    {
        EnqueueMethod(() =>
        {
            if (_isReset)
            {
                artifactCurrentBlockCount = 0;
            }
            else
            {
                artifactCurrentBlockCount += _deltaCount;
            }
            ES3.Save(KEY_ARTIFACT_CURRENTBLOCKCOUNT, artifactCurrentBlockCount);
        });
    }
    #endregion




    #region -- Artifact Slot --
    public async UniTask<ArtifactSlotData> Get_ArtifactSlotData(int _slotIndex)
    {
        string saveKey = GetArtifactSlotDataKey(_slotIndex);
        var loadData = await LoadAsync<ArtifactSlotData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SaveArtifactSlotData(int _slotIndex, bool _isOpen, int _equipedArtifactIndex)
    {
        EnqueueMethod(() => { SaveArtifactSlotData(_slotIndex, _isOpen, _equipedArtifactIndex); });
    }
    private void SaveArtifactSlotData(int _slotIndex, bool _isOpen, int _equipedArtifactIndex)
    {
        var saveKey = GetArtifactSlotDataKey(_slotIndex);
        var newData = new ArtifactSlotData()
        {
            slotIndex = _slotIndex,
            isOpen = _isOpen,
            equipedArtifactIndex = _equipedArtifactIndex
        };
        ES3.Save(saveKey, newData);
    }
    private string GetArtifactSlotDataKey(int _slotIndex)
    {
        return $"ArtifactSlotData-{_slotIndex}";
    }
    #endregion




    #region -- Pickaxe --
    public async UniTask<PickaxeData> Get_PickaxeData(int _pickaxeIndex)
    {
        string saveKey = GetPickaxeDataKey(_pickaxeIndex);
        var loadData = await LoadAsync<PickaxeData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SavePickaxeData(int _pickaxeIndex, int _level)
    {
        EnqueueMethod(() => { SavePickaxeData(_pickaxeIndex, _level); });
    }
    private void SavePickaxeData(int _pickaxeIndex, int _level)
    {
        var saveKey = GetPickaxeDataKey(_pickaxeIndex);
        var newData = new PickaxeData()
        {
            pickaxeIndex = _pickaxeIndex,
            level = _level
        };
        ES3.Save(saveKey, newData);
    }
    private string GetPickaxeDataKey(int _pickaxeIndex)
    {
        return $"PickaxeData-{_pickaxeIndex}";
    }

    public async UniTask<PickaxeSlotData> Get_PickaxeSlotData(int _slotIndex)
    {
        string saveKey = GetPickaxeSlotDataKey(_slotIndex);
        var loadData = await LoadAsync<PickaxeSlotData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SavePickaxeSlotData(int _slotIndex, int _equipedPickaxeIndex)
    {
        EnqueueMethod(() => { SavePickaxeSlotData(_slotIndex, _equipedPickaxeIndex); });
    }
    private void SavePickaxeSlotData(int _slotIndex, int _equipedPickaxeIndex)
    {
        var saveKey = GetPickaxeSlotDataKey(_slotIndex);
        var newData = new PickaxeSlotData()
        {
            slotIndex = _slotIndex,
            equipedPickaxeIndex = _equipedPickaxeIndex
        };
        ES3.Save(saveKey, newData);
    }
    private string GetPickaxeSlotDataKey(int _slotIndex)
    {
        return $"PickaxeSlotData-{_slotIndex}";
    }
    public int Get_PickaxeTotalCount()
    {
        var pickaxeTotalCount = 0;
        foreach (var pickaxeData in SOLoader.PickaxePowerData.pickaxePowerBases)
        {
            var key = GetPickaxeDataKey(pickaxeData.index);
            if (ES3.KeyExists(key))
            {
                pickaxeTotalCount++;
            }
        }
        return pickaxeTotalCount;
    }

    #endregion






    #region -- Pickaxe Power Data --
    public async UniTask<PickaxePowerData> Get_PickaxePowerData(int _pickaxePowerIndex)
    {
        string saveKey = GetPickaxePowerDataKey(_pickaxePowerIndex);
        var loadData = await LoadAsync<PickaxePowerData>(saveKey);
        if (loadData.success)
        {
            return loadData.data;
        }
        return null;
    }
    public void Request_SavePickaxePowerData_Level(int _pickaxePowerIndex, int _level)
    {
        EnqueueMethod(() => { SavePickaxePowerData_Level(_pickaxePowerIndex, _level); });
    }
    private void SavePickaxePowerData_Level(int _pickaxePowerIndex, int _level)
    {
        var saveKey = GetPickaxePowerDataKey(_pickaxePowerIndex);
        var newData = new PickaxePowerData()
        {
            pickaxePowerIndex = _pickaxePowerIndex,
            level = _level
        };
        ES3.Save(saveKey, newData);
    }
    private string GetPickaxePowerDataKey(int _pickaxePowerIndex)
    {
        return $"PickaxePowerData-{_pickaxePowerIndex}";
    }


    public void Request_SavePickaxePowerData_EquipedIndex(int _equipIndex)
    {
        EnqueueMethod(() => { SavePickaxePowerData_EquipedIndex(_equipIndex); });
    }
    private void SavePickaxePowerData_EquipedIndex(int _equipIndex)
    {
        pickaxePowerEquipedIndex = _equipIndex;
        ES3.Save(KEY_PICKAXEPOWER_EQUIPEDINDEX, pickaxePowerEquipedIndex);
    }
    #endregion






    #region -- GameRecordData --
    public async UniTask<GameRecordData> Get_GameRecordData()
    {
        var loadData = await LoadAsync<GameRecordDataSave>(KEY_GAME_RECORD_DATA);
        if (loadData.success)
        {
            return GameRecordDataFromSave(loadData.data);
        }
        return new GameRecordData();
    }
    public void Request_SaveGameRecordData(GameRecordData _gameRecordData)
    {
        EnqueueMethod(() => { SaveGameRecordData(_gameRecordData); });
    }
    private void SaveGameRecordData(GameRecordData _gameRecordData)
    {
        var saveData = new GameRecordDataSave
        {
            total_ingameCount = _gameRecordData.total_ingameCount.ToString(),
            total_blockBreakCount = _gameRecordData.total_blockBreakCount.ToString(),
            total_treasureCount = _gameRecordData.total_treasureCount.ToString(),
            total_playerExp = _gameRecordData.total_playerExp.ToString(),
            total_totalDamage = _gameRecordData.total_totalDamage.ToString(),
            total_skillTreeCount = _gameRecordData.total_skillTreeCount.ToString(),
            total_artifactCount = _gameRecordData.total_artifactCount.ToString(),
            total_pickaxeCount = _gameRecordData.total_pickaxeCount.ToString(),


            oneGame_blockBreakCount = _gameRecordData.oneGame_blockBreakCount.ToString(),
            oneGame_treasureCount = _gameRecordData.oneGame_treasureCount.ToString(),
            oneGame_playerExp = _gameRecordData.oneGame_playerExp.ToString(),
            oneGame_totalDamage = _gameRecordData.oneGame_totalDamage.ToString(),
            oneGame_maxDepth = _gameRecordData.oneGame_maxDepth.ToString(),
        };
        ES3.Save(KEY_GAME_RECORD_DATA, saveData);
    }
    private static BigInteger ParseBigInteger(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        return BigInteger.Parse(s);
    }
    private static GameRecordData GameRecordDataFromSave(GameRecordDataSave save)
    {
        if (save == null) return new GameRecordData();
        return new GameRecordData
        {
            total_ingameCount = ParseBigInteger(save.total_ingameCount),
            total_blockBreakCount = ParseBigInteger(save.total_blockBreakCount),
            total_treasureCount = ParseBigInteger(save.total_treasureCount),
            total_playerExp = ParseBigInteger(save.total_playerExp),
            total_totalDamage = ParseBigInteger(save.total_totalDamage),
            total_skillTreeCount = ParseBigInteger(save.total_skillTreeCount),
            total_artifactCount = ParseBigInteger(save.total_artifactCount),
            total_pickaxeCount = ParseBigInteger(save.total_pickaxeCount),

            oneGame_blockBreakCount = ParseBigInteger(save.oneGame_blockBreakCount),
            oneGame_treasureCount = ParseBigInteger(save.oneGame_treasureCount),
            oneGame_playerExp = ParseBigInteger(save.oneGame_playerExp),
            oneGame_totalDamage = ParseBigInteger(save.oneGame_totalDamage),
            oneGame_maxDepth = ParseBigInteger(save.oneGame_maxDepth),
        };
    }
    #endregion



    #region -- User Settings --
    public void Request_SaveUserSettings(UserSettingsData data)
    {
        EnqueueMethod(() => ES3.Save(KEY_USER_SETTINGS, data));
    }
    #endregion





    #region -- Debug --
    /// <summary>
    /// デバッグ用: 初期ピッケル(index=1)以外の獲得状況をリセットし、装備も初期状態に戻す
    /// </summary>
    public void Debug_ResetPickaxeExceptInitial()
    {
#if UNITY_EDITOR
        const int initialPickaxeIndex = 1;
        var deletedCount = 0;
        foreach (var pickaxeUnitData in SOLoader.AttackUnitData.pickaxeUnitDatas)
        {
            if (pickaxeUnitData.pickaxeIndex == initialPickaxeIndex) continue;
            var key = GetPickaxeDataKey(pickaxeUnitData.pickaxeIndex);
            if (!ES3.KeyExists(key)) continue;
            ES3.DeleteKey(key);
            deletedCount++;
        }

        SavePickaxeData(initialPickaxeIndex, 1);
        SavePickaxeSlotData(0, initialPickaxeIndex);
        Debug.Log($"Debug_ResetPickaxeExceptInitial: deleted={deletedCount}, equip reset to pickaxeIndex={initialPickaxeIndex}");
#endif
    }
    #endregion


}
