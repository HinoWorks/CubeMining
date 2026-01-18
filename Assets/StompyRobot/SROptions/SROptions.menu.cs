using UnityEngine;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using SRF.Service;
using SRDebugger.Services;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;




public partial class SROptions
{
    // private BlockType selectBlockType;

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
    [Category("アイテム取得")]
    [DisplayName("コイン +100")]
    [Sort(0)]
    public void ItemGet_Coin()
    {
        SaveLoader.Inst.Request_SaveCoin(100);
    }
    [Category("アイテム取得")]
    [DisplayName("コイン +10000")]
    [Sort(1)]
    public void ItemGet_Coin_10000()
    {
        SaveLoader.Inst.Request_SaveCoin(10000);
    }


    private int targetIndex;
    [Category("要素アンロック -- Attack")]
    [DisplayName("index")]
    [Sort(0)]
    public int SetTargetIndex
    {
        get { return targetIndex; }
        set { targetIndex = value; }
    }
    [Category("要素アンロック -- Attack")]
    [DisplayName("アンロック")]
    [Sort(1)]
    public void UnlockIndex()
    {
        //var targetSkillTreeData = SOLoader.SkillTreeData.GetSkillTreeDatas(ParamCategory.Attack, targetIndex, ParamType.Unlock);
        //if (targetSkillTreeData == null) return;
        //SaveLoader.Inst.Request_SaveSkillTreeData(targetSkillTreeData.index, 1);
        GameParamManager.DEBUG_AttackParam_Unlock(targetIndex);
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