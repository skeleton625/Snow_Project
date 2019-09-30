using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PhotonInit : Photon.PunBehaviour
{
    void Awake()
    {
        // PhotonNetwork에 정의된 Snow_Project에 버전별(1.0)로 접근
        PhotonNetwork.ConnectUsingSettings("Snow_Project 1.0");
    }

    // 게임 네트워크 로비에 입장됐을 때 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    // 게임 네트워크 로비에 입장하지 못했을 경우 호출되는콜백 함수
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("No Room "+codeAndMsg);
        PhotonNetwork.CreateRoom("GameRoom");
    }

    // 게임 네트워크에 룸을 생성했을 때, 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Finish make a Room");
    }

    // 게임 네트워크 룸에 입장되었을 경우 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
    }

    // 게임 네트워크의 세부 사항을  출력하는 GUI 함수
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}
