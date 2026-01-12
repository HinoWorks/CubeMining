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
