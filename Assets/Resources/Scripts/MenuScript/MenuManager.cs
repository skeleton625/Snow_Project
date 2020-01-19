using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private PhotonView PV;
    [SerializeField]
    private PhotonNet MenuNetwork;

    private bool[] AllPlayerReady;
    private bool MasterPlayerReady;
    private int PreMenuNum;
    private MenuUIManager UManager;
    private InMenuObjectManager OManager;

    private void Start()
    {
        // 싱글톤 변수 할당
        UManager = MenuUIManager.instance;
        OManager = InMenuObjectManager.instance;

        // 현재 플레이어 변수 초기화
        MasterPlayerReady = false;
        AllPlayerReady = new bool[4];
        if (MenuNetwork.LeaveRoom())
            SetMenuActive(1, null);
    }

    private void Update()
    {
        if (MenuNetwork.IsLobbyUpdate)
            DisplayRoomButtons();
        if (MenuNetwork.IsRoomUpdate)
            SettingRoomPlayers();
    }

    public void SetPlayerName()
    {
        // 플레이어가 입력한 이름으로 네트워크에 로그인 진행
        MenuNetwork.OnLogin(UManager.GetInputPlayerName());
    }

    public void SetPlayerModel(bool _isLeft)
    {
        OManager.SetActivateModel(_isLeft);
    }

    public void SetMenuActive(int _num, string option)
    {
        // 이전 Ui 화면 비활성화
        UManager.SetUIActive(PreMenuNum, false);
        // 이전 Ui 화면을 현재로 재설정
        PreMenuNum = _num;

        switch (_num)
        {
            // 1, 3번 Ui일 경우, Room에 있지 않거나 Lobby에 있지 않을 경우, 대기 상태
            case 1: case 3:
                if (!MenuNetwork.IsRoom() || !MenuNetwork.IsLobby())
                    StartCoroutine(WaitingCoroutine(option));
                break;
            // 1, 3 번 Ui가 아닐 경우, 활성화
            default:
                UManager.SetUIActive(_num, true);
                break;
        }
    }

    public void CreateRoomByMaster()
    {
        // 방 생성 시, 입력한 방 이름을 반환 받음
        string _roomName = UManager.GetInputRoomName();

        // 방을 PhotonNetwork에서 생성, 생성되지 않을 시 오류 팝업창을 띄움
        if (!MenuNetwork.CreateRoom(_roomName))
        {
            UManager.VerifyPopup(1, "This Room name is already Exist");
            SetMenuActive(1, _roomName);
            return;
        }

        // 방 생성 시, Ui 3번으로 이동
        SetMenuActive(3, _roomName);
    }

    public void PlayerJoinRoom(string _roomName)
    {
        // PhotonNetwork에서 플레이어가 원하는 방에 입장
        MenuNetwork.JoinRoom(_roomName);
        // 방 입장 시, Ui 3번으로 이동
        SetMenuActive(3, _roomName);
    }

    public void PlayerLeaveRoom()
    {
        // 방 퇴장 시에 플레이어가 방장일 경우, 모든 플레이어 퇴장
        if (MenuNetwork.IsRoomMaster())
            PV.RPC("AllPlayerLeaveRoom", RpcTarget.All);
        // 그렇지 않을 경우, 플레이어 방 정보 초기화
        else
        {
            StaticObjects.MasterPlayerNum = 0;
            MasterPlayerReady = false;

            for (int i = 0; i < AllPlayerReady.Length; i++)
                AllPlayerReady[i] = false;
            MenuNetwork.LeaveRoom();
        }
    }

    public void MasterStartGame()
    {
        // 플레이어가 방장일 경우, 모든 플레이어가 시작 버튼을 눌렀는지 확인
        if (StaticObjects.MasterPlayerNum == 0)
        {
            // 누르지 않았을 경우, 오류 팝업창을 띄움
            if (!VerifyPressStartButton())
                UManager.VerifyPopup(3, "Someone don't press Ready Button");
            // 모든 플레이어가 눌렀을 경우, GameScene으로 이동
            else
            {
                PV.RPC("PlayerStartGame", RpcTarget.All, StaticObjects.GamePlayTime);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        // 플레이어가 방장이 아니면 플레이어의 상태를 "준비됨"으로 변경
        else
        {
            MasterPlayerReady = !MasterPlayerReady;
            PV.RPC("PressReady", RpcTarget.All, StaticObjects.MasterPlayerNum, MasterPlayerReady);
        }
    }

    private bool VerifyPressStartButton()
    {
        // 현재 모든 플레이어가 준비 되어있는지를 확인
        for (int i = 1; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!AllPlayerReady[i])
                return false;
        }
        return true;
    }

    private void DisplayRoomButtons()
    {
        // 방 생성 시, 표시되는 버튼 개수 초기화
        UManager.DisplayPreRoomButtons(MenuNetwork.GetRoomNames());
        // 방 생성이 완료되었음을 정의
        MenuNetwork.IsLobbyUpdate = false;
    }

    private void SettingRoomPlayers()
    {
        // 현재 방의 플레이어 정보를 가져옴
        Player[] _playerList = MenuNetwork.GetPlayerList();
        // 현재 방 정보를 초기화함
        UManager.SetPreRoomName(MenuNetwork.GetCurrentRoomName(), _playerList.Length);

        for (int i = 0; i < _playerList.Length; i++)
        {
            // 각 플레이어의 방에 변경된 정보(준비됨, 준비되지 않음)를 갱신함
            if (PhotonNetwork.LocalPlayer.ActorNumber == _playerList[i].ActorNumber)
            {
                StaticObjects.MasterPlayerNum = i;
                PhotonNetwork.NickName = StaticObjects.MasterPlayerName + "_" + i;
                PV.RPC("PressReady", RpcTarget.All, StaticObjects.MasterPlayerNum, i, MasterPlayerReady);
            }
            UManager.SetPlayerNameInRoom(i, _playerList[i].NickName.Split('_')[0]);
        }
        // 방 정보가 갱신되었음을 정의
        MenuNetwork.IsRoomUpdate = false;
    }

    private IEnumerator WaitingCoroutine(string option)
    {
        // 대기중 Ui 표시
        UManager.SetUIActive(4, true);
        Debug.Log("Waiting...");

        // 이동할 Ui 번호에 따라 다음 내용을 진행
        switch (PreMenuNum)
        {
            // 1번 Ui일 경우
            case 1:
                // 플레이어가 PhotonNetwork에서 Lobby로 들어갈 때까지 대기
                while (!MenuNetwork.IsLobby())
                    yield return null;

                UManager.SetPlayerNameInLobby(StaticObjects.MasterPlayerName);
                // 들어갔을 경우, 현재 존재하는 방 버튼을 Lobby에 정의
                DisplayRoomButtons();
                break;
            // 3번 Ui일 경우
            case 3:
                // 플레이어가 PhotonNetwork에서 해당 방을 들어갈 때까지 대기
                float _timer = 0;
                while (!MenuNetwork.IsRoom())
                {
                    _timer += Time.deltaTime;
                    // 8초가 지나도록 들어가지 못할 경우, 오류를 출력하고 Lobby로 이동
                    if (_timer > 8)
                    {
                        PreMenuNum = 1;
                        MenuNetwork.DeleteRoom(option);
                        UManager.VerifyPopup(1, "This room does not Exist");
                        break;
                    }
                    yield return null;
                }

                // 8초 이전에 방에 들어갈 경우 방 갱신을 정의
                if (_timer < 8)
                    MenuNetwork.IsRoomUpdate = true;
                break;
        }

        // 대기 화면 비활성화 및 다음 화면 활성화
        UManager.SetUIActive(4, false);
        UManager.SetUIActive(PreMenuNum, true);
        yield return null;
    }

    [PunRPC]
    private void PressReady(int _num, bool _isReady)
    {
        AllPlayerReady[_num] = _isReady;
        UManager.SetPlayerReadyInRoom(_num, _isReady);
    }

    [PunRPC]
    private void PressReady(int _prenum, int _num, bool _isReady)
    {
        // 플레이어 이동에 따른 방 정보 갱신 진행
        AllPlayerReady[_prenum] = false;
        UManager.SetPlayerReadyInRoom(_prenum, false);
        AllPlayerReady[_num] = _isReady;
        UManager.SetPlayerReadyInRoom(_num, _isReady);
    }

    [PunRPC]
    public void PlayerStartGame(int _timer)
    {
        OManager.DesideActivateModel();
        StaticObjects.GamePlayTime = _timer;
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    [PunRPC]
    private void AllPlayerLeaveRoom()
    {
        MasterPlayerReady = false;
        MenuNetwork.LeaveRoom();
        SetMenuActive(1, null);
    }
}
