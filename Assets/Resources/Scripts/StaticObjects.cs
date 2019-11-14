using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StaticObjects : MonoBehaviour
{
    private static int ballCount;
    public static int BallCount
    {
        get { return ballCount; }
        set { ballCount += value; }
    }
    private static int masterPlayerNumber;
    public static int MasterPlayerNumber
    {
        get{ return masterPlayerNumber; }
        set{ masterPlayerNumber = value; }
    }
    private static Player[] GamePlayerList;
    private static bool[] playerExist = new bool[4];
    [SerializeField]
    private Transform[] Beacons;
    [SerializeField]
    private GameObject[] Players;
    [SerializeField]
    private GameObject[] PlayerDeads;

    public Transform GetBeacons(int _num)
    {
        return Beacons[_num];
    }

    public GameObject GetPlayerModels(int _num)
    {
        return Players[_num];
    }

    public GameObject GetPlayerDeads(int _num)
    {
        return PlayerDeads[_num];
    }

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

    public static void SetPlayerList(Player[] _playerlist)
    {
        GamePlayerList = _playerlist;
    }

    public static Player GetPlayer(int _num)
    {
        return GamePlayerList[_num];
    }
}
