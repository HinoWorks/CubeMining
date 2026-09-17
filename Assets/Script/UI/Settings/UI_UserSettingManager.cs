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

    public bool IsOpen => obj_main != null && obj_main.activeSelf;

    [SerializeField] GameObject obj_main;

    [Header("Sound")]
    [SerializeField] Slider slider_volumeMaster;
    [SerializeField] Slider slider_volumeBGM;
    [SerializeField] Slider slider_volumeSE;
    [SerializeField] TextMeshProUGUI tmp_volumeMaster;
    [SerializeField] TextMeshProUGUI tmp_volumeBGM;
    [SerializeField] TextMeshProUGUI tmp_volumeSE;

    [Header("BGM Type")]
    [SerializeField] Button btn_bgmPrev;
    [SerializeField] Button btn_bgmNext;
    [SerializeField] TextMeshProUGUI tmp_bgmType;

    [Header("Display")]
    [SerializeField] TMP_Dropdown dropdown_resolution;
    [SerializeField] Button btn_screenModePrev;
    [SerializeField] Button btn_screenModeNext;
    [SerializeField] TextMeshProUGUI tmp_screenMode;

    private static readonly FullScreenMode[] ScreenModeOptions =
    {
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed,
    };

    private readonly List<Vector2Int> resolutionOptions = new();
    private readonly List<int> bgmOptions = new();
    private UserSettingsData savedSnapshot;
    private UserSettingsData pendingDraft;
    private bool isSyncingUI;
    private bool screenModeEventsBound;

    void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(this);
    }

    void Start()
    {
        InitVolumeSliders();
        InitResolutionDropdown();
        InitBGMTypeSelector();
        InitScreenModeSelector();
        BindEvents();
        obj_main.SetActive(false);
    }

    public void Open()
    {
        EnsureResolutionDropdown();
        if (bgmOptions.Count == 0) InitBGMTypeSelector();

        var settings = UserSettingsManager.Inst;
        if (settings != null)
        {
            savedSnapshot = settings.GetSnapshot();
            pendingDraft = savedSnapshot.Copy();
        }

        obj_main.SetActive(true);
        InitScreenModeSelector();
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

    private void InitBGMTypeSelector()
    {
        bgmOptions.Clear();
        var options = UserSettingsManager.Inst != null
            ? UserSettingsManager.Inst.GetBGMOptions()
            : null;
        if (options == null) return;

        foreach (var option in options)
        {
            bgmOptions.Add(option.index);
        }
    }

    private void InitScreenModeSelector()
    {
        if (tmp_screenMode != null && btn_screenModePrev != null && btn_screenModeNext != null) return;
        if (btn_bgmPrev == null) return;

        var sourceRow = FindLayoutRow(btn_bgmPrev);
        if (sourceRow == null) return;

        var clone = Instantiate(sourceRow.gameObject, sourceRow.parent);
        clone.name = "pf_ui_setting(ScreenMode)";

        var resolutionRow = FindLayoutRow(dropdown_resolution);
        if (resolutionRow != null)
        {
            clone.transform.SetSiblingIndex(resolutionRow.GetSiblingIndex());
        }
        else
        {
            clone.transform.SetSiblingIndex(sourceRow.GetSiblingIndex() + 1);
        }

        var title = FindNamedText(clone.transform, "tmp_title");
        if (title != null) title.text = "Screen Mode";

        tmp_screenMode = FindNamedText(clone.transform, "tmp_value");

        var buttons = clone.GetComponentsInChildren<HButton>(true);
        System.Array.Sort(buttons, (a, b) => GetButtonSortX(a).CompareTo(GetButtonSortX(b)));
        if (buttons.Length >= 2)
        {
            btn_screenModePrev = buttons[0];
            btn_screenModeNext = buttons[1];
            btn_screenModePrev.onClick.RemoveAllListeners();
            btn_screenModeNext.onClick.RemoveAllListeners();
        }

        BindScreenModeEvents();
    }

    private static float GetButtonSortX(Component button)
    {
        var t = button.transform;
        while (t != null)
        {
            if (t is RectTransform rt && Mathf.Abs(rt.anchoredPosition.x) > 1f)
            {
                return rt.anchoredPosition.x;
            }
            t = t.parent;
        }
        return 0f;
    }

    private static Transform FindLayoutRow(Component component)
    {
        if (component == null) return null;
        var t = component.transform;
        while (t.parent != null && t.parent.GetComponent<VerticalLayoutGroup>() == null)
        {
            t = t.parent;
        }
        return t;
    }

    private static TextMeshProUGUI FindNamedText(Transform root, string objectName)
    {
        var texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts)
        {
            if (text.gameObject.name == objectName) return text;
        }
        return null;
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

        if (btn_bgmPrev != null)
            btn_bgmPrev.onClick.AddListener(OnClick_BGMPrev);
        if (btn_bgmNext != null)
            btn_bgmNext.onClick.AddListener(OnClick_BGMNext);

        BindScreenModeEvents();
    }

    private void BindScreenModeEvents()
    {
        if (screenModeEventsBound) return;
        if (btn_screenModePrev == null || btn_screenModeNext == null) return;

        btn_screenModePrev.onClick.AddListener(OnClick_ScreenModePrev);
        btn_screenModeNext.onClick.AddListener(OnClick_ScreenModeNext);
        screenModeEventsBound = true;
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
        UpdateBGMTypeLabel();
        UpdateScreenModeLabel();
        UpdateResolutionInteractable();

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

    private void UpdateBGMTypeLabel()
    {
        if (tmp_bgmType == null) return;
        int index = pendingDraft != null ? pendingDraft.bgmIndex : 0;
        var data = SOLoader.SoundData != null
            ? SOLoader.SoundData.Get_SoundData_BGM(index)
            : null;
        tmp_bgmType.text = data != null ? data.GetDisplayName() : "-";
    }

    private void UpdateScreenModeLabel()
    {
        if (tmp_screenMode == null) return;
        bool windowed = pendingDraft != null && UserSettingsManager.IsWindowed(pendingDraft.fullScreenMode);
        tmp_screenMode.text = windowed ? "Windowed" : "Fullscreen";
    }

    private void UpdateResolutionInteractable()
    {
        if (dropdown_resolution == null) return;
        bool windowed = pendingDraft == null || UserSettingsManager.IsWindowed(pendingDraft.fullScreenMode);
        dropdown_resolution.interactable = windowed;
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

    /// <summary>前のBGMタイプへ（インスペクタの Button.onClick からも可）</summary>
    public void OnClick_BGMPrev()
    {
        CycleBGMType(-1);
    }

    /// <summary>次のBGMタイプへ（インスペクタの Button.onClick からも可）</summary>
    public void OnClick_BGMNext()
    {
        CycleBGMType(1);
    }

    public void OnClick_ScreenModePrev()
    {
        CycleScreenMode(-1);
    }

    public void OnClick_ScreenModeNext()
    {
        CycleScreenMode(1);
    }

    private void CycleScreenMode(int direction)
    {
        if (isSyncingUI || pendingDraft == null) return;

        var current = UserSettingsManager.NormalizeScreenMode(pendingDraft.fullScreenMode);
        int index = System.Array.IndexOf(ScreenModeOptions, current);
        if (index < 0) index = 0;
        int next = (index + direction + ScreenModeOptions.Length) % ScreenModeOptions.Length;
        pendingDraft.fullScreenMode = (int)ScreenModeOptions[next];
        UpdateScreenModeLabel();
        UpdateResolutionInteractable();
        ApplyPreview();
    }

    private void CycleBGMType(int direction)
    {
        if (isSyncingUI || pendingDraft == null) return;
        if (bgmOptions.Count == 0) InitBGMTypeSelector();
        if (bgmOptions.Count == 0) return;

        int current = bgmOptions.IndexOf(pendingDraft.bgmIndex);
        if (current < 0) current = 0;
        int next = (current + direction + bgmOptions.Count) % bgmOptions.Count;
        pendingDraft.bgmIndex = bgmOptions[next];
        UpdateBGMTypeLabel();
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
