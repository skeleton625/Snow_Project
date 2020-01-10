using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonManager : MonoBehaviour
{
    private MenuManager PlayerMenu;

    void Start()
    {
        PlayerMenu = GameObject.Find("PhotonNetwork").GetComponent<MenuManager>();    
    }

    public void JoinLobbyButtonClick()
    {
        PlayerMenu.SetPlayerName();
        PlayerMenu.SetMenuActive(1, null);
    }

    public void RemovePopupButtonClick()
    {
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void RoomButtonClick()
    {
        PlayerMenu.SetMenuActive(2, null);
    }

    public void CreateRoomButtonClick()
    {
        PlayerMenu.CreateRoomByMaster();
    }

    public void JoinRoomButtonClick()
    {
        PlayerMenu.PlayerJoinRoom(gameObject.name);
    }

    public void GameStartButtonClick()
    {
        PlayerMenu.MasterStartGame();
    }

    public void CancelRoomButtonClick()
    {
        PlayerMenu.SetMenuActive(1, null);
    }

    public void LeaveRoomButtonClick()
    {
        PlayerMenu.PlayerLeaveRoom();
        PlayerMenu.SetMenuActive(1, null);
    }

    public void OnChangeCharButtonClick(bool _isLeft)
    {
        PlayerMenu.SetPlayerModel(_isLeft);
    }
}
