using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInit2 : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [SerializeField]
    private string GameVersion;
    [SerializeField]
    private byte PlayerNumbers;

    public bool IsLobbyConnected;
    public bool IsLobbyUpdate;
    public bool IsRoomConnected;
    public bool IsRoomUpdate;

    private List<string> Rooms;
    private bool[] IsReady;

    void Awake()
    {
        Rooms = new List<string>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void OnLogin(string _nickName)
    {
        PhotonNetwork.NickName = _nickName + "_0";
        PhotonNetwork.GameVersion = this.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
                Rooms.Remove(room.Name);
            else if(!Rooms.Exists(_room=>_room == room.Name))
                Rooms.Add(room.Name);
        }
        base.OnRoomListUpdate(roomList);
        IsLobbyUpdate = true;
    }

    public override void OnJoinedLobby()
    {
        IsLobbyConnected = true;
        Debug.Log(PhotonNetwork.NickName + " is joined the lobby");
        base.OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
        IsReady = new bool[PlayerNumbers];
        IsRoomConnected = true;
        Debug.Log(PhotonNetwork.NickName + " is joined " + PhotonNetwork.CurrentRoom);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        IsRoomUpdate = true;
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        IsRoomUpdate = true;
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public void JoinRoom(string _roomName)
    {
        PhotonNetwork.JoinRoom(_roomName);
    }

    public void LeaveRoom()
    {
        IsRoomConnected = false;
        IsLobbyConnected = false;
        PhotonNetwork.LeaveRoom();
    }

    public void DeleteRoom(string room)
    {
        Rooms.Remove(room);
        IsLobbyUpdate = true;
    }

    public bool CreateRoom(string _roomName)
    {
        if (Rooms.Exists(_room => _room == _roomName))
        {
            Debug.Log("That Room is already EXISTED!!");
            return false;
        }
            
        PhotonNetwork.CreateRoom(_roomName, new RoomOptions { MaxPlayers = PlayerNumbers });
        return true;
    }

    public List<string> GetRoomNames()
    {
        List<string> RoomNames = new List<string>();
        foreach (string _name in Rooms)
            RoomNames.Add(_name);
        return RoomNames;
    }

    public Player[] GetPlayerList()
    {
        return PhotonNetwork.PlayerList;
    }

    public string GetCurrentRoomName()
    {
        return PhotonNetwork.CurrentRoom.Name;
    }

    public bool IsRoomMaster()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public void setNickNameNumbering(int _num)
    {
        PhotonNetwork.NickName += '_' + _num;
    }

    [PunRPC]
    public void PressReady(int _num, bool _isReady)
    {
        for (int i = 0; i < 4; i++)
            Debug.Log(IsReady[i]);
        IsReady[_num] = _isReady;
    }

    public bool PressGameStart()
    {
        for(int i = 1; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!IsReady[i])
                return false;
        }
        return true;
    }

}
