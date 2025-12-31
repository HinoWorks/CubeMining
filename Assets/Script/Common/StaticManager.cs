using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;


public class StaticManager : MonoBehaviour
{
    public static string KEY_CREATE_INITIAL_DATA = "key_createInitialData";


    private static BigInteger Get_Num(int _count)
    {
        BigInteger num = 1;
        for (int i = 0; i < _count; i++)
        {
            num *= 10;
        }
        return num;
    }


    public static string Get_BigintegerToString(BigInteger _coin)
    {
        if (_coin / Get_Num(3) < 1)
        {
            return (_coin.ToString());
        }
        else if (_coin / Get_Num(6) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(2)) / 10f;
            return (st_coin.ToString("f1") + "K");
        }
        else if (_coin / Get_Num(9) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(5)) / 10f;
            return (st_coin.ToString("f1") + "M");
        }
        else if (_coin / Get_Num(12) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(8)) / 10f;
            return (st_coin.ToString("f1") + "B");
        }
        else if (_coin / Get_Num(15) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(11)) / 10f;
            return (st_coin.ToString("f1") + "T");
        }
        else if (_coin / Get_Num(18) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(14)) / 10f;
            return (st_coin.ToString("f1") + "Q");
        }
        else
        {
            float st_coin = (float)(_coin / Get_Num(17)) / 10f;
            return (st_coin.ToString("f1") + "Qa");
        }
    }

    public static (float num, string unit, int unitInt) Get_BigintegerToUnit(BigInteger _coin)
    {
        if (_coin / Get_Num(3) < 1)
        {
            return (float.Parse(_coin.ToString("f1")), "", 0);
        }
        else if (_coin / Get_Num(6) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(2)) / 10f;
            return (float.Parse(st_coin.ToString("f1")), "K", 3);
        }
        else if (_coin / Get_Num(9) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(5)) / 10f;
            return (float.Parse(st_coin.ToString("f1")), "M", 6);
        }
        else if (_coin / Get_Num(12) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(8)) / 10f;
            return (float.Parse(st_coin.ToString("f1")), "B", 9);
        }
        else if (_coin / Get_Num(15) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(11)) / 10f;
            return (float.Parse(st_coin.ToString("f1")), "T", 12);
        }
        else if (_coin / Get_Num(18) < 1)
        {
            float st_coin = (float)(_coin / Get_Num(14)) / 10f;
            return (float.Parse(st_coin.ToString("f1")), "q", 15);
        }
        else
        {
            float st_coin = (float)(_coin / Get_Num(17)) / 10f;
            return (float.Parse(st_coin.ToString("f1")), "Qa", 18);
        }

    }


    #region -- Time --
    public static string Get_StringFromDateTime(DateTime _date)
    {
        return _date.ToBinary().ToString();
    }
    public static DateTime Get_DateTimeFromString(string _date)
    {
        return System.DateTime.FromBinary(System.Convert.ToInt64(_date));
    }
    #endregion




    #region 
    public static bool CoinCheck(BigInteger _cost)
    {
        return _cost <= SaveLoader.Inst.Coin;
    }
    #endregion

}
