using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Analytics;


/// <summary>
/// ゲームの基本パラメータ
/// </summary>
public class GameBaseParam
{
    public float ingameTime { get; private set; }
    public float bonusRate { get; private set; }

    public void Set_SkillTreeParam(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.IngameTime:
                ingameTime += _setParam;
                break;
            case ParamType.BonusRate:
                bonusRate += _setParam;
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

    public void Init(BlockData _blockData)
    {
        so = _blockData;

        blockIndex = _blockData.blockIndex;
        isActive = false;
        hp = _blockData.hp;
        baseValue = _blockData.baseValue;
        generateInterval = _blockData.generateInterval;
        count = _blockData.count;
        size = _blockData.size;
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
        }
    }


}

/// <summary>
/// ブロックの生成パラメータ
/// </summary>
public class AttackParam
{
    public AttackUnitData so;
    public bool isActive { get; private set; } = false;
    public int attackUnitIndex { get; private set; }

    public float damage { get; private set; }
    public float aliveTime { get; private set; }
    public float ct { get; private set; }
    public int count { get; private set; }
    public float attackInterval { get; private set; }
    public float size { get; private set; }

    public void Init(AttackUnitData _attackUnitData)
    {
        so = _attackUnitData;
        attackUnitIndex = _attackUnitData.attackIndex;
        isActive = false;
        damage = _attackUnitData.damage;
        aliveTime = _attackUnitData.aliveTime;
        ct = _attackUnitData.ct;
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

        // skill treeによるデータ更新
        foreach (var skillData in SOLoader.SkillTreeData.skillTreeDatas)
        {
            var saveData = await SaveLoader.Inst.Get_SkillTreeData(skillData.index);
            if (saveData == null) continue;
            var setParam = skillData.baseValue + skillData.deltaValue * saveData.level;
            Set_DeltaParam(skillData, setParam);
        }

        await UniTask.DelayFrame(1);
    }

    public static void Init_GameBaseParam()
    {
        gameBaseParam.Set_SkillTreeParam(ParamType.IngameTime, 5f);
        gameBaseParam.Set_SkillTreeParam(ParamType.BonusRate, 0f);

        // block generate param init
        list_blockGenerateParam.Clear();
        foreach (var blockData in SOLoader.BlockData.blockDatas)
        {
            var blockParam = new BlockGenerateParam();
            blockParam.Init(blockData);
            list_blockGenerateParam.Add(blockParam);
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



    public static void Set_DeltaParam(SkillTree _skillTree, float _setParam)
    {
        switch (_skillTree.paramCategory)
        {
            case ParamCategory.GameSystem:
                Set_GamesystemParam(_skillTree, _setParam);
                break;
            case ParamCategory.Block:
                Set_BlockParam(_skillTree, _setParam);
                break;
            case ParamCategory.Attack:
                Set_AttackParam(_skillTree, _setParam);
                break;
        }
    }

    private static void Set_GamesystemParam(SkillTree _skillTree, float _setParam)
    {
        gameBaseParam.Set_SkillTreeParam(_skillTree.paramType, _setParam);
    }

    private static void Set_BlockParam(SkillTree _skillTree, float _setParam)
    {
        var targetBlock = list_blockGenerateParam.Find(x => x.blockIndex == _skillTree.targetIndex);
        if (targetBlock == null)
        {
            Debug.LogError($"BlockData is not found: {_skillTree.targetIndex} // ==> 初期ロードで読み込み失敗");
            return;
        }

        targetBlock.Set_SkillTreeParam(_skillTree.paramType, _setParam);
    }


    private static void Set_AttackParam(SkillTree _skillTree, float _setParam)
    {
        var targetAttack = list_attackParam.Find(x => x.attackUnitIndex == _skillTree.targetIndex);
        if (targetAttack == null)
        {
            Debug.LogError($"AttackUnitData is not found: {_skillTree.targetIndex} // ==> 初期ロードで読み込み失敗");
        }
        targetAttack.Set_SkillTreeParam(_skillTree.paramType, _setParam);

    }



}
