using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーザー設定（音声・表示）の保持・適用・永続化を管理する
/// </summary>
[DefaultExecutionOrder(-100)]
public class UserSettingsManager : MonoBehaviour
{
    public static UserSettingsManager Inst;

    private const string KEY_USER_SETTINGS = "key_userSettings";
    private const string KEY_SOUND_SETTINGS_LEGACY = "key_soundSettings";

    public UserSettingsData Data { get; private set; }

    public float VolumeMaster => Data.volumeMaster;
    public float VolumeBGM => Data.volumeBGM;
    public float VolumeSE => Data.volumeSE;
    public bool MuteMaster => Data.muteMaster;
    public bool MuteBGM => Data.muteBGM;
    public bool MuteSE => Data.muteSE;
    public int ResolutionWidth => Data.resolutionWidth;
    public int ResolutionHeight => Data.resolutionHeight;
    public FullScreenMode ScreenMode => (FullScreenMode)Data.fullScreenMode;

    private bool isLoading;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplyAll();
    }

    #region -- Public API (設定UIから呼ぶ) --
    public void SetVolumeMaster(float value)
    {
        Data.volumeMaster = Mathf.Clamp(value, 0f, 100f);
        ApplySound(Data);
        SaveSettings();
    }

    public void SetVolumeBGM(float value)
    {
        Data.volumeBGM = Mathf.Clamp(value, 0f, 100f);
        ApplySound(Data);
        SaveSettings();
    }

    public void SetVolumeSE(float value)
    {
        Data.volumeSE = Mathf.Clamp(value, 0f, 100f);
        ApplySound(Data);
        SaveSettings();
    }

    public void SetMuteMaster(bool mute)
    {
        Data.muteMaster = mute;
        ApplySound(Data);
        SaveSettings();
    }

    public void SetMuteBGM(bool mute)
    {
        Data.muteBGM = mute;
        ApplySound(Data);
        SaveSettings();
    }

    public void SetMuteSE(bool mute)
    {
        Data.muteSE = mute;
        ApplySound(Data);
        SaveSettings();
    }

    public void SetResolution(int width, int height)
    {
        Data.resolutionWidth = width;
        Data.resolutionHeight = height;
        ApplyDisplay(Data);
        SaveSettings();
    }

    public void SetScreenMode(FullScreenMode mode)
    {
        Data.fullScreenMode = (int)mode;
        ApplyDisplay(Data);
        SaveSettings();
    }

    /// <summary>ウィンドウモード時に選択可能な解像度一覧（重複なし）</summary>
    public List<Vector2Int> GetAvailableResolutions()
    {
        var seen = new HashSet<(int, int)>();
        var list = new List<Vector2Int>();
        foreach (var resolution in Screen.resolutions)
        {
            if (seen.Add((resolution.width, resolution.height)))
            {
                list.Add(new Vector2Int(resolution.width, resolution.height));
            }
        }
        list.Sort((a, b) =>
        {
            int cmp = a.x.CompareTo(b.x);
            return cmp != 0 ? cmp : a.y.CompareTo(b.y);
        });
        return list;
    }

    public UserSettingsData GetSnapshot()
    {
        return Data.Copy();
    }

    /// <summary>保存前のプレビュー適用（永続化しない）</summary>
    public void ApplyPreview(UserSettingsData preview)
    {
        ApplySound(preview);
        ApplyDisplay(preview);
    }

    /// <summary>開いた時点の設定に戻す（永続化しない）</summary>
    public void RestoreSnapshot(UserSettingsData snapshot)
    {
        Data = snapshot.Copy();
        ApplyAll();
    }

    /// <summary>設定を確定して永続化する</summary>
    public void CommitSettings(UserSettingsData settings)
    {
        Data = settings.Copy();
        ApplyAll();
        SaveSettings();
    }
    #endregion

    #region -- Load / Save / Apply --
    private void LoadSettings()
    {
        isLoading = true;
        if (ES3.KeyExists(KEY_USER_SETTINGS))
        {
            Data = ES3.Load<UserSettingsData>(KEY_USER_SETTINGS);
        }
        else if (ES3.KeyExists(KEY_SOUND_SETTINGS_LEGACY))
        {
            var legacy = ES3.Load<SoundSettingsData>(KEY_SOUND_SETTINGS_LEGACY);
            Data = CreateDefault();
            Data.volumeBGM = legacy.volumeBGM;
            Data.volumeSE = legacy.volumeSE;
            Data.muteBGM = legacy.muteBGM;
            Data.muteSE = legacy.muteSE;
            isLoading = false;
            SaveSettings();
            return;
        }
        else
        {
            Data = CreateDefault();
        }
        isLoading = false;
    }

    private static UserSettingsData CreateDefault()
    {
        var current = Screen.currentResolution;
        return new UserSettingsData
        {
            volumeMaster = 100f,
            volumeBGM = 80f,
            volumeSE = 80f,
            resolutionWidth = current.width,
            resolutionHeight = current.height,
            fullScreenMode = (int)FullScreenMode.Windowed,
        };
    }

    private void SaveSettings()
    {
        if (isLoading) return;
        if (SaveLoader.Inst != null)
        {
            SaveLoader.Inst.Request_SaveUserSettings(Data);
        }
        else
        {
            ES3.Save(KEY_USER_SETTINGS, Data);
        }
    }

    public void ApplyAll()
    {
        ApplySound(Data);
        ApplyDisplay(Data);
    }

    private void ApplySound(UserSettingsData settings)
    {
        if (SoundManager.Inst == null) return;
        SoundManager.Inst.ApplySoundSettings(
            settings.volumeMaster,
            settings.volumeBGM,
            settings.volumeSE,
            settings.muteMaster,
            settings.muteBGM,
            settings.muteSE
        );
    }

    private void ApplyDisplay(UserSettingsData settings)
    {
        var mode = (FullScreenMode)settings.fullScreenMode;
        if (mode == FullScreenMode.Windowed)
        {
            Screen.SetResolution(settings.resolutionWidth, settings.resolutionHeight, FullScreenMode.Windowed);
        }
        else
        {
            Screen.SetResolution(settings.resolutionWidth, settings.resolutionHeight, mode);
        }
    }
    #endregion
}
