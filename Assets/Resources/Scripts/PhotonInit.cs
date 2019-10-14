using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    private string GameVersion;

    private PhotonView pv;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // SettingPlayerModelName 함수를 위한 PhotonView 컴포넌트
        pv = GetComponent<PhotonView>();
        OnLogin();
    }

    // Player가 처음 입장했을 때, 초기화
    public void OnLogin()
    {
        PhotonNetwork.NickName = "0";
        PhotonNetwork.GameVersion = this.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // 랜덤 룸과 연결하는 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!!");
        // 방이 존재할 경우 존재하는 방 중 하나를 들어감
        PhotonNetwork.JoinRandomRoom();
    }

    // 룸과 연결되었을 때 출력되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room, Generate Player Number");
        // 들어온 Player의 번호를 매겨주는 함수
        if (!PhotonNetwork.IsMasterClient)
        {
            bool[] isExist = new bool[PlayerNumbers];
            foreach(Player p in PhotonNetwork.PlayerList)
                isExist[int.Parse(p.NickName)] = true;
                
            for (int i = 0; i < PlayerNumbers; i++)
            {
                if (!isExist[i])
                {
                    PhotonNetwork.NickName = i + "";
                    break;
                }
            }
        }
        StartCoroutine(CreatePlayer());
    }

    // Player가 나갔을 때 알려주는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Left Player : " + otherPlayer.NickName);
    }

    // 게임 네트워크 로비에 입장하지 못했을 경우 호출되는콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 테스트 상황에서 가장 첫 번째 참가자
        Debug.Log("Failed join room : " + message);
        this.CreateRoom();
    }

    // 룸 생성 함수
    private void CreateRoom()
    {
        // "room1"이란 이름으로 방을 생성
        PhotonNetwork.CreateRoom("room1", new RoomOptions { MaxPlayers = PlayerNumbers });
    }

    IEnumerator CreatePlayer()
    {
        // 캐릭터 모델을 복사해 Player 캐릭터 생성
        GameObject _player = PhotonNetwork.Instantiate(PlayerModel.name, 
                                                        Beacon[int.Parse(PhotonNetwork.NickName)].position, 
                                                        Quaternion.identity, 0);
     // Player 캐릭터의 세부 사항 설정
        _player.name = PhotonNetwork.NickName;
        _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;
        _player.GetComponent<PlayerAttribute>().setAttackDamage(5);
        _player.GetComponent<PlayerAttribute>().setPlayerNumb(int.Parse(PhotonNetwork.NickName));
        yield return null;
    }
}
