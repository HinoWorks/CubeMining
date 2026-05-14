using System.Numerics;
using UnityEngine;
using System;


[System.Serializable]
public class PlayerLevel
{
    public int level;
    public int exp;
}


[CreateAssetMenu(menuName = "SO/SO_PlayerLevelData", fileName = "SO_PlayerLevelData")]
public class SO_PlayerLevelData : ScriptableObject
{
    public PlayerLevel[] playerLevels;
    public int maxLevel => playerLevels.Length;

    public PlayerLevel GetPlayerLevel(int level)
    {
        if (level < 1) level = 1;
        if (level > maxLevel) level = maxLevel;
        return Array.Find(playerLevels, x => x.level == level);
    }
}
