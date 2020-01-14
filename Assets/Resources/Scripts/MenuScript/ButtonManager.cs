using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonManager : MonoBehaviour
{
    private MenuManager PlayerMenu;
    private AudioManager AManager;

    void Start()
    {
        PlayerMenu = GameObject.Find("PhotonNetwork").GetComponent<MenuManager>();
        AManager = GameObject.Find("Main Camera").GetComponent<AudioManager>();
    }

    public void JoinLobbyButtonClick()
    {
        AManager.PlayAudioEffect(0);
        PlayerMenu.SetPlayerName();
        PlayerMenu.SetMenuActive(1, null);
    }

    public void RemovePopupButtonClick()
    {
        AManager.PlayAudioEffect(0);
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void RoomButtonClick()
    {
        AManager.PlayAudioEffect(0);
        PlayerMenu.SetMenuActive(2, null);
    }

    public void CreateRoomButtonClick()
    {
        AManager.PlayAudioEffect(0);
        PlayerMenu.CreateRoomByMaster();
    }

    public void JoinRoomButtonClick()
    {
        AManager.PlayAudioEffect(0);
        PlayerMenu.PlayerJoinRoom(gameObject.name);
    }

    public void GameStartButtonClick()
    {
        PlayerMenu.MasterStartGame();
    }

    public void CancelRoomButtonClick()
    {
        AManager.PlayAudioEffect(0);
        PlayerMenu.SetMenuActive(1, null);
    }

    public void LeaveRoomButtonClick()
    {
        AManager.PlayAudioEffect(0);
        PlayerMenu.PlayerLeaveRoom();
        PlayerMenu.SetMenuActive(1, null);
    }

    public void OnChangeCharButtonClick(bool _isLeft)
    {
        AManager.PlayAudioEffect(0);
        PlayerMenu.SetPlayerModel(_isLeft);
    }
}
