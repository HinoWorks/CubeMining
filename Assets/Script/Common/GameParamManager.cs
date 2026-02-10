using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Xml.Serialization;


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


    public void Set_Param(ParamType _paramType, float _setParam)
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
    public void Set_Param(ParamType _paramType, float _setParam)
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

    public void Set_Param(ParamType _paramType, float _setParam)
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
/// ブロックの生成パラメータ == レイヤー毎にブロックの抽選率を設定
/// </summary>
public class BlockGenerateParam_Layer
{
    public BlockLayerData so;
    public int layerMin;
    public int layerMax;
    public int layerSize;
    public float rate_block1;
    public float rate_block2;
    public float rate_block3;
    public float rate_block4;
    public float rate_block5;
    public float rate_block6;


    public void Init(BlockLayerData _blockLayerData)
    {
        so = _blockLayerData;
        layerMin = _blockLayerData.layerMin;
        layerMax = _blockLayerData.layerMax;
        layerSize = _blockLayerData.layerSize;
        rate_block1 = _blockLayerData.rate_block1;
        rate_block2 = _blockLayerData.rate_block2;
        rate_block3 = _blockLayerData.rate_block3;
        rate_block4 = _blockLayerData.rate_block4;
        rate_block5 = _blockLayerData.rate_block5;
        rate_block6 = _blockLayerData.rate_block6;
    }
    /// <summary>
    /// ブロックのインデックスをランダムで選択
    /// </summary>
    public int SelectBlockIndex()
    {
        var random = UnityEngine.Random.Range(0f, 1f);
        switch (random)
        {
            case var _ when random < rate_block1:
                return 1;
            case var _ when random < rate_block1 + rate_block2:
                return 2;
            case var _ when random < rate_block1 + rate_block2 + rate_block3:
                return 3;
            case var _ when random < rate_block1 + rate_block2 + rate_block3 + rate_block4:
                return 4;
            case var _ when random < rate_block1 + rate_block2 + rate_block3 + rate_block4 + rate_block5:
                return 5;
            case var _ when random < rate_block1 + rate_block2 + rate_block3 + rate_block4 + rate_block5 + rate_block6:
                return 6;
            default:
                return 0;
        }
    }
}


public enum BlockType
{
    None, Gold, Iron, Emerald,
    Rate_4, Rate_5, Rate_6,
}
/// <summary>
/// ブロックの変化率パラメータ == 土、岩などのブロックタイプ毎に鉱石の抽選率を設定
/// </summary>
public class BlockChangeRateParam
{
    public BlockChangeRateData so;
    public int baseRate;
    public int rate_gold;
    public int rate_iron;
    public int rate_emerald;
    public int rate_ruby;
    public int rate_sapphire;
    public int rate_diamond;

    public void Init(BlockChangeRateData _blockChangeRateData)
    {
        so = _blockChangeRateData;
        baseRate = _blockChangeRateData.baseRate;
        rate_gold = _blockChangeRateData.rate_gold;
        rate_iron = _blockChangeRateData.rate_iron;
        rate_emerald = _blockChangeRateData.rate_emerald;
        rate_ruby = _blockChangeRateData.rate_ruby;
        rate_sapphire = _blockChangeRateData.rate_sapphire;
        rate_diamond = _blockChangeRateData.rate_diamond;
    }
    public BlockType SelectBlockType()
    {
        var total = baseRate + rate_gold + rate_iron + rate_emerald + rate_ruby + rate_sapphire + rate_diamond;
        var random = UnityEngine.Random.Range(0, total);
        switch (random)
        {
            case var _ when random < rate_iron:
                return BlockType.Iron;
            case var _ when random < rate_iron + rate_gold:
                return BlockType.Gold;
            case var _ when random < rate_iron + rate_gold + rate_emerald:
                return BlockType.Emerald;
            case var _ when random < rate_iron + rate_gold + rate_emerald + rate_ruby:
                return BlockType.Rate_4;
            case var _ when random < rate_iron + rate_gold + rate_emerald + rate_ruby + rate_sapphire:
                return BlockType.Rate_5;
            case var _ when random < rate_iron + rate_gold + rate_emerald + rate_ruby + rate_sapphire + rate_diamond:
                return BlockType.Rate_6;
            default:
                return BlockType.None;
        }
    }

    public void Set_Param(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Rate_Gold:
                rate_gold += (int)_setParam;
                break;
            case ParamType.Rate_Iron:
                rate_iron += (int)_setParam;
                break;
            case ParamType.Rate_Emerald:
                rate_emerald += (int)_setParam;
                break;
            case ParamType.Rate_4:
                rate_ruby += (int)_setParam;
                break;
            case ParamType.Rate_5:
                rate_sapphire += (int)_setParam;
                break;
            case ParamType.Rate_6:
                rate_diamond += (int)_setParam;
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

    public void Set_Param(ParamType _paramType, float _setParam)
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
    public readonly static List<BlockGenerateParam_Layer> list_blockGenerateParam_Layer = new List<BlockGenerateParam_Layer>();
    public readonly static List<BlockChangeRateParam> list_blockChangeRateParam = new List<BlockChangeRateParam>();
    public readonly static List<AttackParam> list_attackParam = new List<AttackParam>();

    public static int otherObjectRate { get; private set; } = 0;
    public static int otherObjectBaseRate { get; private set; } = 100;



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
    public static BlockChangeRateParam Get_BlockChangeRateParam(int _blockIndex)
    {
        var targetBlockChangeRate = list_blockChangeRateParam.Find(x => x.so.blockIndex == _blockIndex);
        if (targetBlockChangeRate == null)
        {
            Debug.LogError($"BlockChangeRateData is not found: {_blockIndex} // ==> 初期ロードで読み込み失敗");
        }
        return targetBlockChangeRate;
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
    public static BlockGenerateParam_Layer Get_BlockGenerateParam_Layer(int _layerIndex)
    {
        var targetLayer = list_blockGenerateParam_Layer.Find(x => x.layerMin <= _layerIndex && x.layerMax > _layerIndex);
        if (targetLayer == null)
        {
            Debug.LogError($"BlockLayerData is not found: {_layerIndex} // ==> 初期ロードで読み込み失敗");
        }
        return targetLayer;
    }
    #endregion

    #region -- other object generate param --
    public static void Set_OtherObjectRate()
    {
        otherObjectRate = 0;
        foreach (var objectParam in list_objectGenerateParam)
        {
            otherObjectRate += (int)objectParam.generateRate;
        }
    }
    public static bool IsOtherObjectGenerate()
    {
        var random = UnityEngine.Random.Range(0, otherObjectBaseRate + otherObjectRate);
        //Debug.Log($"otherObjectRate: {otherObjectRate} / {otherObjectBaseRate + otherObjectRate} / {random} => {random < otherObjectRate}");
        return random < otherObjectRate;
    }
    public static ObjectGenerateParam SelectOtherObject()
    {
        var random = UnityEngine.Random.Range(0, otherObjectRate);
        var currentRate = 0;
        foreach (var objectParam in list_objectGenerateParam)
        {
            currentRate += (int)objectParam.generateRate;
            if (random < currentRate)
            {
                return objectParam;
            }
        }
        return null;
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

        // block generate param layer init
        list_blockGenerateParam_Layer.Clear();
        foreach (var blockLayerData in SOLoader.BlockLayerData.blockLayerDatas)
        {
            var blockGenerateParam_Layer = new BlockGenerateParam_Layer();
            blockGenerateParam_Layer.Init(blockLayerData);
            list_blockGenerateParam_Layer.Add(blockGenerateParam_Layer);
        }
        // block change rate param init
        list_blockChangeRateParam.Clear();
        foreach (var blockChangeRateData in SOLoader.BlockData.blockChangeRateDatas)
        {
            var blockChangeRateParam = new BlockChangeRateParam();
            blockChangeRateParam.Init(blockChangeRateData);
            list_blockChangeRateParam.Add(blockChangeRateParam);
        }

        // object generate param init
        list_objectGenerateParam.Clear();
        foreach (var objectData in SOLoader.ObjectUnitData.objectUnitDatas)
        {
            var objectParam = new ObjectGenerateParam();
            objectParam.Init(objectData);
            list_objectGenerateParam.Add(objectParam);
        }
        Set_OtherObjectRate();

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
        for (int i = 0; i < StaticManager.artifactSlotCount; i++)
        {
            var saveData = await SaveLoader.Inst.Get_ArtifactSlotData(i);
            if (saveData == null) continue;
            var artifactData = SOLoader.ArtifactData.artifactDatas[saveData.equipedArtifactIndex];
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
            case ParamCategory.BlockChangeRate:
                Set_BlockChangeRateParam(_targetIndex, _paramType, _setParam);
                break;
            case ParamCategory.OtherObject:
                Set_BlockParam(_targetIndex, _paramType, _setParam);
                Set_OtherObjectRate();
                break;
            case ParamCategory.Attack:
                Set_AttackParam(_targetIndex, _paramType, _setParam);
                break;
        }
    }

    private static void Set_GamesystemParam(ParamType _paramType, float _setParam)
    {
        gameBaseParam.Set_Param(_paramType, _setParam);
    }
    private static void Set_BlockParam(int _blockIndex, ParamType _paramType, float _setParam)
    {
        var targetBlock = list_blockGenerateParam.Find(x => x.blockIndex == _blockIndex);
        if (targetBlock == null)
        {
            Debug.LogError($"BlockData is not found: {_blockIndex} // ==> 初期ロードで読み込み失敗");
            return;
        }
        targetBlock.Set_Param(_paramType, _setParam);
    }
    private static void Set_BlockChangeRateParam(int _blockIndex, ParamType _paramType, float _setParam)
    {
        var targetBlockChangeRate = list_blockChangeRateParam.Find(x => x.so.blockIndex == _blockIndex);
        if (targetBlockChangeRate == null)
        {
            Debug.LogError($"BlockData is not found: {_blockIndex} // ==> 初期ロードで読み込み失敗");
            return;
        }
        targetBlockChangeRate.Set_Param(_paramType, _setParam);
    }
    private static void Set_AttackParam(int _attackIndex, ParamType _paramType, float _setParam)
    {
        var targetAttack = list_attackParam.Find(x => x.attackUnitIndex == _attackIndex);
        if (targetAttack == null)
        {
            Debug.LogError($"AttackUnitData is not found: {_attackIndex} // ==> 初期ロードで読み込み失敗");
        }
        targetAttack.Set_Param(_paramType, _setParam);
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
        targetAttack.Set_Param(ParamType.Unlock, 1f);
    }

    #endregion

}
