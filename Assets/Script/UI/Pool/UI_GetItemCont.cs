using UnityEngine;

public class UI_GetItemCont : UI_GetIconBase
{

    private const int ENHANCE_COIN_INDEX = 100;

    public void SetInit_EnhanceCoin(Vector3 _basePosition)
    {
        var itemData = SOLoader.ItemData.GetItemUnitData(ENHANCE_COIN_INDEX);
        var currentCount = InGameManager.Inst.Get_EnhanceCoinCount();
        var targetPosition = UIManager_InGame.Inst.Get_EnhanceCoinTargetPosition(currentCount).position;

        base.Init(itemData.icon, _basePosition, targetPosition);
    }

}
