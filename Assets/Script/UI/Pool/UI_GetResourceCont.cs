using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_GetResourceCont : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] UI_GetResourceMove moveCont;
    [SerializeField] TextMeshProUGUI tmp_resourceCount;
    private Vector3 targetPosition;


    private float size_max2 = 1.75f;
    private float size_max = 1.5f;
    private float size_mid = 1.25f;
    private float size_min = 1f;



    public void Set_ResourceType(ResourceType _resourceType, int _setCount = 1, UI_ResourceUnitSize _unitSize = UI_ResourceUnitSize.Min)
    {
        icon.sprite = SOLoader.ItemData.GetItemUnitData((int)_resourceType).icon;
        tmp_resourceCount.text = _setCount == 1 ? "" : _setCount.ToString();
        tmp_resourceCount.color = SOLoader.UISetting.GetTextColor(_resourceType);
        SetSize(_unitSize);
        var target = UIManager_InGame.Inst.Get_ResourceCounterTargetPosition(_resourceType);
        if (target == null) return;
        targetPosition = target.position;
    }
    private void SetSize(UI_ResourceUnitSize _unitSize)
    {
        switch (_unitSize)
        {
            case UI_ResourceUnitSize.Max2:
                this.transform.localScale = size_max2 * Vector3.one;
                transform.SetAsLastSibling();
                break;
            case UI_ResourceUnitSize.Max:
                this.transform.localScale = size_max * Vector3.one;
                transform.SetAsLastSibling();
                break;
            case UI_ResourceUnitSize.Mid:
                this.transform.localScale = size_mid * Vector3.one;
                transform.SetAsLastSibling();
                break;
            case UI_ResourceUnitSize.Min:
                this.transform.localScale = size_min * Vector3.one;
                break;
        }
    }
    public void SetInit(Vector3 _position)
    {
        var setPosition = Camera.main.WorldToScreenPoint(_position);
        moveCont.UnitActivate_SetPosi(targetPosition, setPosition);
    }
}
