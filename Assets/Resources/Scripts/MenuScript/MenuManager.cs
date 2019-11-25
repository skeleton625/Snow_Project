using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] UI;
    [SerializeField]
    private GameObject UserNameInput;
    [SerializeField]
    private Text UserNameText;

    private bool PlayerReady;
    private bool[] IsReady;
    private int PreMenuNum;
    private int PlayerNumber;
    private PhotonView PV;
    private PhotonInMenu PhotonNet;

    void Start()
    {
        PlayerReady = false;
        PlayerNumber = 0;
        IsReady = new bool[4];
        PV = GetComponent<PhotonView>();
        PhotonNet = GetComponent<PhotonInMenu>();
        UserNameText.text = PhotonNetwork.NickName.Split('_')[0];
        if (PhotonNet.LeaveRoom())
            setMenuActive(1, null);
    }

    void Update()
    {
        if (PhotonNet.IsLobbyUpdate)
            CreateRoomButtons();
        if (PhotonNet.IsRoomUpdate)
            SettingRoomPlayers();
    }

    public void setPlayerName()
    {
        string _name = UserNameInput.GetComponent<InputField>().text;
        if (_name == "")
            _name = "NoName";

        UserNameText.text = _name;
        PhotonNet.OnLogin(_name);
    }

    public void setMenuActive(int _num, string option)
    {
        UI[PreMenuNum].SetActive(false);
        PreMenuNum = _num;

        switch (_num)
        {
            case 1: case 3:
                if (!PhotonNet.IsRoom() || !PhotonNet.IsLobby())
                    StartCoroutine(WaitingCoroutine(option));
                break;
            default:
                UI[_num].SetActive(true);
                break;
        }
    }

    public void CreateRoomByMaster()
    {
        string _roomName = GameObject.Find("RoomName/InputField").GetComponent<InputField>().text;
        StaticObjects.GamePlayTime = int.Parse(GameObject.Find("PlayTime/InputField")
                                                                .GetComponent<InputField>().text);
        if (!PhotonNet.CreateRoom(_roomName))
        {
            GeneratePopup(1, "This Room name is already Exist");
            setMenuActive(1, _roomName);
            return;
        }
        setMenuActive(3, _roomName);
    }

    public void PlayerJoinRoom(string _roomName)
    {
        PhotonNet.JoinRoom(_roomName);
        setMenuActive(3, _roomName);
    }

    public void PlayerLeaveRoom()
    {
        if (PhotonNet.IsRoomMaster())
            PV.RPC("AllPlayerLeaveRoom", RpcTarget.All);
        else
        {
            PlayerReady = false;
            PlayerNumber = 0;
            for (int i = 0; i < IsReady.Length; i++)
                IsReady[i] = false;
            PhotonNet.LeaveRoom();
        }
    }

    public void MasterStartGame()
    {
        if (PlayerNumber == 0)
        {
            if (!PressGameStart())
                GeneratePopup(3, "Someone don't press Ready Button");
            else
            {
                PV.RPC("PlayerStartGame", RpcTarget.All, StaticObjects.GamePlayTime);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            PlayerReady = !PlayerReady;
            PV.RPC("PressReady", RpcTarget.All, PlayerNumber, PlayerReady);
        }
    }

    private bool PressGameStart()
    {
        for (int i = 1; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!IsReady[i])
                return false;
        }
        return true;
    }

    private void CreateRoomButtons()
    {
        Vector3 ButtonInit = new Vector3(0, -50, 0);
        List<string> RoomNames = PhotonNet.GetRoomNames();
        GameObject ButtonModel = Resources.Load("Prefabs/UI/RoomButton") as GameObject;
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
        Player[] _playerList = PhotonNet.GetPlayerList();
        UI[3].transform.Find("Header/RoomName").GetComponent<Text>().text
            = PhotonNet.GetCurrentRoomName();

        for (int i = 0; i < 4; i++)
        {
            GameObject PlayerName = UI[3].transform.Find("Player " + i + "/Username").gameObject;
            PlayerName.GetComponent<Image>().color = Color.white;
            PlayerName.GetComponentInChildren<Text>().text = "None";
        }

        for (int i = 0; i < _playerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == _playerList[i].ActorNumber)
            {
                PhotonNetwork.NickName = PhotonNetwork.NickName.Split('_')[0] + "_" + i;
                PV.RPC("PressReady", RpcTarget.All, PlayerNumber, i, PlayerReady);
                PlayerNumber = i;
            }
            GameObject PlayerPos = UI[3].transform.Find("Player " + i + "/Username/Text").gameObject;
            PlayerPos.GetComponent<Text>().text = _playerList[i].NickName.Split('_')[0];
        }

        PhotonNet.IsRoomUpdate = false;
    }

    private void GeneratePopup(int _num, string _inform)
    {
        GameObject _popup = Instantiate(Resources.Load("Prefabs/UI/Popup") as GameObject, UI[_num].transform);

        _popup.transform.Find("Content/Text").GetComponent<Text>().text = _inform;
    }

    private IEnumerator WaitingCoroutine(string option)
    {
        UI[4].SetActive(true);
        Debug.Log("Waiting...");

        switch (PreMenuNum)
        {
            case 1:
                while (!PhotonNet.IsLobby())
                    yield return null;

                CreateRoomButtons();
                break;
            case 3:
                float _timer = 0;

                while (!PhotonNet.IsRoom())
                {
                    _timer += Time.deltaTime;
                    if (_timer > 8)
                    {
                        PreMenuNum = 1;
                        PhotonNet.DeleteRoom(option);
                        GeneratePopup(1, "This room does not Exist");
                        break;
                    }
                    yield return null;
                }

                if (_timer < 8)
                    PhotonNet.IsRoomUpdate = true;

                break;
        }

        UI[4].SetActive(false);
        UI[PreMenuNum].SetActive(true);
        yield return null;
    }

    [PunRPC]
    private void PressReady(int _num, bool _isReady)
    {
        GameObject UserName = UI[3].transform.Find("Player " + _num + "/Username").gameObject;
        IsReady[_num] = _isReady;
        if (IsReady[_num])
            UserName.GetComponent<Image>().color = Color.green;
        else
            UserName.GetComponent<Image>().color = Color.white;
    }

    [PunRPC]
    private void PressReady(int _prenum, int _num, bool _isReady)
    {
        GameObject PreUserName = UI[3].transform.Find("Player " + _prenum + "/Username").gameObject;
        GameObject UserName = UI[3].transform.Find("Player " + _num + "/Username").gameObject;

        IsReady[_prenum] = false;
        PreUserName.GetComponent<Image>().color = Color.white;
        IsReady[_num] = _isReady;

        if (IsReady[_num])
            UserName.GetComponent<Image>().color = Color.green;
        else
            UserName.GetComponent<Image>().color = Color.white;
    }

    [PunRPC]
    public void PlayerStartGame(int _timer)
    {
        StaticObjects.MasterPlayerNumber = PlayerNumber;
        StaticObjects.GamePlayTime = _timer;
        SceneManager.LoadScene("GameScene");
    }

    [PunRPC]
    private void AllPlayerLeaveRoom()
    {
        PlayerReady = false;
        PhotonNet.LeaveRoom();
        setMenuActive(1, null);
    }
}
