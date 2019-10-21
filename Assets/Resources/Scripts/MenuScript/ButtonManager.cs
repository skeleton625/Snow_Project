using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private MenuManager PlayerMenu;

    void Start()
    {
        PlayerMenu = GameObject.Find("PhotonNetworkObject").GetComponent<MenuManager>();    
    }

    public void JoinLobbyButtonClick()
    {
        string _playerName = GameObject.Find("UserName/InputField").GetComponent<InputField>().text;
        PlayerMenu.setPlayerName(_playerName);
        PlayerMenu.setMenuActive(1);
    }

    public void RemovePopupButtonClick()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    public void RoomButtonClick()
    {
        PlayerMenu.setMenuActive(2);
    }

    public void JoinRoomButtonClick()
    {
        PlayerMenu.PlayerJoinRoom(gameObject.name);
        PlayerMenu.setMenuActive(3);
    }

    public void CreateRoomButtonClick()
    {
        string _roomName = GameObject.Find("RoomName/InputField").GetComponent<InputField>().text;
        PlayerMenu.CreateRoomByMaster(_roomName);
        PlayerMenu.setMenuActive(3);
    }

    public void CancelRoomButtonClick()
    {
        PlayerMenu.setMenuActive(1);
    }

    public void LeaveRoomButtonClick()
    {
        PlayerMenu.PlayerLeaveRoom();
        PlayerMenu.setMenuActive(1);
    }
}
