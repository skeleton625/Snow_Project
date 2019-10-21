using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] UI;

    private PhotonInit2 PhotonNet;
    private string currentRoomName;
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
            SettingRoomPlayers();
    }

    public void setPlayerName(string _playerName)
    {
        PhotonNet.OnLogin(_playerName);
    }

    public void setMenuActive(int _num)
    {
        UI[PreMenuNum].SetActive(false);
        PreMenuNum = _num;

        switch (_num)
        {
            case 1:
                if (!PhotonNet.IsLobbyConnected)
                {
                    StartCoroutine(WaitingCoroutine());
                    break;
                }
                break;
            case 3:
                if (!PhotonNet.IsRoomConnected)
                {
                    StartCoroutine(WaitingCoroutine());
                    break;
                }

                UI[_num].transform.Find("Header/RoomName")
                        .GetComponent<Text>().text = currentRoomName;
                break;
            default:
                UI[_num].SetActive(true);
                break;
        }
    }

    public void CreateRoomByMaster(string _roomName)
    {
        if (!PhotonNet.CreateRoom(_roomName))
        {
            setMenuActive(1);
        }
    }

    public void PlayerJoinRoom(string _roomName)
    {
        currentRoomName = _roomName;
        PhotonNet.JoinRoom(_roomName);
    }

    public void PlayerLeaveRoom()
    {
        if (PhotonNet.IsRoomMaster())
            GetComponent<PhotonView>().RPC("AllPlayerLeaveRoom", RpcTarget.All);
        else
            PhotonNet.LeaveRoom();
    }

    [PunRPC]
    public void AllPlayerLeaveRoom()
    {
        PhotonNet.LeaveRoom();
        setMenuActive(1);
    }

    private IEnumerator WaitingCoroutine()
    {
        UI[4].SetActive(true);
        Debug.Log("Waiting...");

        switch (PreMenuNum)
        {
            case 1:
                while (!PhotonNet.IsLobbyConnected)
                    yield return null;

                CreateRoomButtons();
                break;
            case 3:
                float _timer = 0;

                while (!PhotonNet.IsRoomConnected)
                {
                    _timer += Time.deltaTime;
                    if (_timer > 8)
                    {
                        PreMenuNum = 1;
                        PhotonNet.DeleteRoom(currentRoomName);
                        GeneratePopup(1, "This room does not Exist");
                        break;
                    }
                    yield return null;
                }
                break;
        }

        UI[4].SetActive(false);
        UI[PreMenuNum].SetActive(true);
        yield return null;
    }

    private void CreateRoomButtons()
    {
        Vector3 ButtonInit = new Vector3(0, -50, 0);
        List<string> RoomNames = PhotonNet.GetRoomNames();
        GameObject ButtonModel = Resources.Load("RoomButton") as GameObject;
        GameObject Content = UI[1].transform.Find("SelectRoomBar/Viewport/Content").gameObject;

        for (int i = 0; i < Content.transform.childCount; i++)
            Destroy(Content.transform.GetChild(i).gameObject);

        for (int i = 0; i < RoomNames.Count; i++)
        {
            GameObject _button = Instantiate(ButtonModel, Content.transform);
            _button.name = RoomNames[i];
            _button.transform.localPosition = new Vector3(_button.transform.localPosition.x, -50 - 100 * i, 0);
            _button.transform.Find("Text").GetComponent<Text>().text = RoomNames[i];
        }

        PhotonNet.IsLobbyUpdate = false;
    }

    private void SettingRoomPlayers()
    {
        List<string> _playerList = PhotonNet.GetPlayerList();

        for (int i = 0; i < 4; i++)
        {
            GameObject PlayerPos = UI[3].transform.Find("Player " + i + "/Username/Text").gameObject;
            PlayerPos.GetComponent<Text>().text = "None";
        }

        for (int i = 0; i < _playerList.Count; i++)
        {
            GameObject PlayerPos = UI[3].transform.Find("Player " + i + "/Username/Text").gameObject;
            PlayerPos.GetComponent<Text>().text = _playerList[i];
        }

        PhotonNet.IsRoomUpdate = false;
    }

    private void GeneratePopup(int _num, string _inform)
    {
        GameObject _popup = Instantiate(Resources.Load("Popup") as GameObject, UI[_num].transform);

        _popup.transform.Find("Content/Text").GetComponent<Text>().text = _inform;
    }
}
