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
        bool[] isExist = new bool[PlayerNumbers];
        Debug.Log(PhotonNetwork.PlayerList.Length);
        if (PhotonNetwork.PlayerList.Length == 0)
        {
            PhotonNetwork.NickName = 0+"";
            PlayerNumber = 0;
        }
        else
        {
            
            foreach (Player player in PhotonNetwork.PlayerList)
                isExist[int.Parse(player.NickName)] = true;
            for(int i = 0; i < PlayerNumbers; i++)
            {
                if(isExist[i])
                {
                    PhotonNetwork.NickName = i + "";
                    PlayerNumber = i;
                    break;
                }
            }
        }
        
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
        Debug.Log("Joined Room");
        StartCoroutine(CreatePlayer());
    }

    // 룸 생성 함수
    private void CreateRoom()
    {
        PhotonNetwork.CreateRoom("room1", new RoomOptions { MaxPlayers = PlayerNumbers });
        Debug.Log("Room Name :NULL");
    }

    // 게임 네트워크 로비에 입장하지 못했을 경우 호출되는콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed join room : " + message);
        this.CreateRoom();
    }

    IEnumerator CreatePlayer()
    {
        Debug.Log(PlayerNumber);
        GameObject Player = PhotonNetwork.Instantiate(PlayerModel.name, Beacon[PlayerNumber].position, Quaternion.identity, 0);
        Player.name = PhotonNetwork.NickName;
        Player.GetComponent<PlayerController>().PlayerCamera = MainCamera;
        Player.GetComponent<PlayerAttribute>().setPlayerNumb(PlayerNumber);
        yield return null;
    }
}
