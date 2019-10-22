﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonManager : MonoBehaviour
{
    private MenuManager PlayerMenu;

    void Start()
    {
        PlayerMenu = GameObject.Find("PhotonNetworkObject").GetComponent<MenuManager>();    
    }

    public void JoinLobbyButtonClick()
    {
        PlayerMenu.setPlayerName();
        PlayerMenu.setMenuActive(1, null);
    }

    public void RemovePopupButtonClick()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    public void RoomButtonClick()
    {
        PlayerMenu.setMenuActive(2, null);
    }

    public void CreateRoomButtonClick()
    {
        PlayerMenu.CreateRoomByMaster();
        PlayerMenu.setMenuActive(3, null);
    }

    public void JoinRoomButtonClick()
    {
        PlayerMenu.PlayerJoinRoom(gameObject.name);
        PlayerMenu.setMenuActive(3, null);
    }

    public void GameStartButtonClick()
    {
        PlayerMenu.PlayerStartGame();
    }

    public void CancelRoomButtonClick()
    {
        PlayerMenu.setMenuActive(1, null);
    }

    public void LeaveRoomButtonClick()
    {
        PlayerMenu.PlayerLeaveRoom();
        PlayerMenu.setMenuActive(1, null);
    }
}
