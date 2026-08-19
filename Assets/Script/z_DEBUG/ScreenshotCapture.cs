using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// フラグON時にCキーでゲーム画面をキャプチャする
/// </summary>
public class ScreenshotCapture : MonoBehaviour
{
    [SerializeField] bool enableCapture;

    void Update()
    {
        if (!enableCapture) return;
        if (Keyboard.current == null) return;
        if (!Keyboard.current.cKey.wasPressedThisFrame) return;

        Capture();
    }

    private void Capture()
    {
        string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "../Screenshots"));
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        string path = Path.Combine(directory, fileName);
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"Screenshot saved: {path}");
    }
}
