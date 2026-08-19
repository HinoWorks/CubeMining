using UnityEngine;

/// <summary>
/// ゲーム起動時にカスタムカーソル画像を適用する
/// </summary>
public class CursorManager : MonoBehaviour
{
    public static CursorManager Inst;

    const string DefaultResourcePath = "Cursor/GameCursor";

    [SerializeField] Texture2D cursorTexture;
    [SerializeField] Vector2 hotspot = Vector2.zero;
    [SerializeField] string resourcePath = DefaultResourcePath;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureInstance()
    {
        if (Inst != null) return;
        if (FindFirstObjectByType<CursorManager>() != null) return;
        var go = new GameObject("===CursorManager");
        go.AddComponent<CursorManager>();
    }

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); return; }

        ApplyCustomCursor();
    }

    void OnDestroy()
    {
        if (Inst == this) Inst = null;
    }

    public void ApplyCustomCursor()
    {
        var texture = ResolveTexture();
        if (texture == null) return;

        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    Texture2D ResolveTexture()
    {
        if (cursorTexture != null) return cursorTexture;
        if (string.IsNullOrEmpty(resourcePath)) return null;

        var loaded = Resources.Load<Texture2D>(resourcePath);
        if (loaded == null)
        {
            Debug.LogWarning($"CursorManager: cursor texture not found. Place a Texture2D at Resources/{resourcePath}");
        }
        return loaded;
    }
}
