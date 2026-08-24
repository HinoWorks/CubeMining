using UnityEngine;
using System.ComponentModel;
using SRF.Service;
using SRDebugger.Services;
using System.Runtime.CompilerServices;
using System;
using Cysharp.Threading.Tasks;


public partial class SROptions
{
    // -----------------------------------
    [Category("Time")]
    [DisplayName("TimeScale")]
    [Sort(0)]
    [Increment(0.5)]
    [NumberRange(1, 10.0)]
    public float TimeScale
    {
        get { return Time.timeScale; }
        set { Time.timeScale = value; }
    }


#if DEVELOPMENT_BUILD
    [Category("セーブデータ周り")]
    [DisplayName("データリセットして終了")]
    [Sort(0)]
    public void DeleteData_All()
    {
        //SaveLoader.Inst.Debug_SaveDataDelete();
    }
#endif


    // =============================
    /*
    [Category("アイテム取得")]
    [DisplayName("コイン +100")]
    [Sort(0)]
    public void ItemGet_Coin()
    {
         SaveLoader.Inst.Request_SaveCoin(100);
    }
    */

    [Category("アイテム取得")]
    [DisplayName("石 +10")]
    [Sort(2)]
    public void ItemGet_Stone_10()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Stone, 10);
    }
    [Category("アイテム取得")]
    [DisplayName("鉄 +10")]
    [Sort(3)]
    public void ItemGet_Iron_10()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Iron, 10);
    }
    [Category("アイテム取得")]
    [DisplayName("金 +10")]
    [Sort(4)]
    public void ItemGet_Gold_10()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Gold, 10);
    }
    [Category("アイテム取得")]
    [DisplayName("エメラルド +10")]
    [Sort(5)]
    public void ItemGet_Emerald_10()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Emerald, 10);
    }
    [Category("アイテム取得")]
    [DisplayName("ルビー +10")]
    [Sort(6)]
    public void ItemGet_Ruby_10()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Ruby, 10);
    }
    [Category("アイテム取得")]
    [DisplayName("サファイア +10")]
    [Sort(7)]
    public void ItemGet_Sapphire_10()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Sapphire, 10);
    }
    [Category("アイテム取得")]
    [DisplayName("ダイアモンド +10")]
    [Sort(8)]
    public void ItemGet_Diamond_10()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Diamond, 10);
    }

    [Category("アイテム取得")]
    [DisplayName("石 +100")]
    [Sort(9)]
    public void ItemGet_Stone_100()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Stone, 100);
    }
    [Category("アイテム取得")]
    [DisplayName("鉄 +100")]
    [Sort(10)]
    public void ItemGet_Iron_100()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Iron, 100);
    }
    [Category("アイテム取得")]
    [DisplayName("金 +100")]
    [Sort(11)]
    public void ItemGet_Gold_100()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Gold, 100);
    }
    [Category("アイテム取得")]
    [DisplayName("エメラルド +100")]
    [Sort(12)]
    public void ItemGet_Emerald_100()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Emerald, 100);
    }
    [Category("アイテム取得")]
    [DisplayName("ルビー +100")]
    [Sort(13)]
    public void ItemGet_Ruby_100()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Ruby, 100);
    }
    [Category("アイテム取得")]
    [DisplayName("サファイア +100")]
    [Sort(14)]
    public void ItemGet_Sapphire_100()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Sapphire, 100);
    }
    [Category("アイテム取得")]
    [DisplayName("ダイアモンド +100")]
    [Sort(15)]
    public void ItemGet_Diamond_100()
    {
        SaveLoader.Inst.Request_SaveResource(ResourceType.Diamond, 100);
    }


    public static bool isSkillTreeUpgradeNoMaterial = false;
    [Category("Skill")]
    [DisplayName("SKill Tree アップグレード素材なし)")]
    [Sort(0)]
    public bool IsSkillTreeUpgradeNoMaterial
    {
        get { return isSkillTreeUpgradeNoMaterial; }
        set { isSkillTreeUpgradeNoMaterial = value; }
    }

    public static bool isPickaxeCraftNoMaterial = false;
    [Category("Pickaxe")]
    [DisplayName("ピッケル作成 素材なし")]
    [Sort(0)]
    public bool IsPickaxeCraftNoMaterial
    {
        get { return isPickaxeCraftNoMaterial; }
        set { isPickaxeCraftNoMaterial = value; }
    }

    public static bool isPickaxePowerUpgradeNoMaterial = false;
    [Category("Pickaxe")]
    [DisplayName("ピッケルパワー 素材なし")]
    [Sort(1)]
    public bool IsPickaxePowerUpgradeNoMaterial
    {
        get { return isPickaxePowerUpgradeNoMaterial; }
        set { isPickaxePowerUpgradeNoMaterial = value; }
    }

    [Category("Pickaxe")]
    [DisplayName("ピッケル獲得状況リセット(初期以外)")]
    [Sort(2)]
    public void Debug_ResetPickaxeExceptInitial()
    {
        if (SaveLoader.Inst == null)
        {
            Debug.LogWarning("Debug_ResetPickaxeExceptInitial: SaveLoader not found");
            return;
        }

        SaveLoader.Inst.Debug_ResetPickaxeExceptInitial();

        if (UIManager_OutGame.Inst != null && UIManager_OutGame.Inst.UI_PickaxeManager != null)
        {
            UIManager_OutGame.Inst.UI_PickaxeManager.ToOutGame_InitData();
        }
    }


    private int targetIndex;
    [Category("要素アンロックAttack")]
    [DisplayName("index")]
    [Sort(0)]
    public int SetTargetIndex
    {
        get { return targetIndex; }
        set { targetIndex = value; }
    }
    [Category("要素アンロックAttack")]
    [DisplayName("アンロック(Attack)")]
    [Sort(1)]
    public void UnlockIndex()
    {
        //var targetSkillTreeData = SOLoader.SkillTreeData.GetSkillTreeDatas(ParamCategory.Attack, targetIndex, ParamType.Unlock);
        //if (targetSkillTreeData == null) return;
        //SaveLoader.Inst.Request_SaveSkillTreeData(targetSkillTreeData.index, 1);
        GameParamManager.DEBUG_AttackParam_Unlock(targetIndex);
    }

    /*
        private int targetIndex_block;
        [Category("要素アンロックBlock")]
        [DisplayName("index")]
        [Sort(0)]
        public int SetTargetIndex_block
        {
            get { return targetIndex_block; }
            set { targetIndex_block = value; }
        }
        [Category("要素アンロックBlock")]
        [DisplayName("アンロック(Block)")]
        [Sort(1)]
        public void UnlockIndex_block()
        {
            var targetSkillTreeData = SOLoader.SkillTreeData.GetSkillTreeDatas(ParamCategory.Block, targetIndex_block, ParamType.Unlock);
            if (targetSkillTreeData == null) return;
            SaveLoader.Inst.Request_SaveSkillTreeData(targetSkillTreeData.index, 1);
        }
    */




    [Category("システム周り")]
    [DisplayName("ゴールカウント +10")]
    [Sort(0)]
    public void ForceCountUp_10()
    {
        //GameWatcher.Inst.gameEvent.ev_goalCountMod?.Invoke(10);
    }


    private int debugArtifactIndex;
    [Category("デバッグ")]
    [DisplayName("アーティファクト index")]
    [Sort(0)]
    public int DebugArtifactIndex
    {
        get { return debugArtifactIndex; }
        set { debugArtifactIndex = value; }
    }
    [Category("デバッグ")]
    [DisplayName("アーティファクト獲得")]
    [Sort(1)]
    public void Debug_GetArtifact()
    {
        var artifactData = SOLoader.ArtifactData.Get_ArtifactData(debugArtifactIndex);
        if (artifactData == null)
        {
            Debug.LogWarning($"Debug_GetArtifact: artifact not found (index={debugArtifactIndex})");
            return;
        }

        if (InGameManager.Inst != null)
        {
            InGameManager.Inst.AddGetArtifact(debugArtifactIndex);
        }
        else
        {
            SaveLoader.Inst.Request_SaveArtifactData(debugArtifactIndex, 1);
        }
        Debug.Log($"Debug_GetArtifact: acquired index={debugArtifactIndex} ({artifactData.artifactName})");
    }
    [Category("デバッグ")]
    [DisplayName("アーティファクト全解放")]
    [Sort(2)]
    public void Debug_UnlockAllArtifacts()
    {
        var count = 0;
        foreach (var artifactData in SOLoader.ArtifactData.artifactDatas)
        {
            SaveLoader.Inst.Request_SaveArtifactData(artifactData.artifactIndex, 1);
            count++;
        }
        Debug.Log($"Debug_UnlockAllArtifacts: unlocked {count} artifacts");
    }
    [Category("デバッグ")]
    [DisplayName("レベル強制アップ")]
    [Sort(3)]
    public void Debug_ForceLevelUp()
    {
        if (PlayerLevelManager.Inst == null)
        {
            Debug.LogWarning("Debug_ForceLevelUp: PlayerLevelManager not found");
            return;
        }
        PlayerLevelManager.Inst.DEBUG_ForceLevelUp();
    }

    [Category("デバッグ")]
    [DisplayName("強化コイン取得")]
    [Sort(4)]
    public void Debug_GetEnhanceCoin()
    {
        SaveLoader.Inst.Request_SaveEnhanceCoinCount(1);

    }

    private int debugTutorialIndex;
    [Category("デバッグ")]
    [DisplayName("チュートリアル index")]
    [Sort(5)]
    public int DebugTutorialIndex
    {
        get { return debugTutorialIndex; }
        set { debugTutorialIndex = value; }
    }
    [Category("デバッグ")]
    [DisplayName("チュートリアル表示(セーブなし)")]
    [Sort(6)]
    public void Debug_ShowTutorial()
    {
        if (TutorialManager.Inst == null)
        {
            Debug.LogWarning("Debug_ShowTutorial: TutorialManager not found");
            return;
        }

        var data = SOLoader.TutorialData?.Get_TutorialUnitData(debugTutorialIndex);
        if (data == null)
        {
            Debug.LogWarning($"Debug_ShowTutorial: tutorial not found (index={debugTutorialIndex})");
            return;
        }

        TutorialManager.Inst.Debug_ShowTutorial(debugTutorialIndex).Forget();
        Debug.Log($"Debug_ShowTutorial: show index={debugTutorialIndex} (no save)");
    }


    [Category("例外 / クラッシュ")]
    [DisplayName("例外スロー")]
    [Sort(0)]
    public void ForceException()
    {
        Debug.Log("==Test== log");
        Debug.LogWarning("==Test== Warning log");
        Debug.LogError("==Test== Error Log");

        throw new ExceptionTest(" == Test == Exception Here !");
    }
    [Category("例外 / クラッシュ")]
    [DisplayName("クラッシュ")]
    [Sort(1)]
    public void ForceCrash()
    {
        UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.AccessViolation);
    }
    private class ExceptionTest : System.Exception
    {
        public ExceptionTest(string _message) : base(_message) { }
    }

}