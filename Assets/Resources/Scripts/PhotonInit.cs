using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject PlayerModel;
    [SerializeField]
    private int PlayerNumbers;
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private Transform[] Beacon;

    private int MasterPlayerPos;
    private PhotonView pv;
    private bool[] BeaconPosition;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // SettingPlayerModelName 함수를 위한 PhotonView 컴포넌트
        pv = GetComponent<PhotonView>();
        BeaconPosition = new bool[Beacon.Length];
        Debug.Log(PhotonNetwork.NickName);
        Debug.Log(PhotonNetwork.CurrentRoom);
        MasterPlayerPos = int.Parse(PhotonNetwork.NickName.Split('_')[1]);
        StartCoroutine(CreatePlayer());
    }

    IEnumerator CreatePlayer()
    {
        // 캐릭터 모델을 복사해 Player 캐릭터 생성
        GameObject _player = PhotonNetwork.Instantiate(PlayerModel.name, 
                                                        Beacon[MasterPlayerPos].position, 
                                                        Quaternion.identity, 0);
        // Player 캐릭터의 세부 사항 설정
        _player.name = MasterPlayerPos.ToString();
        _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;
        _player.GetComponent<PlayerAttribute>().setAttackDamage(5);
        _player.GetComponent<PlayerAttribute>().setPlayerNumb(MasterPlayerPos);
        yield return null;
    }
}
