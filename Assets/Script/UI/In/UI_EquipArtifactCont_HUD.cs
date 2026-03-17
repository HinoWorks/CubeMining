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

    [Header("効果表示用UI")]
    [SerializeField] GameObject obj_selectUnit;
    [SerializeField] private GameObject effectInfoRoot;
    [SerializeField] private TextMeshProUGUI effectDescriptionText;

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

        this.gameObject.SetActive(isActive);
        obj_selectUnit.SetActive(false);
        effectInfoRoot.SetActive(false);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"OnPointerEnter: {currentArtifactData.artifactName}");
        if (currentArtifactData == null) return;

        obj_selectUnit.SetActive(true);
        effectDescriptionText.text = currentArtifactData.description;
        effectInfoRoot.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        obj_selectUnit.SetActive(false);
        effectInfoRoot.SetActive(false);
    }
}
