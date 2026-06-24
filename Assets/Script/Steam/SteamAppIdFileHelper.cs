using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// steam_appid.txt を Steam API が参照しうる場所へ配置する。
/// Mac Unity Editor では CWD がプロジェクトルートと一致しないことがある。
/// </summary>
public static class SteamAppIdFileHelper
{
    public static void EnsureFiles(uint appId)
    {
        var appIdText = appId.ToString();
        var content = Encoding.ASCII.GetBytes(appIdText);
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        var cwd = Directory.GetCurrentDirectory();

        WriteIfNeeded(Path.Combine(projectRoot ?? string.Empty, "steam_appid.txt"), appIdText, content);
        WriteIfNeeded(Path.Combine(cwd, "steam_appid.txt"), appIdText, content);

#if UNITY_EDITOR_OSX
        var editorMacOsDir = Path.Combine(UnityEditor.EditorApplication.applicationPath, "Contents/MacOS");
        WriteIfNeeded(Path.Combine(editorMacOsDir, "steam_appid.txt"), appIdText, content);
#endif

        Debug.Log(
            $"[Steam] steam_appid.txt ensured. AppId={appId}, projectRoot={projectRoot}, cwd={cwd}");
    }

    private static void WriteIfNeeded(string path, string appIdText, byte[] content)
    {
        if (string.IsNullOrEmpty(path)) return;

        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) return;

        try
        {
            if (File.Exists(path) && File.ReadAllText(path).Trim() == appIdText) return;

            File.WriteAllBytes(path, content);
            Debug.Log($"[Steam] Wrote steam_appid.txt -> {path}");
        }
        catch (IOException ex)
        {
            Debug.LogWarning($"[Steam] Could not write steam_appid.txt to {path}: {ex.Message}");
        }
    }
}
