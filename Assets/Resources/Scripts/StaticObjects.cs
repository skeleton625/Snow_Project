using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticObjects
{
    private static bool[] playerExist = new bool[4];

    public static void SetPlayerExist(int _num)
    {
        playerExist[_num] = true;
    }

    public static bool GetPlayerExist(int _num)
    {
        return playerExist[_num];
    }

    public static void InitPlayerExist()
    {
        for(int i = 0; i < 4; i++)
        {
            playerExist[i] = false;
        }
    }
}
