using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // GameScene 내의 메인 카메라 객체
    [SerializeField]
    private Camera MainCamera;
    // 전역 PhotonView 객체
    [SerializeField]
    private PhotonView StaticPv;
    // GameScene 내 모든 오브젝트 관리 객체
    [SerializeField]
    private InGameObjects Models;
    // GameScene 내 각 플레이어의 피해 값 변수
    [SerializeField]
    private float[] PlayerAttackDamage;
    // 플레이어 숫자 변수
    [SerializeField]
    private int PlayerNumbers;

    // KillBlock UI 객체
    [SerializeField]
    private GameObject[] KillingList;
    // 모든 플레이어의 킬, 죽은 횟수
    private Dictionary<string, int> KillDict;

    // 모든 플레이어 준비 파악 변수
    private bool[] PlayerReady;
    // 클라이언트 플레이어 번호 변수
    private int masterPlayerNum;
    public int MasterPlayerNum
        { get{ return masterPlayerNum; } }


    void Awake()
    {
        // 현재 클라이언트의 플레이어
        masterPlayerNum = int.Parse(PhotonNetwork.NickName.Split('_')[1]);
        // 각 플레이어 준비 변수 초기화
        PlayerReady = new bool[PlayerNumbers];
        KillDict = new Dictionary<string, int>();
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            KillDict.Add(PhotonNetwork.PlayerList[i].NickName, 0);
    }
    void Start()
    {
        CreateAllPlayer();
        StartCoroutine(GamePlayCoroutine());
    }

    private void PlayerDeadCoroutine(int _playerNum, Vector3 _deadPos, Vector3 _deadRot, float _timer)
    {
        // 번호에 해당하는 플레이어 죽음 모션 시작
        StartCoroutine(Models.PlayerDeadMotion(_playerNum, _deadPos, _deadRot));
        // 5초가 지난 뒤, 해당 플레이어 생성
        StartCoroutine(CreatePlayer(_playerNum, _timer));
    }

    private void CreateAllPlayer()
    {
        int _playerNums = PhotonNetwork.PlayerList.Length;
        // 모든 플레이어에 대해 설정 시작
        for (int i = 0; i < PlayerNumbers; i++)
        {
            // 남은 플레이어 모델의 경우 비활성화
            if(i >= _playerNums)
            {
                Models.GetPlayerModels(i).SetActive(false);
                continue;
            }
               
            // 존재하는 플레이어의 모델을 초기화
            GameObject _player = Models.GetPlayerModels(i);
            _player.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
            _player.GetComponent<PlayerAttribute>().PlayerName = 
                                           PhotonNetwork.PlayerList[i].NickName.Split('_')[0];

            GetComponent<InGameObjects>().GenAttackSnowBall(i);
            GetComponent<InGameObjects>().GenAttackEffect(i);
            _player.GetComponent<PlayerController>().enabled = true;
            _player.GetComponent<AttackController>().enabled = true;
            _player.GetComponent<PlayerAttribute>().enabled = true;
            _player.GetComponent<UIController>().enabled = true;
        }
        StaticPv.RPC("SetPlayerReady", RpcTarget.All, masterPlayerNum);
    }
    private void SettingKillingBlocks(string _player, string _attackPlayer)
    {
        for (int i = 0; i < KillingList.Length; i++)
        {
            if (!KillingList[i].activeSelf)
            {
                StaticPv.RPC("ActivateKillingBlocks", RpcTarget.All,
                                                            i, _player, _attackPlayer);
                break;
            }
        }
    }

    private IEnumerator KillingBlockCoroutine(int _num, string _player, string _attackPlayer)
    {
        KillingList[_num].GetComponent<KillBlock>().SetPlayerNames(_player, _attackPlayer);
        KillingList[_num].SetActive(true);

        yield return new WaitForSeconds(5f);

        KillingList[_num].SetActive(false);
    }

    private IEnumerator CreatePlayer(int _num, float _cnt)
    {
        GameObject _player = Models.GetPlayerModels(_num);
        _player.transform.position = Models.GetBeacons(_num).position;

        yield return new WaitForSeconds(_cnt);
        _player.GetComponent<PlayerController>().InitPlayerController();
        _player.GetComponent<UIController>().InitPlayerHealthBar();
        _player.SetActive(true);
    }

    private IEnumerator GamePlayCoroutine()
    {
        // 모든 플레이어가 준비될 때까지 대기
        int _ready = PhotonNetwork.PlayerList.Length;
        while (_ready > 0)
        {
            if (PlayerReady[_ready - 1])
                --_ready;
            yield return null;
        }

        MasterUIManager MasterUI = MainCamera.GetComponent<MasterUIManager>();

        yield return StartCoroutine(MasterUI.ActivateGameStart(3));
        yield return StartCoroutine(MasterUI.ClockingGameTimeCoroutine());

        MasterUI.ViewPlayerKillList(KillDict);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject _player = Models.GetPlayerModels(i);
            _player.GetComponent<PlayerController>().enabled = false;
            _player.GetComponent<AttackController>().enabled = false;
        }
    }

    [PunRPC]
    private void SetPlayerReady(int _num)
    {
        PlayerReady[_num] = true;
    }

    [PunRPC]
    private void PlayerDeactive(int _num)
    {
        Models.GetPlayerModels(_num).SetActive(false);
    }

    [PunRPC]
    private void ActivateKillingBlocks(int _num, string _player, string _attackPlayer)
    {
        StartCoroutine(KillingBlockCoroutine(_num, _player, _attackPlayer));
    }

    public void PlayerDead(int _num, int _attackNum)
    {
        // 죽는 플레이어 오브젝트를 가져옴
        GameObject _player = Models.GetPlayerModels(_num);

        // 죽는 플레이어가 클라이언트의 플레이어일 경우
        if (_num == masterPlayerNum)
        {
            MainCamera.transform.parent = null;
            StartCoroutine(MainCamera.GetComponent<MasterUIManager>().ActivateDeadScene(5));
        }
        // 플레이어 킬 및 데드 횟수 측정
        ++KillDict[PhotonNetwork.PlayerList[_attackNum].NickName];

        // 플레이어 킬 표시
        string _playerName = PhotonNetwork.PlayerList[_num].NickName.Split('_')[0];
        string _attackPlayerName = PhotonNetwork.PlayerList[_attackNum].NickName.Split('_')[0];
        SettingKillingBlocks(_playerName, _attackPlayerName);
        PlayerDeadCoroutine(_num, _player.transform.position, _player.transform.localEulerAngles, 5);
        _player.SetActive(false);
    }

    public float GetPlayerDamage(int _player)
    {
        return PlayerAttackDamage[_player];
    }

    public void PlayerOutTheGame(int _num)
    {
        StaticPv.RPC("PlayerDeactive", RpcTarget.All, _num);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        StaticPv.RPC("PlayerDeactive", RpcTarget.All, int.Parse(otherPlayer.NickName.Split('_')[1]));
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
