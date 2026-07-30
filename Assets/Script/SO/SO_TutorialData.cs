using UnityEngine;
using System;



public enum TutorialType
{
    None,
    Welcome = 1,
    CraftPickaxe = 2,
    SpecialSkill = 3,
    Artifact = 4,
}



[System.Serializable]
public class TutorialUnitData
{
    public int tutorialIndex;
    public string title;
    [TextArea(2, 6)] public string description;
    public Sprite icon;
}


[CreateAssetMenu(menuName = "SO/SO_TutorialData")]
public class SO_TutorialData : ScriptableObject
{
    public TutorialUnitData[] tutorialUnitDatas;

    public TutorialUnitData Get_TutorialUnitData(int _tutorialIndex)
    {
        var data = Array.Find(tutorialUnitDatas, x => x.tutorialIndex == _tutorialIndex);
        if (data == null)
        {
            Debug.LogError($"TutorialUnitData is not found: {_tutorialIndex}");
        }
        return data;
    }

    public int Get_TutorialIndex(TutorialType _tutorialType)
    {
        var index = (int)_tutorialType;
        return index;
    }

}
