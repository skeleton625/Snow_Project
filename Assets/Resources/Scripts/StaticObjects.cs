using UnityEngine;
using Photon.Realtime;

public static class StaticObjects
{
    private static string masterPlayerName;
    public static string MasterPlayerName
    {
        get { return masterPlayerName; }
        set { masterPlayerName = value; }
    }

    private static int masterPlayerNum;
    public static int MasterPlayerNum
    {
        get{ return masterPlayerNum; }
        set{ masterPlayerNum = value; }
    }
    private static int masterPlayerModelNum;
    public static int MasterPlayerModelNum
    {
        get { return masterPlayerModelNum; }
        set { masterPlayerModelNum = value; }
    }
    private static bool isGameStart;
    public static bool IsGameStart
    {
        get { return isGameStart; }
        set { isGameStart = value; }
    }
    private static int gamePlayTime = 60;
    public static int GamePlayTime
    {
        get { return gamePlayTime; }
        set { gamePlayTime = value; }
    }
}
