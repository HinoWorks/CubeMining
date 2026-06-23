using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;

[CustomEditor(typeof(UI_SkillTreeMaanger))]
public class SkillTreeEditor : Editor
{
    const string DataBasePrefabPath = "Assets/Prefab/z_SO Loader.prefab";

    public override async void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var ui = target as UI_SkillTreeMaanger;
        var t = "SkillTree SO更新 & UI再生成";

        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button(t))
        {
            var master = GetDataBase();
            if (master == null)
            {
                Debug.LogError($"DataBase not found. Check scene or prefab: {DataBasePrefabPath}");
                return;
            }

            await master.SkillTreeData_Update();
            ui.SkillTreeData_Update();
        }
        EditorGUI.EndDisabledGroup();
    }

    static DataBase GetDataBase()
    {
        var inScene = Object.FindFirstObjectByType<DataBase>();
        if (inScene != null) return inScene;

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(DataBasePrefabPath);
        return prefab != null ? prefab.GetComponent<DataBase>() : null;
    }
}
