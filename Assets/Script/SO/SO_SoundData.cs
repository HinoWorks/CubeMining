using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[System.Serializable]
public class SO_SoundElement
{
    public int index;
    public string memo;
    public AudioClip clip;
    [SerializeField, Range(0, 1f)] public float Volume = 1f;
}


[CreateAssetMenu(menuName = "SO/SoundData")]
public class SO_SoundData : ScriptableObject
{
    public SO_SoundElement[] SoundData_BGM;
    public SO_SoundElement[] SoundData_SE;
    public SO_SoundElement[] SoundData_SE_UI;


    public SO_SoundElement Get_SoundData_BGM(int _index)
    {
        var data = Array.Find(SoundData_BGM, d => d.index == _index);
        return data;
    }



    public SO_SoundElement Get_SoundData_SE(int _index)
    {
        var data = Array.Find(SoundData_SE, d => d.index == _index);
        return data;
    }

    public SO_SoundElement Get_SoundData_SE_UI(int _index)
    {
        var data = Array.Find(SoundData_SE_UI, d => d.index == _index);
        return data;
    }

}
