using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ArtifactDetailUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_skillName;
    [SerializeField] TextMeshProUGUI tmp_description;
    //[SerializeField] TextMeshProUGUI tmp_paramNow;
    //[SerializeField] TextMeshProUGUI tmp_paramNext;
    [SerializeField] Color colorParamA = Color.green;
    [SerializeField] Color colorParamB = Color.red;
    [SerializeField] float hoverYOffset = 200f;
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
        ApplyVerticalOffset(hoverYOffset);

        Canvas.ForceUpdateCanvases();
        if (IsAnyCornerOutOfScreen())
        {
            ApplyVerticalOffset(-hoverYOffset);
        }
    }

    private void SetData_Base()
    {
        tmp_skillName.SetText(so.artifactName);

        var setText = so.description;
        var setParam = "";
        var setParam2 = "";
        switch (so.unit)
        {
            case "%":
                setParam = $"+{(so.value * 100).ToString("F0")}%";
                setParam2 = $"-{(so.value_2 * 100).ToString("F0")}%";
                break;
            default:
                setParam = $"+{so.value.ToString("F0")} {so.unit}";
                setParam2 = $"-{so.value_2.ToString("F0")} {so.unit}";
                break;
        }
        var colorA = ColorUtility.ToHtmlStringRGBA(colorParamA);
        var colorB = ColorUtility.ToHtmlStringRGBA(colorParamB);
        setText = setText.Replace("[A]", $"<color=#{colorA}>{setParam}</color>");
        setText = setText.Replace("[B]", $"<color=#{colorB}>{setParam2}</color>");
        tmp_description.SetText(setText);

        // var paramNow = so.value;
        //tmp_paramNow.SetText(paramNow.ToString("F2"));
        //tmp_paramNext.SetText("xxxx");

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
