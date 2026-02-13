using UnityEngine;
using System;

[System.Serializable]
public class TextColor
{
    public ResourceType resourceType;
    public Color color;
}

[CreateAssetMenu(menuName = "SO/SO_UISetting")]
public class SO_UISetting : ScriptableObject
{
    public TextColor[] textColors;



    public Color GetTextColor(ResourceType _resourceType)
    {
        var data = Array.Find(textColors, data => data.resourceType == _resourceType);
        if (data == null)
        {
            Debug.LogError($"TextColor not found: {_resourceType}");
            return Color.white;
        }
        return data.color;
    }





}
