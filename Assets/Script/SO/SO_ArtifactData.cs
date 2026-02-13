using UnityEngine;
using System;




[System.Serializable]
public class ArtifactUnitData
{
    public int artifactIndex;
    public string artifactName;
    public string artifactDescription;
    public Sprite icon;
    public GameObject pf;
    public ParamType paramType;
    public float value;
    //public AppearanceType appearanceType;
    //public float appearanceValue;
}


[System.Serializable]
public class ArtifactGenerateRateData
{
    public int generateLevel;
    public int artifactCount;
    public float baseRate;
    public float deltaInterval;
    public float deltaRate;
}




[CreateAssetMenu(menuName = "SO/SO_ArtifactData")]
public class SO_ArtifactData : ScriptableObject
{
    public ArtifactUnitData[] artifactDatas;
    public ArtifactGenerateRateData[] artifactGenerateRateDatas;


    public ArtifactUnitData Get_ArtifactData(int _artifactIndex)
    {

        var data = Array.Find(artifactDatas, x => x.artifactIndex == _artifactIndex);
        if (data == null)
        {
            Debug.LogError($"ArtifactUnitData is not found: {_artifactIndex}");
        }
        return data;
    }

    public ArtifactGenerateRateData Get_ArtifactGenerateRateData(int artifactCount)
    {
        var data = Array.Find(artifactGenerateRateDatas, x => x.artifactCount >= artifactCount);
        if (data == null)
        {
            Debug.LogError($"ArtifactGenerateRateData is not found: {artifactCount}");
        }
        return data;
    }
}
