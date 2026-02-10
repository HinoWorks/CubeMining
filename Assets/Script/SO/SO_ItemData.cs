using UnityEngine;
using System;

[System.Serializable]
public class ItemUnitData
{
    public int itemIndex;
    public string itemName;
    public string itemDescription;
    public Sprite icon;
}


[CreateAssetMenu(menuName = "SO/SO_ItemData")]
public class SO_ItemData : ScriptableObject
{
    public ItemUnitData[] itemUnitDatas;

    public ItemUnitData GetItemUnitData(int _itemIndex)
    {
        var data = Array.Find(itemUnitDatas, data => data.itemIndex == _itemIndex);
        if (data == null)
        {
            Debug.LogError($"ItemUnitData not found: {_itemIndex}");
        }
        return data;
    }
}
