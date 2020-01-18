using System.Collections;
using UnityEngine;
using Photon.Pun;

public class AttackController : MonoBehaviour
{
    [SerializeField]
    private float KnockBackForce;
    // 공격 주기 변수
    [SerializeField]
    private float AttackCycle;
    // 공격 눈덩이의 생성 위치
    [SerializeField]
    private Transform BallGeneratePos;
    // 공격 눈덩이 주소
    [SerializeField]
    private string ballObjectPos;

    // 플레이어가 공을 던진 후의 시간
    private float CurAttackTime;
    private Rigidbody PlayerBody;
    // 플레이어의 PhotonView 객체
    private PhotonView PlayerPv;
    // GameManager 스크립트
    private PlayerManager PManager;
    // 마스터 플레이어 UI 스크립트
    private MasterUIManager UIManager;
    // 게임 내부 오브젝트 관리 스크립트
    private InGameObjectManager OManager;
    // 오디오 관리 오브젝트
    private AudioManager AManager;
    private bool IsAttack;
    private bool isProtected;
    private Vector3 KnockBackVector;

    void Start()
    {
        GameObject OManagerObject = GameObject.Find("InGameObjectManager");
        PManager = OManagerObject.GetComponent<PlayerManager>();
        OManager = InGameObjectManager.instance;
        PlayerPv = GetComponent<PhotonView>();
        PlayerBody = GetComponent<Rigidbody>();
        KnockBackVector = new Vector3(0, KnockBackForce, KnockBackForce);

        if (PlayerPv.IsMine)
        {
            GameObject MainCamera = GameObject.Find("Main Camera");
            UIManager = MainCamera.GetComponent<MasterUIManager>();
            AManager = OManagerObject.GetComponent<AudioManager>();
            AManager.InitAudioManager(MainCamera.GetComponent<AudioSource>());
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPv.IsMine && !UIManager.IsMouseVisible)
            TryAttack();
    }

    private void TryAttack()
    {
        // 플레이어가 공격 불가능할 경우
        if(!IsAttack)
        {
            // 플레이어의 다음 공격까지 남은 시간을 측정
            CurAttackTime += Time.deltaTime;
            if(CurAttackTime > AttackCycle)
            {
                CurAttackTime = 0;
                IsAttack = true;
            }
        }

        // 플레이어가 공격이 가능하고 공격 버튼을 눌렀을 경우
        if(Input.GetMouseButton(0) && IsAttack)
        {
            if (AManager)
                // 공격 사운드 출력
                AManager.PlayAudioEffect(2, 1f);
            // 플레이어의 공이 날려졌음을 다른 플레이어들에게 알림
            PlayerPv.RPC("SendGetSnowBall",RpcTarget.All, 
                         StaticObjects.MasterPlayerNum, BallGeneratePos.position, BallGeneratePos.rotation);
            // 바로 공격 불가능하도록 공격 불가능 정의
            IsAttack = false;
        }
    }

    [PunRPC]
    private void SendGetSnowBall(int _player, Vector3 pos, Quaternion rot)
    {
        OManager.GetSnowBall(_player, pos, rot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 같은 눈덩이 오브젝트의 경우 무시
        if (collision.gameObject.tag == "SnowBall")
        {
            // 공격한 사람과 당한 사람 번호를 정의
            int _player = int.Parse(collision.gameObject.name.Split('_')[0]);
            int _me = int.Parse(gameObject.name);
            // 자기 공에 대해선 무시
            if (_player == _me)
                return;

            // 공격당한 플레이어를 넉백시킴
            PlayerBody.AddForce(KnockBackVector, ForceMode.Impulse);

            // 공격 당했음을 모두에게 알림
            if (PlayerPv.IsMine)
                PlayerPv.RPC("SendPlayerAttacked", RpcTarget.All, _me, _player);
        }
    }

    [PunRPC]
    private void SendPlayerAttacked(int _player, int _attackPlayer)
    {
        // 각 플레이어 모델에 피해 적용
        GameObject AttackedPlayer = OManager.GetPlayerModels(_player);
        AttackedPlayer.GetComponent<PlayerAttribute>().PlayerHealth =
                                    PManager.GetPlayerDamage(_attackPlayer);
        AttackedPlayer.GetComponent<UIController>().SetPlayerHealthBar();

        if(_player != StaticObjects.MasterPlayerNum)
            AttackedPlayer.GetComponent<UIController>().VisibleHealthBar();

        // 공격 당한 플레이어의 체력이 0보다 작을 경우, 해당 플레이어 사망
        if (AttackedPlayer.GetComponent<PlayerAttribute>().PlayerHealth <= 0)
            PManager.PlayerDead(_player, _attackPlayer);
    }
}
