using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 指定した装備スロットにおいて、現在装備中のアーティファクトを表示するHUD用クラス
/// </summary>
public class UI_EquipArtifactCont_HUD : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int slotIndex = 1;

    [Header("表示用UI")]
    [SerializeField] private Image icon;
    [SerializeField] GameObject obj_activeEffect_always;
    [SerializeField] ParticleSystem eff_activeEffect_oneShot;


    [Header("効果表示用UI")]
    [SerializeField] GameObject obj_selectUnit;
    [SerializeField] private GameObject effectInfoRoot;
    [SerializeField] private TextMeshProUGUI effectDescriptionText;
    [SerializeField] private Color colorParamA;
    [SerializeField] private Color colorParamB;

    private ArtifactUnitData currentArtifactData;


    /// <summary>
    /// スロットの状態と装備中アーティファクトを読み込み、HUDに反映
    /// </summary>
    public async void Init_ArtifactData()
    {
        // セーブデータから対象スロット情報を取得
        var slotData = await SaveLoader.Inst.Get_ArtifactSlotData(slotIndex);

        // 解放済みスロットの場合、装備中アーティファクトを表示
        currentArtifactData = null;
        var isActive = slotData != null && slotData.equipedArtifactIndex != -1;
        if (isActive)
        {
            var artifactData = SOLoader.ArtifactData.Get_ArtifactData(slotData.equipedArtifactIndex);
            if (artifactData != null)
            {
                currentArtifactData = artifactData;
                icon.sprite = artifactData.icon;
            }
        }
        obj_activeEffect_always.SetActive(false);
        eff_activeEffect_oneShot.Stop();

        this.gameObject.SetActive(isActive);
        obj_selectUnit.SetActive(false);
        effectInfoRoot.SetActive(false);
    }


    public void Set_ActiveEffect(int _artifactIndex)
    {
        if (currentArtifactData == null) return;
        if (currentArtifactData.artifactIndex != _artifactIndex) return;

        switch (currentArtifactData.activeCheckTiming)
        {
            case ActiveCheckTiming.StartIngame:
            case ActiveCheckTiming.Interval_breakBlock_25:
            case ActiveCheckTiming.Interval_attackPickaxe:
                eff_activeEffect_oneShot.Play();
                break;
            case ActiveCheckTiming.Passive:
                obj_activeEffect_always.SetActive(true);
                break;
            case ActiveCheckTiming.LastBooster:
                obj_activeEffect_always.SetActive(true);
                eff_activeEffect_oneShot.Play();
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentArtifactData == null) return;

        obj_selectUnit.SetActive(true);
        SetText();
        effectInfoRoot.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        obj_selectUnit.SetActive(false);
        effectInfoRoot.SetActive(false);
    }

    private void SetText()
    {
        var setText = currentArtifactData.description;
        var setParam = "";
        var setParam2 = "";
        switch (currentArtifactData.unit)
        {
            case "%":
                setParam = $"+{(currentArtifactData.value * 100).ToString("F0")}%";
                setParam2 = $"-{(currentArtifactData.value_2 * 100).ToString("F0")}%";
                break;
            default:
                setParam = $"+{currentArtifactData.value.ToString("F0")} {currentArtifactData.unit}";
                setParam2 = $"-{currentArtifactData.value_2.ToString("F0")} {currentArtifactData.unit}";
                break;
        }
        var colorA = ColorUtility.ToHtmlStringRGBA(colorParamA);
        var colorB = ColorUtility.ToHtmlStringRGBA(colorParamB);
        setText = setText.Replace("[A]", $"<color=#{colorA}>{setParam}</color>");
        setText = setText.Replace("[B]", $"<color=#{colorB}>{setParam2}</color>");
        effectDescriptionText.SetText(setText);

        this.gameObject.SetActive(true);
    }



}
