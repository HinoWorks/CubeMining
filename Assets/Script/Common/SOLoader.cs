using UnityEngine;

public static class SOLoader
{
    private static string pathBase = "SO/";


    // =====================
    private static SO_AttackUnitData so_attackUnitData;
    public static SO_AttackUnitData AttackUnitData
    {
        get
        {
            if (so_attackUnitData == null)
            {
                so_attackUnitData = LoadSO<SO_AttackUnitData>(pathBase + nameof(SO_AttackUnitData));
            }
            return so_attackUnitData;
        }
    }
    private static SO_SubSkillUnitData so_subSkillUnitData;
    public static SO_SubSkillUnitData SubSkillUnitData
    {
        get
        {
            if (so_subSkillUnitData == null)
            {
                so_subSkillUnitData = LoadSO<SO_SubSkillUnitData>(pathBase + nameof(SO_SubSkillUnitData));
            }
            return so_subSkillUnitData;
        }
    }



    private static SO_ObjectUnit so_objectUnit;
    public static SO_ObjectUnit ObjectUnitData
    {
        get
        {
            if (so_objectUnit == null)
            {
                so_objectUnit = LoadSO<SO_ObjectUnit>(pathBase + nameof(SO_ObjectUnit));
            }
            return so_objectUnit;
        }
    }

    private static SO_BlockData so_blockData;
    public static SO_BlockData BlockData
    {
        get
        {
            if (so_blockData == null)
            {
                so_blockData = LoadSO<SO_BlockData>(pathBase + nameof(SO_BlockData));
            }
            return so_blockData;
        }
    }


    private static SO_SkillTreeData so_skillTreeData;
    public static SO_SkillTreeData SkillTreeData
    {
        get
        {
            if (so_skillTreeData == null)
            {
                so_skillTreeData = LoadSO<SO_SkillTreeData>(pathBase + nameof(SO_SkillTreeData));
            }
            return so_skillTreeData;
        }
    }
    private static SO_ArtifactData so_artifactData;
    public static SO_ArtifactData ArtifactData
    {
        get
        {
            if (so_artifactData == null)
            {
                so_artifactData = LoadSO<SO_ArtifactData>(pathBase + nameof(SO_ArtifactData));
            }
            return so_artifactData;
        }
    }

    private static SO_UnlockData so_unlockData;
    public static SO_UnlockData UnlockData
    {
        get
        {
            if (so_unlockData == null)
            {
                so_unlockData = LoadSO<SO_UnlockData>(pathBase + nameof(SO_UnlockData));
            }
            return so_unlockData;
        }
    }

    private static SO_BlockLayerData so_blockLayerData;
    public static SO_BlockLayerData BlockLayerData
    {
        get
        {
            if (so_blockLayerData == null)
            {
                so_blockLayerData = LoadSO<SO_BlockLayerData>(pathBase + nameof(SO_BlockLayerData));
            }
            return so_blockLayerData;
        }
    }


    private static SO_ItemData so_itemData;
    public static SO_ItemData ItemData
    {
        get
        {
            if (so_itemData == null)
            {
                so_itemData = LoadSO<SO_ItemData>(pathBase + nameof(SO_ItemData));
            }
            return so_itemData;
        }
    }
    private static SO_UISetting so_uiSetting;
    public static SO_UISetting UISetting
    {
        get
        {
            if (so_uiSetting == null)
            {
                so_uiSetting = LoadSO<SO_UISetting>(pathBase + nameof(SO_UISetting));
            }
            return so_uiSetting;
        }
    }

    private static SO_SoundData so_soundData;
    public static SO_SoundData SoundData
    {
        get
        {
            if (so_soundData == null)
            {
                so_soundData = LoadSO<SO_SoundData>(pathBase + nameof(SO_SoundData));
            }
            return so_soundData;
        }
    }


    private static SO_MaterialData so_materialData;
    public static SO_MaterialData MaterialData
    {
        get
        {
            if (so_materialData == null)
            {
                so_materialData = LoadSO<SO_MaterialData>(pathBase + nameof(SO_MaterialData));
            }
            return so_materialData;
        }
    }

    private static SO_PlayerLevelData so_playerLevelData;
    public static SO_PlayerLevelData PlayerLevelData
    {
        get
        {
            if (so_playerLevelData == null)
            {
                so_playerLevelData = LoadSO<SO_PlayerLevelData>(pathBase + nameof(SO_PlayerLevelData));
                if (so_playerLevelData == null)
                {
                    so_playerLevelData = ScriptableObject.CreateInstance<SO_PlayerLevelData>();
                }
            }
            return so_playerLevelData;
        }
    }

    private static SO_PickaxePowerData so_pickaxePowerData;
    public static SO_PickaxePowerData PickaxePowerData
    {
        get
        {
            if (so_pickaxePowerData == null)
            {
                so_pickaxePowerData = LoadSO<SO_PickaxePowerData>(pathBase + nameof(SO_PickaxePowerData));
            }
            return so_pickaxePowerData;
        }
    }

    private static SO_BlockGenerateData so_blockGenerateData;
    public static SO_BlockGenerateData BlockGenerateData
    {
        get
        {
            if (so_blockGenerateData == null)
            {
                so_blockGenerateData = LoadSO<SO_BlockGenerateData>(pathBase + nameof(SO_BlockGenerateData));
            }
            return so_blockGenerateData;
        }
    }

    private static SO_EnhanceCoinData so_enhanceCoinData;
    public static SO_EnhanceCoinData EnhanceCoinData
    {
        get
        {
            if (so_enhanceCoinData == null)
            {
                so_enhanceCoinData = LoadSO<SO_EnhanceCoinData>(pathBase + nameof(SO_EnhanceCoinData));
            }
            return so_enhanceCoinData;
        }
    }


    private static SO_AchievementData so_achievementData;
    public static SO_AchievementData AchievementData
    {
        get
        {
            if (so_achievementData == null)
            {
                so_achievementData = LoadSO<SO_AchievementData>(pathBase + nameof(SO_AchievementData));
                if (so_achievementData == null)
                {
                    so_achievementData = ScriptableObject.CreateInstance<SO_AchievementData>();
                }
            }
            return so_achievementData;
        }
    }


    // ========= loc =============
    private static T LoadSO<T>(string path) where T : ScriptableObject
    {
        return Resources.Load<T>(path);
    }
}
