using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PhotonInGame : MonoBehaviourPunCallbacks
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
    private PhotonView pv;
    private bool IsDead;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // SettingPlayerModelName 함수를 위한 PhotonView 컴포넌트
        pv = gameObject.GetComponent<PhotonView>();
        MasterPlayerPos = int.Parse(PhotonNetwork.NickName.Split('_')[1]);
        StartCoroutine(CreatePlayer());
    }

    void Update()
    {
        if (MasterPlayer != null)
            PlayerDead();
    }

    void LateUpdate()
    {
        if (IsDead)
        {
            PlayerDelete(MasterPlayerPos + "", MasterPlayer.transform.position);
            IsDead = false;
        }
            
    }

    private void PlayerDead()
    {
        if (MasterPlayer.GetComponent<PlayerAttribute>().getPlayerDead())
        {
            Vector3 _deadPos = MasterPlayer.transform.position;
            MainCamera.transform.parent = null;
            MainCamera.transform.position = _deadPos + new Vector3(0, 1.5f, 0);
            
            pv.RPC("PlayerDelete", RpcTarget.Others, MasterPlayerPos+"", _deadPos);
            MasterPlayer.GetComponent<PlayerAttribute>().setPlayerDead(false);
            IsDead = true;
        }
    }
    
    [PunRPC]
    private void PlayerDelete(string _playerName, Vector3 _deadPos)
    {
        StartCoroutine(PlayerDeadMotion(_deadPos));
        Destroy(GameObject.Find(_playerName));
    }

    private IEnumerator PlayerDeadMotion(Vector3 _deadPos)
    {
        GameObject _deadClone = Instantiate(Resources.Load(PlayerDeadModelPos) as GameObject
                                            , _deadPos, Quaternion.identity);
        float _xRot = 0;

        while (_xRot > -90)
        {
            _xRot = Mathf.Lerp(_xRot, -91, 0.1f);
            _deadClone.transform.localEulerAngles = new Vector3(_xRot, 0, 0);
            yield return null;
        }

        GameObject _deadEffect = Instantiate(Resources.Load(PlayerDeadEffectPos) as GameObject
                                             , _deadPos, Quaternion.identity);
        Destroy(_deadClone, 0.5f);
        Destroy(_deadEffect, 3f);
        yield return null;
    }

    private IEnumerator CreatePlayer()
    {
        // 캐릭터 모델을 복사해 Player 캐릭터 생성
        GameObject _player = PhotonNetwork.Instantiate(PlayerModelPos, 
                                                        Beacon[MasterPlayerPos].position, 
                                                        Quaternion.identity, 0);
        // Player 캐릭터의 세부 사항 설정
        _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;
        _player.GetComponent<PlayerAttribute>().setAttackDamage(100);
        _player.GetComponent<PlayerAttribute>().setPlayerNumb(MasterPlayerPos);
        MasterPlayer = _player;
        StartCoroutine(SettingPlayerNames());
        yield return null;
    }

    private IEnumerator SettingPlayerNames()
    {
        float timer = 0;
        RaycastHit Players;
        while(timer < 3)
        {
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
