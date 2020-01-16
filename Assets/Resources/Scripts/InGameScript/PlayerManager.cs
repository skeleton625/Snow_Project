using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    // GameScene 내의 메인 카메라 객체
    [SerializeField]
    private Camera MainCamera;
    // 전역 PhotonView 객체
    [SerializeField]
    private PhotonView StaticPv;
    // GameScene 내 각 플레이어의 피해 값 변수
    [SerializeField]
    private float[] PlayerAttackDamage;
    [SerializeField]
    private InGameObjectManager OManager;
    private MasterUIManager MUManager;

    // KillBlock UI 객체
    [SerializeField]
    private GameObject[] KillingList;
    // 모든 플레이어의 킬, 죽은 횟수
    private Dictionary<string, int> KillDict;

    // 현재 플레이어 명 수
    private int PlayerNumbers;
    // 모든 플레이어 준비 파악 변수
    private int PlayerReady;
    private bool[] PlayerInit;
    
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.IsMessageQueueRunning = true;
        // 현재 플레이어 명 수 초기화
        PlayerNumbers = PhotonNetwork.PlayerList.Length;
        PlayerReady = PlayerNumbers;
        // 마스터 플레이어의 MasterUIManager 스크립트 정의
        MUManager = MainCamera.GetComponent<MasterUIManager>();
        PlayerInit = new bool[PlayerNumbers];
        // 킬 리스트 관련 변수 초기화
        KillDict = new Dictionary<string, int>();
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            KillDict.Add(PhotonNetwork.PlayerList[i].NickName, 0);
    }
    private void Start()
    {
        // 플레이 시작
        StartCoroutine(GamePlayCoroutine());
    }

    private void PlayerDisplayAll()
    {
        int _playerNums = PhotonNetwork.PlayerList.Length;
        // 모든 플레이어에 대해 설정 시작
        for (int i = 0; i < PlayerNumbers; i++)
        {
            // 존재하는 플레이어의 모델을 초기화
            GameObject _player = OManager.GetPlayerModels(i);
            _player.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
            _player.GetComponent<PlayerAttribute>().PlayerName = 
                                           PhotonNetwork.PlayerList[i].NickName.Split('_')[0];
            PlayerAttribute _pa = _player.GetComponent<PlayerAttribute>();
            _pa.PlayerHealthMax = 100;
            _pa.PlayerHealth = 0;
            _pa.PlayerNumber = i;

            // 마스터 플레이어가 사용할 공 오브젝트 및 효과 오브젝트 생성
            OManager.GenAttackSnowBall(i);
            OManager.GenAttackEffect(i);
            // 마스터 플레이어의 캐릭터 스크립트 활성화
            _player.GetComponent<PlayerController>().enabled = true;
            _player.GetComponent<AttackController>().enabled = true;
            _player.GetComponent<PlayerAttribute>().enabled = true;
            _player.GetComponent<UIController>().enabled = true;
        }
    }

    private IEnumerator GamePlayCoroutine()
    {
        while(true)
        {
            int _cnt = 0;
            for(int i = 0; i < PlayerNumbers; i++)
            {
                if (PlayerInit[i])
                    ++_cnt;
            }

            if (_cnt == PlayerNumbers)
                break;
            StaticPv.RPC("IsConnected", RpcTarget.All, StaticObjects.MasterPlayerNum);
            yield return null;
        }
        // 현재 게임에서 사용할 캐릭터 오브젝트 생성
        StaticPv.RPC("InitPlayerModels", RpcTarget.All, StaticObjects.MasterPlayerNum, StaticObjects.MasterPlayerModelNum);

        // 모든 플레이어가 준비될 때까지 대기
        int _ready = PhotonNetwork.PlayerList.Length - 1;
        while (PlayerReady > 0)
            yield return null;

        // 현재 게임에서 사용할 플레이어 오브젝트 활성화
        PlayerDisplayAll();

        yield return StartCoroutine(MUManager.ActivateGameStart(3));
        yield return StartCoroutine(MUManager.ClockingGameTimeCoroutine());

        MUManager.ViewPlayerKillList(KillDict);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject _player = OManager.GetPlayerModels(i);
            _player.GetComponent<PlayerController>().enabled = false;
            _player.GetComponent<AttackController>().enabled = false;
        }
    }

    private IEnumerator PlayerReborn(int _num, float _cnt)
    {
        GameObject _player = OManager.GetPlayerModels(_num);
        _player.transform.position = OManager.GetBeacons(_num).position;

        yield return new WaitForSeconds(_cnt);
        _player.GetComponent<PlayerController>().InitPlayerController();
        _player.GetComponent<UIController>().InitPlayerHealthBar();
        _player.SetActive(true);
    }

    public void PlayerDead(int _num, int _attackNum)
    {
        // 죽는 플레이어 오브젝트를 가져옴
        GameObject _player = OManager.GetPlayerModels(_num);

        // 죽는 플레이어가 클라이언트의 플레이어일 경우
        if (_num == StaticObjects.MasterPlayerNum)
        {
            MainCamera.transform.parent = null;
            StartCoroutine(MUManager.ActivateDeadScene(5));
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

    private void PlayerDeadCoroutine(int _playerNum, Vector3 _deadPos, Vector3 _deadRot, float _timer)
    {
        // 번호에 해당하는 플레이어 죽음 모션 시작
        StartCoroutine(OManager.PlayerDeadMotion(_playerNum, _deadPos, _deadRot));
        // 5초가 지난 뒤, 해당 플레이어 생성
        StartCoroutine(PlayerReborn(_playerNum, _timer));
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

    [PunRPC]
    private void IsConnected(int _num)
    {
        PlayerInit[_num] = true;
    }

    [PunRPC]
    private void InitPlayerModels(int _MPN, int _MPMN)
    {
        if (OManager.GenPlayerObjects(_MPN, _MPMN))
        {
            Debug.Log(_MPN + " Passed");
            --PlayerReady;
        }
    }


    [PunRPC]
    private void ActivateKillingBlocks(int _num, string _player, string _attackPlayer)
    {
        StartCoroutine(KillingBlockCoroutine(_num, _player, _attackPlayer));
    }


    [PunRPC]
    private void PlayerDeactive(int _num)
    {
        OManager.GetPlayerModels(_num).SetActive(false);
        --PlayerNumbers;
        if (PlayerNumbers < 2)
        {
            StopAllCoroutines();
            MUManager.GameSet();
            MUManager.ViewPlayerKillList(KillDict);
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                GameObject _player = OManager.GetPlayerModels(i);
                _player.GetComponent<PlayerController>().enabled = false;
                _player.GetComponent<AttackController>().enabled = false;
            }
        }
    }

    public float GetPlayerDamage(int _player)
    {
        return PlayerAttackDamage[_player];
    }

    /* PhotonNetwork 관련 함수들 */

    public void PlayerOutTheGame(int _num)
    {
        StaticPv.RPC("PlayerDeactive", RpcTarget.All, _num);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        StaticPv.RPC("PlayerDeactive", RpcTarget.All, int.Parse(otherPlayer.NickName.Split('_')[1]));
        base.OnPlayerLeftRoom(otherPlayer);
    }
}
