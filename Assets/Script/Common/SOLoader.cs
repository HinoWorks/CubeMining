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




    // ========= loc =============
    private static T LoadSO<T>(string path) where T : ScriptableObject
    {
        return Resources.Load<T>(path);
    }
}
