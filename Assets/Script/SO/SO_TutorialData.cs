using UnityEngine;
using System;



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
}
