using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject StartMenu;
    [SerializeField]
    private GameObject LobbyMenu;
    [SerializeField]
    private GameObject CreateRoomMenu;
    [SerializeField]
    private GameObject RoomMenu;
    [SerializeField]
    private GameObject WaitingUI;

    private PhotonInit2 PhotonNet;
    private int PreMenuNum;

    void Start()
    {
        PhotonNet = GetComponent<PhotonInit2>();
    }

    void Update()
    {
        if (PhotonNet.IsLobbyUpdate)
            CreateRoomButtons();
        if (PhotonNet.IsRoomUpdate)
            SettingRoomInfo();
    }

    public void setPlayerName(string _playerName)
    {
        PhotonNet.OnLogin(_playerName);
    }

    public void setMenuActive(int _num)
    {
        switch(PreMenuNum)
        {
            case 0:
                StartMenu.SetActive(false);
                break;
            case 1:
                LobbyMenu.SetActive(false);
                break;
            case 2:
                CreateRoomMenu.SetActive(false);
                break;
            case 3:
                RoomMenu.SetActive(false);
                break;
        }

        PreMenuNum = _num;

        switch (_num)
        {
            case 0:
                StartMenu.SetActive(true);
                break;
            case 1:
                if (!PhotonNet.IsLobbyConnected)
                {
                    WaitingUI.SetActive(true);
                    StartCoroutine(WaitingCoroutine(_num));
                    break;
                }

                LobbyMenu.SetActive(true);
                break;
            case 2:
                CreateRoomMenu.SetActive(true);
                break;
            case 3:
                if (!PhotonNet.IsRoomConnected)
                {
                    WaitingUI.SetActive(true);
                    StartCoroutine(WaitingCoroutine(_num));
                    break;
                }
                RoomMenu.transform.Find("Header/RoomName").GetComponent<Text>().text =
                    PhotonNet.GetCurrentRoomName();
                RoomMenu.SetActive(true);
                break;
        }
    }

    private IEnumerator WaitingCoroutine(int _num)
    {
        float _timer = 0;
        Debug.Log("Waiting...");
        switch(_num)
        {
            case 1:
                while (!PhotonNet.IsLobbyConnected)
                    yield return null;
                CreateRoomButtons();
                break;
            case 3:
                while (!PhotonNet.IsRoomConnected)
                {
                    _timer += Time.deltaTime;
                    if (_timer > 8)
                    {
                        PreMenuNum = 1;
                        break;
                    }
                    yield return null;
                }
                SettingRoomInfo();
                break;
        }

        WaitingUI.SetActive(false);
        setMenuActive(PreMenuNum);
    }

    public void CreateRoomByMaster(string _roomName)
    {
        if (!PhotonNet.CreateRoom(_roomName))
            setMenuActive(1);
    }

    private void CreateRoomButtons()
    {
        Vector3 ButtonInit = new Vector3(0, -50, 0);
        List<string> RoomNames = PhotonNet.GetRoomNames();
        GameObject ButtonModel = Resources.Load("RoomButton") as GameObject;
        GameObject Content = LobbyMenu.transform.Find("SelectRoomBar/Viewport/Content").gameObject;

        for(int i = 0; i < Content.transform.childCount; i++)
            Destroy(Content.transform.GetChild(i).gameObject);

        for(int i = 0; i < RoomNames.Count ; i++)
        {
            GameObject _button = Instantiate(ButtonModel, Content.transform);
            _button.name = RoomNames[i];
            _button.transform.localPosition = new Vector3(_button.transform.localPosition.x, -50 - 100 * i, 0);
            _button.transform.Find("Text").GetComponent<Text>().text = RoomNames[i];
        }

        PhotonNet.IsLobbyUpdate = false;
    }

    private void SettingRoomInfo()
    {
        List<string> _playerList = PhotonNet.GetPlayerList();
        for(int i = 0; i < 4; i++)
        {
            GameObject PlayerPos = RoomMenu.transform.Find("Player " + i + "/Username/Text").gameObject;
            PlayerPos.GetComponent<Text>().text = "None";
        }

        for (int i = 0; i < _playerList.Count; i++)
        {
            GameObject PlayerPos = RoomMenu.transform.Find("Player " + i + "/Username/Text").gameObject;
            PlayerPos.GetComponent<Text>().text = _playerList[i];
        }
        PhotonNet.IsRoomUpdate = false;
    }

    public void PlayerJoinRoom(string _roomName)
    {
        PhotonNet.JoinRoom(_roomName);
    }

    public void PlayerLeaveRoom()
    {
        if (PhotonNet.IsRoomMaster())
        {
            GetComponent<PhotonView>().RPC("AllPlayerLeaveRoom", RpcTarget.All);
        }
        else
            PhotonNet.LeaveRoom();
    }

    [PunRPC]
    public void AllPlayerLeaveRoom()
    {
        Debug.Log("Activate");
        PhotonNet.LeaveRoom();
        setMenuActive(1);
    }
}
