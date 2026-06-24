using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System;
using UnityEditor;

public class DataBase : MonoBehaviour
{
    /*
        public static DataBase Inst;
        void Awake()
        {
            if (Inst == null) { Inst = this; }
            else { Destroy(this); }
        }
        */

    // --- GSS Load Setting --
    //https://docs.google.com/spreadsheets/d/18r14I3b0cia4TxMk525C686vuO6sjbfO5Cd16gWzNes/edit?usp=sharing
    private string SheetID = "18r14I3b0cia4TxMk525C686vuO6sjbfO5Cd16gWzNes";
    string tqx = "tqx=out:csv";




    [Header("GSS Load data")]
    [SerializeField] SO_SkillTreeData mSO_SkillTreeData;
    [SerializeField] SO_AttackUnitData mSO_AttackUnitData;
    [SerializeField] SO_BlockData mSO_BlockData;
    [SerializeField] SO_ObjectUnit mSO_ObjectUnitData;
    [SerializeField] SO_ArtifactData mSO_ArtifactData;
    [SerializeField] SO_UnlockData mSO_UnlockData;
    [SerializeField] SO_BlockLayerData mSO_BlockGenerateParam_LayerData;
    [SerializeField] SO_ItemData mSO_ItemData;
    [SerializeField] SO_PlayerLevelData mSO_PlayerLevelData;
    [SerializeField] SO_PickaxePowerData mSO_PickaxePowerData;
    [SerializeField] SO_BlockGenerateData mSO_BlockGenerateData;

    [SerializeField] SO_AchievementData mSO_AchievementData;
    [SerializeField] SO_SoundData mSO_SoundData;

    public async UniTask LoadData()
    {
        await DataLoad_SkillTreeData();
        await DataLoad_AttackUnitData();
        await DataLoad_BlockData();
        await DataLoad_ObjectUnitData();
        await DataLoad_ArtifactData();
        await DataLoad_UnlockData();
        await DataLoad_BlockGenerateParam_LayerData();
        await DataLoad_ItemData();
        await DataLoad_PlayerLevelData();
        await DataLoad_PickaxePowerData();
        await DataLoad_BlockGenerateData();
        await DataLoad_SoundData();
        await DataLoad_AchievementData();
#if UNITY_EDITOR
        Debug.Log($"<color=yellow>End Master Data update!</color>");
        await UniTask.Delay(200, true);

        EditorUtility.SetDirty(mSO_SkillTreeData);
        EditorUtility.SetDirty(mSO_AttackUnitData);
        EditorUtility.SetDirty(mSO_BlockData);
        EditorUtility.SetDirty(mSO_ObjectUnitData);
        EditorUtility.SetDirty(mSO_ArtifactData);
        EditorUtility.SetDirty(mSO_UnlockData);
        EditorUtility.SetDirty(mSO_BlockGenerateParam_LayerData);
        EditorUtility.SetDirty(mSO_ItemData);
        EditorUtility.SetDirty(mSO_PlayerLevelData);
        EditorUtility.SetDirty(mSO_PickaxePowerData);
        EditorUtility.SetDirty(mSO_BlockGenerateData);
        EditorUtility.SetDirty(mSO_SoundData);
        EditorUtility.SetDirty(mSO_AchievementData);
        // -- save --
        AssetDatabase.SaveAssets();
#endif
    }


    // スキルツリーのみ更新
    public async UniTask SkillTreeData_Update()
    {
        await DataLoad_SkillTreeData();
        EditorUtility.SetDirty(mSO_SkillTreeData);
    }


    private async UniTask DataLoad_SkillTreeData()
    {
        var loadData = await DataLoad("SkillTreeBase");
        var convData = CSVSerializer.Deserialize<SkillTreeBase>(loadData);
        mSO_SkillTreeData.skillTreeDatas = convData;

        var loadData2 = await DataLoad("SkillTreeUnit");
        var convData2 = CSVSerializer.Deserialize<SkillTreeUnit>(loadData2);
        mSO_SkillTreeData.skillTreeUnits = convData2;
    }

    private async UniTask DataLoad_AttackUnitData()
    {
        var loadData = await DataLoad("AttackUnit");
        var convData = CSVSerializer.Deserialize<AttackUnitData>(loadData);
        mSO_AttackUnitData.attackUnitDatas = convData;

        var loadData2 = await DataLoad("PickaxeUnit");
        var convData2 = CSVSerializer.Deserialize<PickaxeUnitData>(loadData2);
        mSO_AttackUnitData.pickaxeUnitDatas = convData2;

        var loadData3 = await DataLoad("PickaxeResource");
        var convData3 = CSVSerializer.Deserialize<PickaxeResourceData>(loadData3);
        mSO_AttackUnitData.pickaxeResourceDatas = convData3;
    }

    private async UniTask DataLoad_BlockData()
    {
        var loadData = await DataLoad("BlockUnit");
        var convData = CSVSerializer.Deserialize<BlockData>(loadData);
        mSO_BlockData.blockDatas = convData;

        var loadData2 = await DataLoad("BlockChangeRate");
        var convData2 = CSVSerializer.Deserialize<BlockChangeRateData>(loadData2);
        mSO_BlockData.blockChangeRateDatas = convData2;
    }

    private async UniTask DataLoad_ObjectUnitData()
    {
        var loadData = await DataLoad("ObjectUnit");
        var convData = CSVSerializer.Deserialize<ObjectUnitData>(loadData);
        mSO_ObjectUnitData.objectUnitDatas = convData;
    }

    private async UniTask DataLoad_ArtifactData()
    {
        var loadData = await DataLoad("Artifact");
        var convData = CSVSerializer.Deserialize<ArtifactUnitData>(loadData);
        mSO_ArtifactData.artifactDatas = convData;

        var loadData2 = await DataLoad("ArtifactRate");
        var convData2 = CSVSerializer.Deserialize<ArtifactGenerateRateData>(loadData2);
        mSO_ArtifactData.artifactGenerateRateDatas = convData2;
    }

    private async UniTask DataLoad_UnlockData()
    {
        var loadData = await DataLoad("Unlock");
        var convData = CSVSerializer.Deserialize<UnlockData>(loadData);
        mSO_UnlockData.unlockDatas = convData;
    }
    private async UniTask DataLoad_BlockGenerateParam_LayerData()
    {
        var loadData = await DataLoad("BlockLayer");
        var convData = CSVSerializer.Deserialize<BlockLayerData>(loadData);
        mSO_BlockGenerateParam_LayerData.blockLayerDatas = convData;
    }

    private async UniTask DataLoad_ItemData()
    {
        var loadData = await DataLoad("Item");
        var convData = CSVSerializer.Deserialize<ItemUnitData>(loadData);
        mSO_ItemData.itemUnitDatas = convData;
    }

    private async UniTask DataLoad_SoundData()
    {
        var loadData = await DataLoad("Sound");
        var convData = CSVSerializer.Deserialize<SO_SoundElement>(loadData);
        mSO_SoundData.SoundData_SE = convData;
    }

    private async UniTask DataLoad_PlayerLevelData()
    {
        var loadData = await DataLoad("PlayerLevel");
        var convData = CSVSerializer.Deserialize<PlayerLevel>(loadData);
        mSO_PlayerLevelData.playerLevels = convData;
    }
    private async UniTask DataLoad_PickaxePowerData()
    {
        var loadData = await DataLoad("PickaxePowerBase");
        var convData = CSVSerializer.Deserialize<PickaxePowerBase>(loadData);
        mSO_PickaxePowerData.pickaxePowerBases = convData;

        var loadData2 = await DataLoad("PickaxePowerLevel");
        var convData2 = CSVSerializer.Deserialize<PickaxePowerLevel>(loadData2);
        mSO_PickaxePowerData.pickaxePowerLevels = convData2;
    }

    private async UniTask DataLoad_BlockGenerateData()
    {
        var loadData = await DataLoad("BlockGenerateParam");
        var convData = CSVSerializer.Deserialize<BlockGenerateParam>(loadData);
        mSO_BlockGenerateData.blockGenerateParams = convData;
    }

    private async UniTask DataLoad_AchievementData()
    {
        var loadData = await DataLoad("Achievement");
        var convData = CSVSerializer.Deserialize<AchievementUnitData>(loadData);
        mSO_AchievementData.achievementDatas = convData;
    }

    private async UniTask<string> DataLoad(string _sheetName)
    {
        string url = "https://docs.google.com/spreadsheets/d/" + SheetID + "/gviz/tq?" + tqx + "&sheet=" + _sheetName;
        UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.DataProcessingError
                    || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else { Debug.Log(request.downloadHandler.text); }
        return request.downloadHandler.text;
    }





    /*
    private async UniTask DataLoad_EnahnceData_Ball_()
    {
        string SheetName = "EnhanceData_Ball";

        string url = "https://docs.google.com/spreadsheets/d/" + SheetID + "/gviz/tq?" + tqx + "&sheet=" + SheetName;
        UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.DataProcessingError
                    || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else { Debug.Log(request.downloadHandler.text); }

        // -- create MasterData --
        //var convData = CSVSerializer.Deserialize<HintData>(request.downloadHandler.text);
        //mSO_HintData.hintDatas = convData;
    }*/






    #region -- loader 参考 --
    /*
    [MenuItem("Tools/マスターデータビルド/ローカル環境のマスターデータを最新にする")]
    public static async void LocalBuildMasterData()
    {
        Debug.Log("Build Master Data Start");

        var (names, jsons) = await GetSpreadSheetJsons();
        var datas = await GenerateMasterDatas(names, jsons);

        if (datas == null)
        {
            EditorUtility.DisplayDialog("マスターデータビルド", $"マスターデータのビルドに失敗しました。", "OK");
            return;
        }

        await SaveMasterDatas(names, datas);

        EditorUtility.DisplayDialog("マスターデータビルド", $"マスターデータのビルド・保存が完了しました。", "OK");
    }
    private static async UniTask<string[]> GetSpreadSheetNames()
    {
        var baseUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{SpreadSheetId}?key={SpreadSheetAPIKey}";
        var baseJson = await WebRequestUtil.DownloadJsonAsync(baseUrl);
        var root = JsonConvert.DeserializeObject<SpreadSheet_Root>(baseJson);
        var list = new List<string>();
        foreach (var sheet in root.Sheets)
        {
            if (!sheet.Properties.Title.Contains("ignore_")) list.Add(sheet.Properties.Title);
        }
        return list.ToArray();
    }

    private static async UniTask<(string[] names, string[] jsons)> GetSpreadSheetJsons()
    {
        var sheetNames = await GetSpreadSheetNames();

        string[] jsons = new string[sheetNames.Length];

        await UniTask.Run(() =>
        {
            Parallel.For(0, jsons.Length, i =>
            {
                var name = sheetNames[i];
                var url = $"https://sheets.googleapis.com/v4/spreadsheets/{SpreadSheetId}/values/{name}?key={SpreadSheetAPIKey}";
                jsons[i] = WebRequestUtil.DownloadJson(url);
            });
        });

        return (sheetNames, jsons);
    }

    private static async UniTask SaveMasterDatas(string[] names, MasterData[][] datas)
    {
        var init = ClientAPI.Inst;
        var hasLocalServerPath = EditorPrefs.HasKey("LocalServerPath");
        await UniTask.Run(async () =>
        {
            await UniTask.SwitchToMainThread();

            var versions = new MasterDataVersions();
            versions.Version = "0.0";
            versions.Data = new MasterDataVersion[names.Length];
            var dstPath = Path.Combine(EditorPrefs.GetString("LocalServerPath"), "master_data", "0.0");
            if (hasLocalServerPath)
            {
                if (Directory.Exists(dstPath)) Directory.Delete(dstPath, true);
                Directory.CreateDirectory(dstPath);
            }
            var assetPath = Application.dataPath;
            Parallel.For(0, datas.Length, i =>
            {
                UniTask.SwitchToMainThread();
                try
                {
                    var data = datas[i];
                    var name = names[i];
                    var root = new MasterDataRoot<MasterData>()
                    {
                        Name = name,
                        Version = "0.0",
                        Data = data,
                    };
                    var json = JsonConvert.SerializeObject(root, Formatting.Indented);
                    Debug.Log(name + " = " + json);
                    EncryptUtil.SaveObject(ClientAPI.GetMasterDataPath(name), root);
                    if (ClientAPI.NetworkEnv == "None")
                    {
                        EncryptUtil.SaveObject(Path.Combine(assetPath, "ClientApi/Resources/NoNetworkMasterData", name + ".bytes"), root);
                    }

                    if (!hasLocalServerPath)
                    {
                        Debug.LogWarning("ローカルサーバー用のマスターデータの出力先が指定されていません。[Edit]-[Preferences...]-[Local Server]-[Local server path]を指定してください。");
                    }
                    else
                    {
                        versions.Data[i] = new MasterDataVersion()
                        {
                            Name = names[i],
                        };

                        File.WriteAllText(Path.Combine(dstPath, names[i].ToLower() + ".json"), JsonConvert.SerializeObject(root));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            });
            if (hasLocalServerPath)
            {
                File.WriteAllText(Path.Combine(EditorPrefs.GetString("LocalServerPath"), "master_data", "latest_version.json"), JsonConvert.SerializeObject(versions));
            }

            EncryptUtil.SaveObject(ClientAPI.MasterVerPath, versions);
        });

        if (ClientAPI.NetworkEnv == "None")
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }*/
    #endregion

}