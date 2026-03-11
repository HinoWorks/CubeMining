using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Xml.Serialization;
using Unity.VisualScripting;


/// <summary>
/// ゲームの基本パラメータ
/// </summary>
public class GameBaseParam
{
    // インゲーム時間
    public float ingameTime => ingameTime_Base + ingameTime_enhanced;
    private float ingameTime_Base = 15f;
    private float ingameTime_enhanced = 0f;

    //再生成確率xアップ(上から降ってくる)
    // TODO here


    //ラッキーマイン - 採掘量が1.5倍になる確率x
    public float luckyMineRate => 0 + luckyMineRate_enhanced;
    private float luckyMineRate_enhanced = 0f;

    //ラッキーマインの増加量+50%
    public float luckyMineRate_ResourceUpRate => luckyMineRate_ResourceUpRate_enhanced;
    private float luckyMineRate_ResourceUpRate_enhanced = 0f;

    //下層ボーナスxx
    public float deepLayer_Bonus => 0 + deepLayer_Bonus_enhanced;
    private float deepLayer_Bonus_enhanced = 0f;



    // ピッケルの基礎パラメータ向上
    public float pickaxeBase_AttackDamage => pickaxeBase_AttackDamage_enhanced;
    private float pickaxeBase_AttackDamage_enhanced = 0f;
    public float pickaxeBase_AttackInterval => pickaxeBase_AttackInterval_enhanced;
    private float pickaxeBase_AttackInterval_enhanced = 0f;
    public float pickaxeBase_CriticalRate => pickaxeBase_CriticalRate_enhanced;
    private float pickaxeBase_CriticalRate_enhanced = 0f;
    public float pickaxeBase_ResourceUpRate => pickaxeBase_ResourceUpRate_enhanced;
    private float pickaxeBase_ResourceUpRate_enhanced = 0f;
    public float pickaxeBase_Size => pickaxeBase_Size_enhanced;
    private float pickaxeBase_Size_enhanced = 0f;


    public void Set_Param(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.IngameTime:
                ingameTime_enhanced += _setParam;
                break;
            case ParamType.LuckyMineRate:
                luckyMineRate_enhanced += _setParam;
                break;
            case ParamType.LuckyMineRate_Resource:
                luckyMineRate_ResourceUpRate_enhanced += _setParam;
                break;
            case ParamType.DeepLayerBonus:
                deepLayer_Bonus_enhanced += _setParam;
                break;

            // -- ピッケルの基礎パラメータ向上 --
            case ParamType.Damage:
                pickaxeBase_AttackDamage_enhanced += _setParam;
                break;
            case ParamType.Interval:
                pickaxeBase_AttackInterval_enhanced += _setParam;
                break;
            case ParamType.CriticalRate:
                pickaxeBase_CriticalRate_enhanced += _setParam;
                break;
            case ParamType.ResourceRate:
                pickaxeBase_ResourceUpRate_enhanced += _setParam;
                break;
            case ParamType.Size:
                pickaxeBase_Size_enhanced += _setParam;
                break;
        }
    }
}

/// <summary>
/// ブロック以外のオブジェクト生成パラメータ
/// </summary>
public class ObjectGenerateParam
{
    public ObjectUnitData so;

    public bool isActive { get; private set; } = false;
    public float generateRate_total => generateRate_base + generateRate_enhanced;
    public float valueRate_total => valueRate_base + valueRate_enhanced;
    public float damageRate_total => 1f + damageRate_enhanced;

    private int generateRate_base = 0;
    private float valueRate_base = 0;
    private int generateRate_enhanced = 0;
    private float valueRate_enhanced = 0;
    private float damageRate_enhanced = 0;

    public void Init(ObjectUnitData _objectUnitData)
    {
        so = _objectUnitData;
        generateRate_base = _objectUnitData.generateRate;
        valueRate_base = _objectUnitData.valueRate;
    }
    public void Set_Param(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Unlock:
                isActive = true;
                break;
            case ParamType.Rate_Generate:
                generateRate_enhanced += (int)_setParam;
                break;
            case ParamType.Rate_Value:
                valueRate_enhanced += _setParam;
                break;
            case ParamType.Damage:
                damageRate_enhanced += _setParam;
                break;
        }
    }
}


/// <summary>
/// ベースブロックの基本パラメータ
/// </summary>
public class BlockBaseParam
{
    public BlockData so;
    //public bool isActive { get; private set; } = false;
    public int blockIndex => so.blockIndex;
    public int hp => so.hp + hp_enhanced;
    private int hp_enhanced = 0;
    public int baseValue => so.baseValue + baseValue_enhanced;
    private int baseValue_enhanced = 0;

    public void Init(BlockData _blockData)
    {
        so = _blockData;
        //isActive = false;
    }
    public void Set_Param(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Unlock:
                //isActive = true;
                break;
            case ParamType.Value:
                baseValue_enhanced += (int)_setParam;
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
    public int layerMin => so.layerMin;
    public int layerMax => so.layerMax;
    public int layerSize => so.layerSize;
    public void Init(BlockLayerData _blockLayerData)
    {
        so = _blockLayerData;
    }
    /// <summary>
    /// ブロックのインデックスをランダムで選択
    /// </summary>
    public int SelectBlockIndex()
    {
        var random = UnityEngine.Random.Range(0f, 1f);
        switch (random)
        {
            case var _ when random < so.rate_block1:
                return 1;
            case var _ when random < so.rate_block1 + so.rate_block2:
                return 2;
            case var _ when random < so.rate_block1 + so.rate_block2 + so.rate_block3:
                return 3;
            case var _ when random < so.rate_block1 + so.rate_block2 + so.rate_block3 + so.rate_block4:
                return 4;
            case var _ when random < so.rate_block1 + so.rate_block2 + so.rate_block3 + so.rate_block4 + so.rate_block5:
                return 5;
            case var _ when random < so.rate_block1 + so.rate_block2 + so.rate_block3 + so.rate_block4 + so.rate_block5 + so.rate_block6:
                return 6;
            default:
                return 0;
        }
    }
}

/// <summary>
/// 共通　 = ブロックの変化率パラメータ == 土、岩などのブロックタイプ毎に鉱石の抽選率を設定
/// </summary>
public class BlockChangeRateParam
{
    private int baseRate = 100;
    public int rate_iron_enhanced { get; private set; } = 0;
    public int rate_gold_enhanced { get; private set; } = 0;
    public int rate_emerald_enhanced { get; private set; } = 0;
    public int rate_ruby_enhanced { get; private set; } = 0;
    public int rate_sapphire_enhanced { get; private set; } = 0;
    public int rate_diamond_enhanced { get; private set; } = 0;

    private bool isActive_gold = false;
    private bool isActive_emerald = false;
    private bool isActive_ruby = false;
    private bool isActive_sapphire = false;
    private bool isActive_diamond = false;

    private BlockChangeRateData blockChangeData;
    private int rate_iron_total => rate_iron_enhanced + blockChangeData.rate_iron;
    private int rate_gold_total => isActive_gold ? rate_gold_enhanced + blockChangeData.rate_gold : 0;
    private int rate_emerald_total => isActive_emerald ? rate_emerald_enhanced + blockChangeData.rate_emerald : 0;
    private int rate_ruby_total => isActive_ruby ? rate_ruby_enhanced + blockChangeData.rate_ruby : 0;
    private int rate_sapphire_total => isActive_sapphire ? rate_sapphire_enhanced + blockChangeData.rate_sapphire : 0;
    private int rate_diamond_total => isActive_diamond ? rate_diamond_enhanced + blockChangeData.rate_diamond : 0;

    public void Init()
    {
    }
    public void Set_Param(ParamType _paramType, int _targetBlockIndex, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Unlock:
                switch (_targetBlockIndex)
                {
                    case 2: isActive_gold = true; break;
                    case 3: isActive_emerald = true; break;
                    case 4: isActive_ruby = true; break;
                    case 5: isActive_sapphire = true; break;
                    case 6: isActive_diamond = true; break;
                }
                break;
            case ParamType.Value:
                switch (_targetBlockIndex)
                {
                    case 1: rate_iron_enhanced += (int)_setParam; break;
                    case 2: rate_gold_enhanced += (int)_setParam; break;
                    case 3: rate_emerald_enhanced += (int)_setParam; break;
                    case 4: rate_ruby_enhanced += (int)_setParam; break;
                    case 5: rate_sapphire_enhanced += (int)_setParam; break;
                    case 6: rate_diamond_enhanced += (int)_setParam; break;
                }
                break;
        }
    }
    public ResourceType SelectBlockType(BlockChangeRateData _blockChangeData)
    {
        blockChangeData = _blockChangeData;
        var total = baseRate + rate_iron_total + rate_gold_total + rate_emerald_total + rate_ruby_total + rate_sapphire_total + rate_diamond_total;
        var random = Random.Range(0, total);
        switch (random)
        {
            case var _ when random < rate_iron_total:
                return ResourceType.Iron;
            case var _ when random < rate_iron_total + rate_gold_total:
                return ResourceType.Gold;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total:
                return ResourceType.Emerald;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total + rate_ruby_total:
                return ResourceType.Ruby;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total + rate_ruby_total + rate_sapphire_total:
                return ResourceType.Sapphire;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total + rate_ruby_total + rate_sapphire_total + rate_diamond_total:
                return ResourceType.Diamond;
            default:
                return ResourceType.Stone;
        }
    }
    public bool IsBlockTypeUnlock(ResourceType _resourceType)
    {
        switch (_resourceType)
        {
            case ResourceType.Gold: return isActive_gold;
            case ResourceType.Emerald: return isActive_emerald;
            case ResourceType.Ruby: return isActive_ruby;
            case ResourceType.Sapphire: return isActive_sapphire;
            case ResourceType.Diamond: return isActive_diamond;
            default: return true;
        }
    }

    /*
    public BlockChangeRateData so;
    private int baseRate;
    private int rate_iron_enhanced;
    private int rate_gold_enhanced;
    private int rate_emerald_enhanced;
    private int rate_ruby_enhanced;
    private int rate_sapphire_enhanced;
    private int rate_diamond_enhanced;
    private int rate_iron_total => rate_iron_enhanced + so.rate_iron;
    private int rate_gold_total => rate_gold_enhanced + so.rate_gold;
    private int rate_emerald_total => rate_emerald_enhanced + so.rate_emerald;
    private int rate_ruby_total => rate_ruby_enhanced + so.rate_ruby;
    private int rate_sapphire_total => rate_sapphire_enhanced + so.rate_sapphire;
    private int rate_diamond_total => rate_diamond_enhanced + so.rate_diamond;

    public void Init(BlockChangeRateData _blockChangeRateData)
    {
        so = _blockChangeRateData;
        baseRate = _blockChangeRateData.baseRate;
    }
    public void Set_Param(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Rate_ChangeGold: rate_gold_enhanced += (int)_setParam; break;
            case ParamType.Rate_ChangeIron: rate_iron_enhanced += (int)_setParam; break;
            case ParamType.Rate_ChangeEmerald: rate_emerald_enhanced += (int)_setParam; break;
            case ParamType.Rate_ChangeRuby: rate_ruby_enhanced += (int)_setParam; break;
            case ParamType.Rate_ChangeSapphire: rate_sapphire_enhanced += (int)_setParam; break;
            case ParamType.Rate_ChangeDiamond: rate_diamond_enhanced += (int)_setParam; break;
        }
    }
    public ResourceType SelectBlockType()
    {
        var total = baseRate + rate_iron_total + rate_gold_total + rate_emerald_total
            + rate_ruby_total + rate_sapphire_total + rate_diamond_total;
        var random = Random.Range(0, total);
        switch (random)
        {
            case var _ when random < rate_iron_total:
                return ResourceType.Iron;
            case var _ when random < rate_iron_total + rate_gold_total:
                return ResourceType.Gold;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total:
                return ResourceType.Emerald;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total + rate_ruby_total:
                return ResourceType.Ruby;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total + rate_ruby_total + rate_sapphire_total:
                return ResourceType.Sapphire;
            case var _ when random < rate_iron_total + rate_gold_total + rate_emerald_total + rate_ruby_total + rate_sapphire_total + rate_diamond_total:
                return ResourceType.Diamond;
            default:
                return ResourceType.Stone;
        }
    }
    */
}



/// <summary>
/// アーティファクトの生成率パラメータ --- インゲーム開始時に設定、ゲーム中は変更不可
/// </summary>
public class ArtifactGenerateRateParam
{
    private ArtifactGenerateRateData so;
    public float generateRate { get; private set; } = 0f;
    public void Init()
    {
        var alredyArtifactCount = SaveLoader.Inst.Get_ArtifactTotalCount();
        so = SOLoader.ArtifactData.Get_ArtifactGenerateRateData(alredyArtifactCount);
        var currentBlockCount = SaveLoader.Inst.ArtifactCurrentBlockCount;

        generateRate = so.baseRate + so.deltaRate * currentBlockCount / so.deltaInterval;
        Debug.Log($"<color=green>=** baseRate: {so.baseRate} / deltaRate: {so.deltaRate} / deltaInterval: {so.deltaInterval} / currentBlockCount: {currentBlockCount} => artifactGenerateRate: {generateRate} **=</color>");
    }
}


/// <summary>
/// ピッケルのパラメータ
/// </summary>
public class PickaxeParam
{
    public PickaxeUnitData so;
    public int damage;
    public float attackInterval;
    public float criticalRate;
    public float resourceUpRate;
    public float size;

    public void Init(PickaxeUnitData _pickaxeUnitData)
    {
        so = _pickaxeUnitData;
        damage = _pickaxeUnitData.damage;
        attackInterval = _pickaxeUnitData.attackInterval;
        criticalRate = _pickaxeUnitData.criticalRate;
        resourceUpRate = _pickaxeUnitData.resourceUpRate;
        size = _pickaxeUnitData.size;
    }
}




/// <summary>
/// アタックユニットのパラメータ
/// </summary>
public class AttackParam
{
    public AttackUnitData so;
    public bool isActive { get; private set; } = false;
    public int attackUnitIndex => so.attackIndex;

    public float damageRate => damageRate_enhanced + so.damageRate;
    public float attackInterval => (1f - attackInterval_enhanced) * so.attackInterval;
    public float criticalRate => criticalRate_enhanced + so.criticalRate;
    public float size => size_enhanced + so.size;
    public float aliveTime => so.aliveTime;
    public float speed => so.speed + speed_enhanced;
    public int count => count_enhanced + so.count;

    private float damageRate_enhanced = 0f;
    private float speed_enhanced = 0f;
    private int count_enhanced = 0;
    private float attackInterval_enhanced = 0f;
    private float criticalRate_enhanced = 0f;
    private float size_enhanced = 0f;


    public void Init(AttackUnitData _attackUnitData)
    {
        so = _attackUnitData;
    }

    public void Set_Param(ParamType _paramType, float _setParam)
    {
        switch (_paramType)
        {
            case ParamType.Unlock:
                isActive = true;
                break;
            case ParamType.Damage:
                damageRate_enhanced += _setParam;
                break;
            case ParamType.Interval:
                attackInterval_enhanced += _setParam;
                break;
            case ParamType.Speed:
                speed_enhanced += _setParam;
                break;
            case ParamType.Count:
                count_enhanced += (int)_setParam;
                break;
            case ParamType.Size:
                size_enhanced += _setParam;
                break;
            case ParamType.CriticalRate:
                criticalRate_enhanced += _setParam;
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
    public readonly static ArtifactGenerateRateParam artifactGenerateRateParam = new ArtifactGenerateRateParam();
    public readonly static BlockChangeRateParam blockChangeRateParam = new BlockChangeRateParam();

    public readonly static List<ObjectGenerateParam> list_objectGenerateParam = new List<ObjectGenerateParam>();
    public readonly static List<BlockBaseParam> list_blockGenerateParam = new List<BlockBaseParam>();
    public readonly static List<BlockGenerateParam_Layer> list_blockGenerateParam_Layer = new List<BlockGenerateParam_Layer>();
    //public readonly static List<BlockChangeRateParam> list_blockChangeRateParam = new List<BlockChangeRateParam>();
    public readonly static List<AttackParam> list_attackParam = new List<AttackParam>();
    public readonly static List<PickaxeParam> list_pickaxeParam = new List<PickaxeParam>();
    public static float artifactGenerateRate => artifactGenerateRateParam.generateRate;
    public static int otherObjectRate { get; private set; } = 0;
    public static int otherObjectBaseRate { get; private set; } = 100;



    #region get param reference
    public static BlockBaseParam Get_BlockGenerateParam(int _blockIndex)
    {
        var targetBlock = list_blockGenerateParam.Find(x => x.blockIndex == _blockIndex);
        if (targetBlock == null)
        {
            Debug.LogError($"BlockData is not found: {_blockIndex} // ==> 初期ロードで読み込み失敗");
        }
        return targetBlock;
    }
    public static ResourceType Get_RandamBlockType(int _blockIndex)
    {
        var blockChangeBaseParam = SOLoader.BlockData.GetBlockChangeRateData(_blockIndex);
        var selectType = blockChangeRateParam.SelectBlockType(blockChangeBaseParam);
        return selectType;
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
    public static PickaxeParam Get_PickaxeParam(int _pickaxeIndex)
    {
        var targetPickaxe = list_pickaxeParam.Find(x => x.so.pickaxeIndex == _pickaxeIndex);
        if (targetPickaxe == null)
        {
            Debug.LogError($"PickaxeUnitData is not found: {_pickaxeIndex} // ==> 初期ロードで読み込み失敗");
        }
        return targetPickaxe;
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
    private static void Set_OtherObjectRate()
    {
        otherObjectRate = 0;
        foreach (var objectParam in list_objectGenerateParam)
        {
            if (!objectParam.isActive) continue;
            otherObjectRate += (int)objectParam.generateRate_total;
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
            if (!objectParam.isActive) continue;
            currentRate += (int)objectParam.generateRate_total;
            if (random < currentRate)
            {
                return objectParam;
            }
        }
        return null;
    }
    #endregion



    #region -- other object generate param --
    public static bool IsArtifactGenerate()
    {
        var random = UnityEngine.Random.Range(0f, 1f);
        //Debug.Log($"otherObjectRate: {otherObjectRate} / {otherObjectBaseRate + otherObjectRate} / {random} => {random < otherObjectRate}");
        return random < artifactGenerateRate;
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

    /// <summary>
    /// インゲーム開始時に更新するパラメータはここに記載
    /// </summary>
    public static void Init_IngameStart()
    {
        artifactGenerateRateParam.Init();
    }

    public static void Init_GameBaseParam()
    {
        // block generate param init
        list_blockGenerateParam.Clear();
        foreach (var blockData in SOLoader.BlockData.blockDatas)
        {
            var blockParam = new BlockBaseParam();
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
        blockChangeRateParam.Init();

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

        // pickaxe param init
        list_pickaxeParam.Clear();
        foreach (var pickaxeData in SOLoader.AttackUnitData.pickaxeUnitDatas)
        {
            var pickaxeParam = new PickaxeParam();
            pickaxeParam.Init(pickaxeData);
            list_pickaxeParam.Add(pickaxeParam);
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
                Set_BlockChangeRateParam(_paramType, _targetIndex, _setParam);
                break;
            case ParamCategory.OtherBlock:
                Set_ObjectGenerateParam(_targetIndex, _paramType, _setParam);
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
    private static void Set_BlockChangeRateParam(ParamType _paramType, int _targetBlockIndex, float _setParam)
    {
        blockChangeRateParam.Set_Param(_paramType, _targetBlockIndex, _setParam);
    }
    private static void Set_ObjectGenerateParam(int _objectIndex, ParamType _paramType, float _setParam)
    {
        var targetObject = list_objectGenerateParam.Find(x => x.so.objectIndex == _objectIndex);
        if (targetObject == null)
        {
            Debug.LogError($"ObjectUnitData is not found: {_objectIndex} // ==> 初期ロードで読み込み失敗");
        }
        targetObject.Set_Param(_paramType, _setParam);
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
