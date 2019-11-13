using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInMenu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [SerializeField]
    private string GameVersion;
    [SerializeField]
    private byte PlayerNumbers;
    public bool IsLobbyUpdate;
    public bool IsRoomUpdate;

    private List<string> Rooms;
    
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
            if (room.RemovedFromList || !room.IsOpen)
                Rooms.Remove(room.Name);
            else if(!Rooms.Exists(_room=>_room == room.Name))
                Rooms.Add(room.Name);
        }
        base.OnRoomListUpdate(roomList);
        IsLobbyUpdate = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.NickName + " is joined the lobby");
        base.OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
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

    public bool JoinRoom(string _roomName)
    {
        return PhotonNetwork.JoinRoom(_roomName);
    }

    public bool LeaveRoom()
    {
        if(IsRoom())
        {
            PhotonNetwork.LeaveRoom();
            return true;
        }
        return false;
    }

    public void DeleteRoom(string room)
    {
        Rooms.Remove(room);
        IsLobbyUpdate = true;
    }

    public bool CreateRoom(string _roomName)
    {
        if (Rooms.Exists(_room => _room == _roomName))
            return false;
            
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

    public bool IsLobby()
    {
        return PhotonNetwork.InLobby;
    }

    public bool IsRoom()
    {
        return PhotonNetwork.InRoom;
    }
}
