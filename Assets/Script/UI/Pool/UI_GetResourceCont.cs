using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_GetResourceCont : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] UI_GetResourceMove moveCont;
    private Vector3 targetPosition;

    public void Set_ResourceType(ResourceType _resourceType)
    {
        icon.sprite = SOLoader.ItemData.GetItemUnitData((int)_resourceType).icon;
        var target = UIManager_InGame.Inst.Get_ResourceCounterTargetPosition(_resourceType);
        if (target == null) return;
        targetPosition = target.position;
    }
    public void SetInit(Vector3 _position)
    {
        var setPosition = Camera.main.WorldToScreenPoint(_position);
        moveCont.UnitActivate_SetPosi(targetPosition, setPosition);
    }
}
