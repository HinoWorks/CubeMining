using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class UI_SkillTreeMaanger : MonoBehaviour
{
    private UI_SkillTreeUnit[] skillTreeUnits;
    [SerializeField] UI_SkillTreeDetail ui_skillTreeDetail;


    [Space(10)]
    [Header("Scroll settings")]
    [SerializeField] RectTransform scrollContent;
    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 1.0f;

    private Vector2 lastMousePos;
    private float duration_zoom = 0.05f;

    void Awake()
    {
        // -unit set-
        skillTreeUnits = transform.GetComponentsInChildren<UI_SkillTreeUnit>();
        foreach (var skillTreeUnit in skillTreeUnits)
        {
            skillTreeUnit.AwakeCall(OnMouseOver);
        }
        ui_skillTreeDetail.gameObject.SetActive(false);
    }


    public void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == OutGame_MenuType.SkillTree;
        this.gameObject.SetActive(isActive);
        ui_skillTreeDetail.gameObject.SetActive(false);
        if (!isActive) return;

        // == TODO ==
        // content Init
    }



    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    // ---------- Zoom ----------
    void HandleZoom()
    {
        float wheel = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(wheel) < 0.01f) return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            scrollContent,
            Mouse.current.position.ReadValue(),
            null,
            out localMousePos
        );

        float oldScale = scrollContent.localScale.x;
        float newScale = Mathf.Clamp(oldScale + wheel * zoomSpeed, minScale, maxScale);
        if (Mathf.Approximately(oldScale, newScale)) return;

        float scaleRatio = newScale / oldScale;
        // scrollContent.localScale = Vector3.one * newScale;
        // マウス位置基準ズーム
        //scrollContent.anchoredPosition -= localMousePos * (scaleRatio - 1f);
        //DOTween.To(() => scrollContent.localScale, x => scrollContent.localScale = x, Vector3.one * newScale, duration_zoom);
        //DOTween.To(() => scrollContent.anchoredPosition, x => scrollContent.anchoredPosition = x, scrollContent.anchoredPosition - localMousePos * (scaleRatio - 1f), duration_zoom);
    }

    // ---------- Pan ----------
    void HandlePan()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            lastMousePos = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 current = Mouse.current.position.ReadValue();
            Vector2 delta = current - lastMousePos;
            //scrollContent.anchoredPosition += delta * panSpeed;
            DOTween.To(() => scrollContent.anchoredPosition, x => scrollContent.anchoredPosition = x, scrollContent.anchoredPosition + delta * panSpeed, duration_zoom);
            lastMousePos = current;
        }
    }


    private void OnMouseOver(bool _isEnter, UI_SkillTreeUnit _skillTreeUnit)
    {
        if (_isEnter)
        {
            ui_skillTreeDetail.transform.position = _skillTreeUnit.transform.position;
            ui_skillTreeDetail.gameObject.SetActive(true);
        }
        else
        {
            ui_skillTreeDetail.gameObject.SetActive(false);
        }
    }
}
