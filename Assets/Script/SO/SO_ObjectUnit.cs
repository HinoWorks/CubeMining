using UnityEngine;
using System;



[System.Serializable]
public class ObjectUnitData
{
    public int objectIndex;
    public string objectName;
    public Sprite icon;
    public GameObject pf;

    // -- param --
    public float hpRate;
    public int generateRate;
    public float valueRate;
}



[CreateAssetMenu(menuName = "SO/SO_ObjectUnit")]
public class SO_ObjectUnit : ScriptableObject
{
    public ObjectUnitData[] objectUnitDatas;


    public ObjectUnitData GetObjectUnitData(int _objectIndex)
    {
        var data = Array.Find(objectUnitDatas, data => data.objectIndex == _objectIndex);
        if (data == null)
        {
            Debug.LogError($"ObjectUnitData not found: {_objectIndex}");
            return null;
        }
        return data;
    }


}
