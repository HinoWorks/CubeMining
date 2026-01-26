using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


/// <summary>
/// ゲームの基本パラメータ
/// </summary>
public class GameBaseParam
{
    // インゲーム時間
    public float ingameTime => ingameTime_Base + ingameTime_enhanced;
    private float ingameTime_Base = 10f;
    private float ingameTime_enhanced = 0f;

    // リザルト時のコインボーナス
    public float coinBonusRate => 1f + coinBonusRate_enhanced;
    private float coinBonusRate_enhanced = 0f;

    // ブロック生成時間の短縮率
    public float blockGenerateTimeRate => 1f - blockGenerateTimeRate_enhanced;
    private float blockGenerateTimeRate_enhanced = 0f;


    public void Set_SkillTreeParam(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.IngameTime:
                ingameTime_enhanced += _setParam;
                break;
            case ParamType.CoinBonusRate:
                coinBonusRate_enhanced += _setParam;
                break;

                // ==== TODO HERE ====
                // Add here Artifact param

        }
    }
}

/// <summary>
/// ブロック以外のオブジェクト生成パラメータ
/// </summary>
public class ObjectGenerateParam
{
    public ObjectUnitData so;
    public float generateRate { get; private set; }
    public float valueRate { get; private set; }

    public void Init(ObjectUnitData _objectUnitData)
    {
        so = _objectUnitData;
        generateRate = _objectUnitData.generateRate;
        valueRate = _objectUnitData.valueRate;
    }
    public void Set_SkillTreeParam(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Rate_Generate:
                generateRate += _setParam;
                break;
            case ParamType.Rate_Value:
                valueRate += _setParam;
                break;
        }
    }
}


/// <summary>
/// ブロックの生成パラメータ
/// </summary>
public class BlockGenerateParam
{
    public BlockData so;
    public bool isActive { get; private set; } = false;

    public int blockIndex { get; private set; }
    public int hp { get; private set; }
    public int baseValue { get; private set; }
    public float generateInterval { get; private set; }
    public int count { get; private set; }
    public float size { get; private set; }

    public float bigBlockRate { get; private set; }
    public int separateBlockCount { get; private set; }


    public void Init(BlockData _blockData)
    {
        so = _blockData;

        blockIndex = _blockData.blockIndex;
        isActive = so.blockIndex == 1 ? true : false;
        hp = _blockData.hp;
        baseValue = _blockData.baseValue;
        generateInterval = _blockData.generateInterval;
        count = _blockData.count;
        size = _blockData.size;
        bigBlockRate = _blockData.bigBlockRate;
        separateBlockCount = _blockData.separateBlock;
    }

    public void Set_SkillTreeParam(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Unlock:
                isActive = true;
                break;

            case ParamType.Value:
                baseValue += (int)_setParam;
                break;
            case ParamType.Interval:
                generateInterval += _setParam;
                break;
            case ParamType.Count:
                count += (int)_setParam;
                break;
            case ParamType.Size:
                size += _setParam;
                break;
            case ParamType.BigBlockRate:
                bigBlockRate += _setParam;
                break;
            case ParamType.SeparateBlockCount:
                separateBlockCount += (int)_setParam;
                break;
        }
    }


}

/// <summary>
/// アタックユニットのパラメータ
/// </summary>
public class AttackParam
{
    public AttackUnitData so;
    public bool isActive { get; private set; } = false;
    public int attackUnitIndex { get; private set; }

    public float damage { get; private set; }
    public float aliveTime { get; private set; }
    public float ct { get; private set; }
    public float speed { get; private set; }
    public int count { get; private set; }
    public float attackInterval { get; private set; }
    public float size { get; private set; }

    public void Init(AttackUnitData _attackUnitData)
    {
        so = _attackUnitData;
        attackUnitIndex = _attackUnitData.attackIndex;
        isActive = attackUnitIndex == 1 ? true : false;
        damage = _attackUnitData.damage;
        aliveTime = _attackUnitData.aliveTime;
        ct = _attackUnitData.ct;
        speed = _attackUnitData.speed;
        count = _attackUnitData.count;
        attackInterval = _attackUnitData.attackInterval;
        size = _attackUnitData.size;
    }

    public void Set_SkillTreeParam(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Unlock:
                isActive = true;
                break;
            case ParamType.Damage:
                damage += _setParam;
                break;
            case ParamType.AliveTime:
                aliveTime += _setParam;
                break;
            case ParamType.CT:
                ct += _setParam;
                break;
            case ParamType.Speed:
                speed += _setParam;
                break;
            case ParamType.Count:
                count += (int)_setParam;
                break;
            case ParamType.Interval:
                attackInterval += _setParam;
                break;
            case ParamType.Size:
                size += _setParam;
                break;
        }
    }
}



/// <summary>
/// ゲームのパラメータを管理するクラス
/// 全てのパラはここを参照して取得する
/// </summary>
public static class GameParamManager
{
    public readonly static GameBaseParam gameBaseParam = new GameBaseParam();
    public readonly static List<ObjectGenerateParam> list_objectGenerateParam = new List<ObjectGenerateParam>();
    public readonly static List<BlockGenerateParam> list_blockGenerateParam = new List<BlockGenerateParam>();
    public readonly static List<AttackParam> list_attackParam = new List<AttackParam>();


    #region get param reference
    public static BlockGenerateParam Get_BlockGenerateParam(int _blockIndex)
    {
        var targetBlock = list_blockGenerateParam.Find(x => x.blockIndex == _blockIndex);
        if (targetBlock == null)
        {
            Debug.LogError($"BlockData is not found: {_blockIndex} // ==> 初期ロードで読み込み失敗");
        }
        return targetBlock;
    }
    public static AttackParam Get_AttackParam(int _attackIndex)
    {
        var targetAttack = list_attackParam.Find(x => x.attackUnitIndex == _attackIndex);
        if (targetAttack == null)
        {
            Debug.LogError($"AttackUnitData is not found: {_attackIndex} // ==> 初期ロードで読み込み失敗");
        }
        return targetAttack;
    }
    #endregion


    public static async UniTask Init()
    {
        // ゲームの基本的なパラメタを読み込む
        Init_GameBaseParam();

        await Init_SkillTreeParam(); // skill treeによるデータ更新
        await Init_ArtifactParam(); // artifactによるデータ更新

        await UniTask.DelayFrame(1);
    }

    public static void Init_GameBaseParam()
    {
        // block generate param init
        list_blockGenerateParam.Clear();
        foreach (var blockData in SOLoader.BlockData.blockDatas)
        {
            var blockParam = new BlockGenerateParam();
            blockParam.Init(blockData);
            list_blockGenerateParam.Add(blockParam);
        }

        // object generate param init
        list_objectGenerateParam.Clear();
        foreach (var objectData in SOLoader.ObjectUnitData.objectUnitDatas)
        {
            var objectParam = new ObjectGenerateParam();
            objectParam.Init(objectData);
            list_objectGenerateParam.Add(objectParam);
        }

        // attack param init
        list_attackParam.Clear();
        foreach (var attackData in SOLoader.AttackUnitData.attackUnitDatas)
        {
            var attackParam = new AttackParam();
            attackParam.Init(attackData);
            list_attackParam.Add(attackParam);
        }
    }

    private static async UniTask Init_SkillTreeParam()
    {
        foreach (var skillData in SOLoader.SkillTreeData.skillTreeDatas)
        {
            var saveData = await SaveLoader.Inst.Get_SkillTreeData(skillData.index);
            if (saveData == null) continue;
            var setParam = skillData.baseValue + skillData.deltaValue * saveData.level;
            Set_DeltaParam(skillData.paramCategory, skillData.targetIndex, skillData.paramType, setParam);
        }
    }

    private static async UniTask Init_ArtifactParam()
    {
        foreach (var artifactData in SOLoader.ArtifactData.artifactDatas)
        {
            var saveData = await SaveLoader.Inst.Get_ArtifactData(artifactData.artifactIndex);
            if (saveData == null) continue;
            Set_DeltaParam(ParamCategory.GameSystem, -1, artifactData.paramType, artifactData.value);
        }
    }


    /// <summary>
    /// パラメータの差分fix
    /// </summary>
    public static void Set_DeltaParam(ParamCategory _paramCategory, int _targetIndex, ParamType _paramType, float _setParam)
    {
        switch (_paramCategory)
        {
            case ParamCategory.GameSystem:
                Set_GamesystemParam(_paramType, _setParam);
                break;
            case ParamCategory.Block:
                Set_BlockParam(_targetIndex, _paramType, _setParam);
                break;
            case ParamCategory.OtherObject:
                Set_BlockParam(_targetIndex, _paramType, _setParam);
                break;
            case ParamCategory.Attack:
                Set_AttackParam(_targetIndex, _paramType, _setParam);
                break;
        }
    }

    private static void Set_GamesystemParam(ParamType _paramType, float _setParam)
    {
        gameBaseParam.Set_SkillTreeParam(_paramType, _setParam);
    }
    private static void Set_BlockParam(int _blockIndex, ParamType _paramType, float _setParam)
    {
        var targetBlock = list_blockGenerateParam.Find(x => x.blockIndex == _blockIndex);
        if (targetBlock == null)
        {
            Debug.LogError($"BlockData is not found: {_blockIndex} // ==> 初期ロードで読み込み失敗");
            return;
        }
        targetBlock.Set_SkillTreeParam(_paramType, _setParam);
    }
    private static void Set_AttackParam(int _attackIndex, ParamType _paramType, float _setParam)
    {
        var targetAttack = list_attackParam.Find(x => x.attackUnitIndex == _attackIndex);
        if (targetAttack == null)
        {
            Debug.LogError($"AttackUnitData is not found: {_attackIndex} // ==> 初期ロードで読み込み失敗");
        }
        targetAttack.Set_SkillTreeParam(_paramType, _setParam);
    }







    #region DEBUG
    public static void DEBUG_AttackParam_Unlock(int _attackIndex)
    {
        var targetAttack = list_attackParam.Find(x => x.attackUnitIndex == _attackIndex);
        if (targetAttack == null)
        {
            Debug.LogError($"AttackUnitData is not found: {_attackIndex} // ==> 初期ロードで読み込み失敗");
            return;
        }
        targetAttack.Set_SkillTreeParam(ParamType.Unlock, 1f);
    }

    #endregion

}
