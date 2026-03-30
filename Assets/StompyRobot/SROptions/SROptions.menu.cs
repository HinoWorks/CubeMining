using UnityEngine;
using System.ComponentModel;
using SRF.Service;
using SRDebugger.Services;
using System.Runtime.CompilerServices;
using System;


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


    public static bool isSkillTreeUpgradeNoMaterial = false;
    [Category("Skill")]
    [DisplayName("SKill Tree アップグレード素材なし)")]
    [Sort(0)]
    public bool IsSkillTreeUpgradeNoMaterial
    {
        get { return isSkillTreeUpgradeNoMaterial; }
        set { isSkillTreeUpgradeNoMaterial = value; }
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





    [Category("システム周り")]
    [DisplayName("ゴールカウント +10")]
    [Sort(0)]
    public void ForceCountUp_10()
    {
        //GameWatcher.Inst.gameEvent.ev_goalCountMod?.Invoke(10);
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