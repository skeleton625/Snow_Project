using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string PlayerModelPos;
    [SerializeField]
    private string PlayerDeadModelPos;
    [SerializeField]
    private string PlayerDeadEffectPos;
    [SerializeField]
    private int PlayerNumbers;
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private Transform[] Beacon;
    [SerializeField]
    private float ResponTime;

    private int MasterPlayerPos;
    private float PlayerDeadTime;
    private GameObject MasterPlayer;
    private PhotonView PlayerPv;
    private bool IsDead;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // SettingPlayerModelName 함수를 위한 PhotonView 컴포넌트
        PlayerPv = gameObject.GetComponent<PhotonView>();
        MasterPlayerPos = int.Parse(PhotonNetwork.NickName.Split('_')[1]);
        StartCoroutine(CreatePlayer());
    }

    public void PlayerDead(string _playerName)
    {
        GameObject _deadPlayer = GameObject.Find(_playerName);
        Debug.Log(_deadPlayer.transform.rotation.y);
        if(_deadPlayer.name == MasterPlayer.name)
        {
            MainCamera.transform.parent = null;
            MasterPlayer.GetComponent<UIController>().VisibleDeadScene();
            IsDead = true;
        }

        StartCoroutine(PlayerDeadMotion(_deadPlayer.transform.position, _deadPlayer.transform.localEulerAngles));
        Destroy(_deadPlayer);
    }

    private IEnumerator PlayerDeadMotion(Vector3 _deadPos, Vector3 _deadRot)
    {
        // Dead Model 생성
        GameObject _deadClone = 
            Instantiate(Resources.Load(PlayerDeadModelPos) as GameObject, _deadPos, Quaternion.identity);

        float _xRot = 0;
        // Dead Model을 죽는 것 처럼 표현
        while (_xRot > -90)
        {
            _xRot = Mathf.Lerp(_xRot, -91, 0.1f);
            _deadClone.transform.localEulerAngles = new Vector3(_xRot, _deadRot.y, 0);
            yield return null;
        }
        // 죽음 이펙트 표현
        GameObject _deadEffect = 
            Instantiate(Resources.Load(PlayerDeadEffectPos) as GameObject, _deadPos, Quaternion.Euler(-90,0,0));
        // 생성한 Player Dead Model 제거
        Destroy(_deadClone, 0.5f);
        // Player Dead Effect 생성 및 3초 뒤 제거
        Destroy(_deadEffect, 3f);
        yield return null;
    }

    private IEnumerator CreatePlayer()
    {
        // 캐릭터 모델을 복사해 Player 캐릭터 생성
        GameObject _player = 
            PhotonNetwork.Instantiate(PlayerModelPos, Beacon[MasterPlayerPos].position, Quaternion.identity, 0);

        // Player 캐릭터의 세부 사항 설정
        _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;
        _player.GetComponent<PlayerAttribute>().setAttackDamage(100);
        _player.GetComponent<PlayerAttribute>().setPlayerNumb(MasterPlayerPos);
        MasterPlayer = _player;

        // 모든 Player Character명 지정
        StartCoroutine(SettingPlayerNames());
        yield return null;
    }

    private IEnumerator SettingPlayerNames()
    {
        // 이름 확인 타이머 변수
        float timer = 0;
        RaycastHit Players;
        // 3초간 Player 명을 지정
        while(timer < 3)
        {
            // 4 개의 위치에 대해 Player명을 지정
            for(int i = 0; i < 4; i++)
            {
                Vector3 _rayPos = Beacon[i].position + new Vector3(0, 2, 0);
                Physics.Raycast(_rayPos, new Vector3(0, -1, 0), out Players, 2f);

                if(Players.point != Vector3.zero)
                    Players.transform.gameObject.name = "" + i;
            }

            timer += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
