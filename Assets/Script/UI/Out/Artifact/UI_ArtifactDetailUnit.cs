using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ArtifactDetailUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_skillName;
    [SerializeField] TextMeshProUGUI tmp_description;
    [SerializeField] Image icon;
    [SerializeField] Color colorParamA = Color.green;
    [SerializeField] Color colorParamB = Color.red;
    [SerializeField] float hoverYOffset = 200f;
    [SerializeField] float targetPositionThresholdY = 300f;
    private ArtifactUnitData so;
    private RectTransform rectTr;
    private Canvas rootCanvas;
    private Vector3 anchorWorldPosition;

    public void SetData(ArtifactUnitData _so = null)
    {
        if (_so == null)
        {
            this.gameObject.SetActive(false);
            so = null;
            return;
        }
        so = _so;
        SetData_Base();
    }

    public void SetPositionWithAutoFlip(Vector3 _worldPosition)
    {
        EnsureCachedRefs();
        anchorWorldPosition = _worldPosition;
        var offsetY = GetVerticalOffsetForTargetPosition();
        ApplyVerticalOffset(offsetY);

        Canvas.ForceUpdateCanvases();
        if (IsAnyCornerOutOfScreen())
        {
            ApplyVerticalOffset(-offsetY);
        }
    }

    private void SetData_Base()
    {
        tmp_skillName.SetText(so.artifactName);
        icon.sprite = so.icon;

        var setText = so.description;
        var setParam = "";
        var setParam2 = "";

        string colorA = "";
        string colorB = "";

        switch (so.activeCheckTiming)
        {
            case ActiveCheckTiming.Passive:
            case ActiveCheckTiming.LastBooster:
                switch (so.unit)
                {
                    case "%":
                        setParam = $"+{(so.value * 100).ToString("F0")}%";
                        setParam2 = $"{(so.value_2 * 100).ToString("F0")}%";
                        break;
                    default:
                        setParam = $"+{so.value.ToString("F1")} {so.unit}";
                        setParam2 = $"{so.value_2.ToString("F1")} {so.unit}";
                        break;
                }
                colorA = ColorUtility.ToHtmlStringRGBA(colorParamA);
                colorB = ColorUtility.ToHtmlStringRGBA(colorParamB);

                break;
            case ActiveCheckTiming.Interval_5sec:
            case ActiveCheckTiming.Interval_attackPickaxe:
                setParam = $"{(so.activeCheckRate * 100).ToString("F0")}%";
                setParam2 = $"+{(so.value).ToString("F0")}sec";

                colorA = ColorUtility.ToHtmlStringRGBA(colorParamA);
                colorB = ColorUtility.ToHtmlStringRGBA(colorParamA);
                break;
        }

        setText = setText.Replace("[A]", $"<color=#{colorA}>{setParam}</color>");
        setText = setText.Replace("[B]", $"<color=#{colorB}>{setParam2}</color>");
        tmp_description.SetText(setText);

        this.gameObject.SetActive(true);
    }

    private void EnsureCachedRefs()
    {
        if (rectTr == null) rectTr = transform as RectTransform;
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
    }

    private void ApplyVerticalOffset(float _offsetY)
    {
        var targetScreenPos = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(), anchorWorldPosition);
        targetScreenPos.y += _offsetY;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTr.parent as RectTransform,
            targetScreenPos,
            GetCanvasCamera(),
            out var worldPos
        );
        rectTr.position = worldPos;
    }

    private Camera GetCanvasCamera()
    {
        if (rootCanvas == null) return null;
        return rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
    }

    private float GetVerticalOffsetForTargetPosition()
    {
        return IsTargetYAboveThreshold() ? -hoverYOffset : hoverYOffset;
    }

    private bool IsTargetYAboveThreshold()
    {
        EnsureCachedRefs();
        var screenPoint = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(), anchorWorldPosition);
        return screenPoint.y >= targetPositionThresholdY;
    }

    private bool IsAnyCornerOutOfScreen()
    {
        EnsureCachedRefs();
        var corners = new Vector3[4];
        rectTr.GetWorldCorners(corners);
        var cam = GetCanvasCamera();
        for (var i = 0; i < corners.Length; i++)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(cam, corners[i]);
            if (screenPoint.x < 0f || screenPoint.x > Screen.width || screenPoint.y < 0f || screenPoint.y > Screen.height)
            {
                return true;
            }
        }
        return false;
    }


}
