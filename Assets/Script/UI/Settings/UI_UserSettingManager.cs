using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ユーザー設定画面の UI 制御
/// </summary>
public class UI_UserSettingManager : MonoBehaviour
{
    public static UI_UserSettingManager Inst;



    [SerializeField] GameObject obj_main;

    [Header("Sound")]
    [SerializeField] Slider slider_volumeMaster;
    [SerializeField] Slider slider_volumeBGM;
    [SerializeField] Slider slider_volumeSE;
    [SerializeField] TextMeshProUGUI tmp_volumeMaster;
    [SerializeField] TextMeshProUGUI tmp_volumeBGM;
    [SerializeField] TextMeshProUGUI tmp_volumeSE;

    [Header("Display")]
    [SerializeField] TMP_Dropdown dropdown_resolution;

    private readonly List<Vector2Int> resolutionOptions = new();
    private UserSettingsData savedSnapshot;
    private UserSettingsData pendingDraft;
    private bool isSyncingUI;

    void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(this);
    }

    void Start()
    {
        InitVolumeSliders();
        InitResolutionDropdown();
        BindEvents();
        obj_main.SetActive(false);
    }

    public void Open()
    {
        EnsureResolutionDropdown();

        var settings = UserSettingsManager.Inst;
        if (settings != null)
        {
            savedSnapshot = settings.GetSnapshot();
            pendingDraft = savedSnapshot.Copy();
        }

        obj_main.SetActive(true);
        SyncUIFromDraft();
    }

    public void Close()
    {
        obj_main.SetActive(false);
    }

    #region -- Init --
    private void InitVolumeSliders()
    {
        SetSliderRange(slider_volumeMaster);
        SetSliderRange(slider_volumeBGM);
        SetSliderRange(slider_volumeSE);
    }

    private static void SetSliderRange(Slider slider)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.wholeNumbers = true;
    }

    private void InitResolutionDropdown()
    {
        if (dropdown_resolution == null) return;
        if (UserSettingsManager.Inst == null) return;

        resolutionOptions.Clear();
        resolutionOptions.AddRange(UserSettingsManager.Inst.GetAvailableResolutions());
        if (resolutionOptions.Count == 0) return;

        dropdown_resolution.ClearOptions();
        var options = new List<string>(resolutionOptions.Count);
        foreach (var resolution in resolutionOptions)
        {
            options.Add($"{resolution.x} x {resolution.y}");
        }
        dropdown_resolution.AddOptions(options);
    }

    private void EnsureResolutionDropdown()
    {
        if (resolutionOptions.Count == 0)
        {
            InitResolutionDropdown();
        }
    }

    private void BindEvents()
    {
        if (slider_volumeMaster != null)
            slider_volumeMaster.onValueChanged.AddListener(OnVolumeMasterChanged);
        if (slider_volumeBGM != null)
            slider_volumeBGM.onValueChanged.AddListener(OnVolumeBGMChanged);
        if (slider_volumeSE != null)
            slider_volumeSE.onValueChanged.AddListener(OnVolumeSEChanged);

        if (dropdown_resolution != null)
            dropdown_resolution.onValueChanged.AddListener(OnResolutionChanged);
    }
    #endregion

    #region -- Sync --
    private void SyncUIFromDraft()
    {
        if (pendingDraft == null) return;

        isSyncingUI = true;

        slider_volumeMaster?.SetValueWithoutNotify(pendingDraft.volumeMaster);
        slider_volumeBGM?.SetValueWithoutNotify(pendingDraft.volumeBGM);
        slider_volumeSE?.SetValueWithoutNotify(pendingDraft.volumeSE);

        SyncResolutionDropdown(pendingDraft.resolutionWidth, pendingDraft.resolutionHeight);
        UpdateVolumeLabels();

        isSyncingUI = false;
    }

    private void SyncResolutionDropdown(int width, int height)
    {
        if (dropdown_resolution == null || resolutionOptions.Count == 0) return;

        int index = resolutionOptions.FindIndex(r => r.x == width && r.y == height);
        if (index < 0) index = 0;
        dropdown_resolution.SetValueWithoutNotify(index);
    }

    private void UpdateVolumeLabels()
    {
        SetVolumeLabel(tmp_volumeMaster, slider_volumeMaster);
        SetVolumeLabel(tmp_volumeBGM, slider_volumeBGM);
        SetVolumeLabel(tmp_volumeSE, slider_volumeSE);
    }

    private static void SetVolumeLabel(TextMeshProUGUI label, Slider slider)
    {
        if (label == null || slider == null) return;
        label.text = $"{Mathf.RoundToInt(slider.value)}%";
    }

    private void ApplyPreview()
    {
        if (pendingDraft == null) return;
        UserSettingsManager.Inst?.ApplyPreview(pendingDraft);
    }
    #endregion

    #region -- UI Events --
    private void OnVolumeMasterChanged(float value)
    {
        if (isSyncingUI || pendingDraft == null) return;
        pendingDraft.volumeMaster = value;
        SetVolumeLabel(tmp_volumeMaster, slider_volumeMaster);
        ApplyPreview();
    }

    private void OnVolumeBGMChanged(float value)
    {
        if (isSyncingUI || pendingDraft == null) return;
        pendingDraft.volumeBGM = value;
        SetVolumeLabel(tmp_volumeBGM, slider_volumeBGM);
        ApplyPreview();
    }

    private void OnVolumeSEChanged(float value)
    {
        if (isSyncingUI || pendingDraft == null) return;
        pendingDraft.volumeSE = value;
        SetVolumeLabel(tmp_volumeSE, slider_volumeSE);
        ApplyPreview();
    }

    private void OnResolutionChanged(int index)
    {
        if (isSyncingUI || pendingDraft == null) return;
        if (index < 0 || index >= resolutionOptions.Count) return;

        var resolution = resolutionOptions[index];
        pendingDraft.resolutionWidth = resolution.x;
        pendingDraft.resolutionHeight = resolution.y;
        ApplyPreview();
    }
    #endregion

    #region -- on Click --
    /// <summary>戻る：変更を破棄して閉じる</summary>
    public void OnClick_Back()
    {
        if (savedSnapshot != null)
        {
            UserSettingsManager.Inst?.RestoreSnapshot(savedSnapshot);
        }
        Close();
    }

    /// <summary>適用：変更を保存して閉じる</summary>
    public void OnClick_Apply()
    {
        if (pendingDraft != null)
        {
            UserSettingsManager.Inst?.CommitSettings(pendingDraft);
            savedSnapshot = pendingDraft.Copy();
        }
        Close();
    }

    public void OnClick_Close()
    {
        OnClick_Back();
    }
    #endregion
}
