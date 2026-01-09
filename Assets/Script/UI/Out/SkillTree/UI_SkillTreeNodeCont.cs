using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTreeNodeCont : MonoBehaviour
{
    [SerializeField] private int baseSkillIndex;
    [SerializeField] private int targetSkillIndex;
    public int BaseSkillIndex => baseSkillIndex;
    public int TargetSkillIndex => targetSkillIndex;

    private UI_SkillTreeUnit baseUnit;
    private UI_SkillTreeUnit targetUnit;

    private float lineHeight = 4f;
    private RectTransform rectTransform;
    private Image lineImage;


#if UNITY_EDITOR
    public void SetNodeCont(UI_SkillTreeUnit _baseUnit, UI_SkillTreeUnit _targetUnit, float _lineHeight)
    {
        baseUnit = _baseUnit;
        targetUnit = _targetUnit;
        lineHeight = _lineHeight;
        baseSkillIndex = _baseUnit.skillIndex;
        targetSkillIndex = _targetUnit.skillIndex;

        if (baseUnit != null && targetUnit != null)
        {
            this.gameObject.SetActive(true);
            UpdateConnection();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 2つのUnit間の接続線を更新する
    /// </summary>
    public void UpdateConnection()
    {
        Debug.Log($"UpdateConnection: {baseUnit.name}, {targetUnit.name}");
        rectTransform = GetComponent<RectTransform>();
        lineImage = GetComponent<Image>();
        if (baseUnit == null || targetUnit == null || rectTransform == null) return;

        RectTransform baseRect = baseUnit.GetComponent<RectTransform>();
        RectTransform targetRect = targetUnit.GetComponent<RectTransform>();
        if (baseRect == null || targetRect == null) return;

        // 2つのUnitの中心位置を取得（ワールド座標）
        Vector3 baseWorldPos = baseRect.position;
        Vector3 targetWorldPos = targetRect.position;
        Debug.Log($"baseWorldPos: {baseWorldPos}, targetWorldPos: {targetWorldPos}");

        // 親のRectTransformを取得（通常はnodeRoot）
        RectTransform parentRect = rectTransform.parent as RectTransform;
        if (parentRect == null)
        {
            return;
        }

        // ワールド座標を親のローカル座標に変換
        Vector2 baseLocalPos;
        Vector2 targetLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(null, baseWorldPos),
            null,
            out baseLocalPos
        );
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(null, targetWorldPos),
            null,
            out targetLocalPos
        );

        // 2点間の距離と角度を計算
        Vector2 direction = targetLocalPos - baseLocalPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 線の位置を2点の中点に設定
        Vector2 centerPos = (baseLocalPos + targetLocalPos) * 0.5f;
        rectTransform.anchoredPosition = centerPos;

        // 線の回転を設定
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        // 線のサイズを設定（距離を幅、lineHeightを高さ）
        rectTransform.sizeDelta = new Vector2(distance, lineHeight);

        // ピボットを中心に設定（既に設定されているはずだが念のため）
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    }
    public void NotActivate()
    {
        this.gameObject.SetActive(false);
        baseUnit = null;
        targetUnit = null;
    }
#endif



    void Awake()
    {
        lineImage = GetComponent<Image>();
    }



    public void Set_LineState(SkillTreeUnlockState _unlockState, int _level)
    {
        lineImage.enabled = _unlockState != SkillTreeUnlockState.Hide;
        if (_unlockState == SkillTreeUnlockState.Hide) return;
        lineImage.color = _level == 0 ? Color.gray : Color.green;
    }



}
