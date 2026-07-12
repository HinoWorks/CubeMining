using UnityEngine;
using System;


[System.Serializable]
public class EnhanceCoinRateData
{
    public int generateLevel;
    public int coinTotalCount;
    public int createMax;
    public float baseRate;
    public float deltaRate;
}




[CreateAssetMenu(fileName = "SO_EnhanceCoinData", menuName = "SO/SO_EnhanceCoinData")]
public class SO_EnhanceCoinData : ScriptableObject
{
    public EnhanceCoinRateData[] enhanceCoinRates;


    public EnhanceCoinRateData Get_EnhanceCoinRateData(int _totalCoinCount)
    {
        var data = Array.Find(enhanceCoinRates, x => x.coinTotalCount <= _totalCoinCount);
        if (data == null)
        {
            Debug.LogError($"EnhanceCoinRateData not found: {_totalCoinCount}");
            return new EnhanceCoinRateData { generateLevel = 0, coinTotalCount = 0, baseRate = 0f, deltaRate = 0f };
        }
        return data;
    }
}
