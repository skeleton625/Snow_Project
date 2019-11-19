using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // GameScene 내의 메인 카메라 객체
    [SerializeField]
    private Camera MainCamera;
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

    private int masterPlayerNum;
    public int MasterPlayerNum
        { get{ return masterPlayerNum; } }

    void Start()
    {
        // 현재 클라이언트의 플레이어 번호를 정의
        masterPlayerNum = int.Parse(PhotonNetwork.NickName.Split('_')[1]);
            
        CreateAllPlayer();
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

        string _playerName = PhotonNetwork.PlayerList[_num].NickName;
        string _attackPlayerName = PhotonNetwork.PlayerList[_attackNum].NickName;
        SettingKillingBlocks(_playerName, _attackPlayerName);
        PlayerDeadCoroutine(_num, _player.transform.position, _player.transform.localEulerAngles, 5);
        _player.SetActive(false);
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
            if (i == masterPlayerNum)
                _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;

            GetComponent<InGameObjects>().GenAttackSnowBall(i);
            GetComponent<InGameObjects>().GenAttackEffect(i);
            _player.GetComponent<PlayerController>().enabled = true;
            _player.GetComponent<AttackController>().enabled = true;
            _player.GetComponent<PlayerAttribute>().enabled = true;
            _player.GetComponent<UIController>().enabled = true;
        }
    }

    private IEnumerator CreatePlayer(int _num, float _cnt)
    {
        GameObject _player = Models.GetPlayerModels(_num);
        _player.GetComponent<PlayerController>().enabled = false;
        _player.GetComponent<AttackController>().enabled = false;
        _player.GetComponent<PlayerAttribute>().enabled = false;
        _player.GetComponent<UIController>().enabled = false;

        yield return new WaitForSeconds(_cnt);
        if (_num == masterPlayerNum)
            _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;

        _player.GetComponent<PlayerController>().InitPlayerController();
        _player.GetComponent<UIController>().InitPlayerHealthBar();

        _player.GetComponent<PlayerController>().enabled = true;
        _player.GetComponent<AttackController>().enabled = true;
        _player.GetComponent<PlayerAttribute>().enabled = true;
        _player.GetComponent<UIController>().enabled = true;
        _player.SetActive(true);
    }

    public float GetPlayerDamage(int _player)
    {
        return PlayerAttackDamage[_player];
    }

    public void PlayerOutTheGame(int _num)
    {
        GetComponent<PhotonView>().RPC("PlayerDeactive", RpcTarget.All, _num);
    }

    [PunRPC]
    private void PlayerDeactive(int _num)
    {
        Models.GetPlayerModels(_num).SetActive(false);
    }

    private void SettingKillingBlocks(string _player, string _attackPlayer)
    {
        for(int i = 0; i < KillingList.Length; i++)
        {
            if (!KillingList[i].activeSelf)
            {
                GetComponent<PhotonView>().RPC("ActivateKillingBlocks", RpcTarget.All,
                                                            i, _player, _attackPlayer);
                break;
            }
                
        }
    }

    [PunRPC]
    private void ActivateKillingBlocks(int _num, string _player, string _attackPlayer)
    {
        StartCoroutine(KillingBlockCoroutine(_num, _player, _attackPlayer));
    }

    private IEnumerator KillingBlockCoroutine(int _num, string _player, string _attackPlayer)
    {
        KillingList[_num].GetComponent<KillBlock>().SetPlayerNames(_player, _attackPlayer);
        KillingList[_num].SetActive(true);

        yield return new WaitForSeconds(5f);

        KillingList[_num].SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetComponent<PhotonView>().RPC("PlayerDeactive", RpcTarget.All, 
                            int.Parse(otherPlayer.NickName.Split('_')[1]));
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
