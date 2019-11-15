using UnityEngine;
using Photon.Realtime;

public static class StaticObjects
{
    private static int masterPlayerNumber;
    public static int MasterPlayerNumber
    {
        get{ return masterPlayerNumber; }
        set{ masterPlayerNumber = value; }
    }
    private static bool isGameStart;
    public static bool IsGameStart
    {
        get { return isGameStart; }
        set { isGameStart = value; }
    }
}
