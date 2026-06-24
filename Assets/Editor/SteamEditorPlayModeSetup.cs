#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Play 開始前に steam_appid.txt を配置する（Mac Editor 向け）。
/// </summary>
[InitializeOnLoad]
internal static class SteamEditorPlayModeSetup
{
    private const uint DefaultAppId = 480;

    static SteamEditorPlayModeSetup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode) return;
        SteamAppIdFileHelper.EnsureFiles(ReadAppId());
    }

    private static uint ReadAppId()
    {
        var path = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "steam_appid.txt");
        if (!File.Exists(path)) return DefaultAppId;

        var text = File.ReadAllText(path).Trim();
        return uint.TryParse(text, out var appId) ? appId : DefaultAppId;
    }
}
#endif
