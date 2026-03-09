using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst;

    [Range(0, 100)]
    public float Volume_BGM = 80f;
    [Range(0, 100)]
    public float Volume_Sound = 80f;
    public bool Mute_BGM;
    public bool Mute_SE;

    private const string KEY_SOUND_SETTINGS = "key_soundSettings";
    private const int MAX_SIMULTANEOUS_SE = 8;
    private const float BGM_FADE_DURATION = 0.5f;
    [Tooltip("SE再生時のピッチランダム範囲（連続再生の違和感軽減）")]
    [SerializeField] private float sePitchMin = 0.92f;
    [SerializeField] private float sePitchMax = 1.08f;

    // === AudioSource ===
    private AudioSource BGMsource;
    private AudioSource[] SEsources = new AudioSource[16];
    private SO_SoundElement soundData_BGM;

    // -- SE同時になってしまう現状回避 --
    private float timer;
    private float duration_SE = 0.2f;
    private bool isSEPlayed = false;

    private bool isPaused;
    private bool isLoadingSettings;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }

        LoadSettings();

        // BGM AudioSource
        BGMsource = gameObject.AddComponent<AudioSource>();
        BGMsource.loop = true;

        // SE AudioSource
        for (int i = 0; i < SEsources.Length; i++)
        {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }

        ApplyVolumeAndMute();
    }

    void Start()
    {
        PlayBGM(0);
    }

    void Update()
    {
        if (isSEPlayed)
        {
            timer += Time.deltaTime;
            if (timer >= duration_SE)
            {
                isSEPlayed = false;
                timer = 0;
            }
        }
    }

    #region -- Settings Persistence --
    private void LoadSettings()
    {
        isLoadingSettings = true;
        if (ES3.KeyExists(KEY_SOUND_SETTINGS))
        {
            var data = ES3.Load<SoundSettingsData>(KEY_SOUND_SETTINGS);
            Volume_BGM = data.volumeBGM;
            Volume_Sound = data.volumeSE;
            Mute_BGM = data.muteBGM;
            Mute_SE = data.muteSE;
        }
        isLoadingSettings = false;
    }

    private void SaveSettings()
    {
        if (isLoadingSettings) return;
        if (SaveLoader.Inst != null)
        {
            SaveLoader.Inst.Request_SaveSoundSettings(Volume_BGM, Volume_Sound, Mute_BGM, Mute_SE);
        }
    }

    private void ApplyVolumeAndMute()
    {
        if (BGMsource == null) return;

        BGMsource.mute = Mute_BGM || isPaused;
        float bgmBaseVolume = (soundData_BGM != null) ? soundData_BGM.Volume : 1f;
        BGMsource.volume = (Mute_BGM || isPaused) ? 0f : bgmBaseVolume * (Volume_BGM / 100f);

        for (int i = 0; i < SEsources.Length; i++)
        {
            if (SEsources[i] != null)
            {
                SEsources[i].mute = Mute_SE;
            }
        }
    }

    /// <summary>音量・ミュート変更時に呼ぶ（設定UIから）</summary>
    public void SetVolumeBGM(float value)
    {
        Volume_BGM = Mathf.Clamp(value, 0, 100);
        ChangeVolume_ForBGM();
        SaveSettings();
    }

    public void SetVolumeSE(float value)
    {
        Volume_Sound = Mathf.Clamp(value, 0, 100);
        SaveSettings();
    }

    public void SetMuteBGM(bool mute)
    {
        Mute_BGM = mute;
        ApplyVolumeAndMute();
        SaveSettings();
    }

    public void SetMuteSE(bool mute)
    {
        Mute_SE = mute;
        ApplyVolumeAndMute();
        SaveSettings();
    }

    /// <summary>一時停止時に呼ぶ（ポーズ画面など）</summary>
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        if (BGMsource == null) return;
        if (paused)
            BGMsource.Pause();
        else
            BGMsource.UnPause();
    }
    #endregion

    #region -- BGM --
    public void PlayBGM(int _index)
    {
        var getData = SOLoader.SoundData.Get_SoundData_BGM(_index);
        if (getData == null) return;

        BGMsource.DOKill();
        BGMsource.Stop();
        soundData_BGM = getData;
        BGMsource.clip = soundData_BGM.clip;
        BGMsource.Play();
        ChangeVolume_ForBGM();
    }

    /// <summary>BGMをフェード付きで再生</summary>
    public void PlayBGMWithFade(int _index, float fadeDuration = -1f)
    {
        var d = fadeDuration > 0 ? fadeDuration : BGM_FADE_DURATION;
        var getData = SOLoader.SoundData.Get_SoundData_BGM(_index);
        if (getData == null) return;

        BGMsource.DOKill();
        BGMsource.Stop();
        soundData_BGM = getData;
        BGMsource.clip = soundData_BGM.clip;
        BGMsource.volume = 0f;
        BGMsource.Play();
        var targetVol = Mute_BGM ? 0f : soundData_BGM.Volume * (Volume_BGM / 100f);
        BGMsource.DOFade(targetVol, d).SetUpdate(true);
    }

    public void StopBGM()
    {
        BGMsource.DOKill();
        BGMsource.Stop();
        BGMsource.clip = null;
        soundData_BGM = null;
    }

    /// <summary>BGMをフェードアウトして停止</summary>
    public void StopBGMWithFade(float fadeDuration = -1f, Action onComplete = null)
    {
        var d = fadeDuration > 0 ? fadeDuration : BGM_FADE_DURATION;
        BGMsource.DOKill();
        BGMsource.DOFade(0f, d).SetUpdate(true).OnComplete(() =>
        {
            BGMsource.Stop();
            BGMsource.clip = null;
            soundData_BGM = null;
            onComplete?.Invoke();
        });
    }

    public void ChangeVolume_ForBGM()
    {
        if (BGMsource == null) return;
        BGMsource.mute = Mute_BGM || isPaused;
        float bgmBaseVolume = (soundData_BGM != null) ? soundData_BGM.Volume : 1f;
        BGMsource.volume = (Mute_BGM || isPaused) ? 0f : bgmBaseVolume * (Volume_BGM / 100f);
    }
    #endregion



    #region -- SE --
    /// <summary>インデックス指定でSE再生（汎用）</summary>
    public void PlaySE(int index)
    {
        var getData = SOLoader.SoundData.Get_SoundData_SE(index);
        if (getData == null) return;
        PlaySE(getData);
    }
    /// <summary>主にボタンクリックなどのUI再生用</summary>
    public void PlaySE_UI(int index)
    {
        var getData = SOLoader.SoundData.Get_SoundData_SE_UI(index);
        if (getData == null) return;
        PlaySE(getData);
    }

    public void PlaySE(SO_SoundElement getData)
    {
        if (getData == null || getData.clip == null) return;

        int count = 0;
        foreach (AudioSource source in SEsources)
        {
            if (count >= MAX_SIMULTANEOUS_SE) break;
            if (!source.isPlaying)
            {
                source.clip = getData.clip;
                source.volume = getData.Volume * Volume_Sound / 100f;
                source.pitch = UnityEngine.Random.Range(sePitchMin, sePitchMax);
                source.Play();
                return;
            }
            count++;
        }
    }

    public void StopSE()
    {
        foreach (AudioSource source in SEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }
    #endregion
}
