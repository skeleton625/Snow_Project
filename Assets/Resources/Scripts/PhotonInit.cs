using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject PlayerModel;
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private byte PlayerNumbers;
    [SerializeField]
    private Transform[] Beacon;

    private string gameVersion = "1.0";
    private int PlayerNumber;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        OnLogin();
    }

    public void OnLogin()
    {
        PhotonNetwork.NickName = "0";
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // 랜덤 룸과 연결하는 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!!");
        PhotonNetwork.JoinRandomRoom();
    }

    // 룸과 연결되었을 때 출력되는 콜백 함수
    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.NickName != "1")
        {
            bool[] isExist = new bool[PlayerNumbers + 1];
            for (int i = 1; i <= PhotonNetwork.PlayerList.Length-1; i++)
            {
                isExist[int.Parse(PhotonNetwork.PlayerList[i].NickName)] = true;
            }
                
            for (int i = 1; i <= PlayerNumbers; i++)
            {
                if (!isExist[i])
                {
                    PhotonNetwork.NickName = i + "";
                    PlayerNumber = i;
                    break;
                }
            }
        }
        Debug.Log("Joined Room");
        StartCoroutine(CreatePlayer());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("new Player : " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
            Debug.Log(p.NickName);
        Debug.Log("Left Player : " + otherPlayer.NickName);
    }

    // 게임 네트워크 로비에 입장하지 못했을 경우 호출되는콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed join room : " + message);
        PhotonNetwork.NickName = "1";
        this.CreateRoom();
    }

    // 룸 생성 함수
    private void CreateRoom()
    {
        PhotonNetwork.CreateRoom("room1", new RoomOptions { MaxPlayers = PlayerNumbers });
        Debug.Log("Room Name :room1");
    }

    IEnumerator CreatePlayer()
    {
        GameObject Player = PhotonNetwork.Instantiate(PlayerModel.name, Beacon[PlayerNumber].position, Quaternion.identity, 0);
        Player.name = PhotonNetwork.NickName;
        Player.GetComponent<PlayerController>().PlayerCamera = MainCamera;
        Player.GetComponent<PlayerAttribute>().setPlayerNumb(PlayerNumber);
        yield return null;
    }
}
