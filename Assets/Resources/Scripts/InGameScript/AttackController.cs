using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackController : MonoBehaviourPunCallbacks
{
    // 공격 주기 변수
    [SerializeField]
    private float AttackCycle;
    // 공격 눈덩이의 생성 위치
    [SerializeField]
    private Transform BallGeneratePos;
    [SerializeField]
    private string ballObjectPos;

    // 플레이어가 공을 던진 후의 시간
    private float CurAttackTime;
    // 플레이어의 PhotonView 객체
    private PhotonView MasterPv;
    // 플레이어의 상태 객체
    private PlayerAttribute PlayerAtt;
    private MasterUIManager UIManager;
    private PlayerManager PManager;

    private bool IsAttack;

    void Start()
    {

        MasterPv = GetComponent<PhotonView>();
        PlayerAtt = GetComponent<PlayerAttribute>();
        UIManager = GameObject.Find("MainCamera").GetComponent<MasterUIManager>();
        PManager = GameObject.Find("StaticObjects").GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(MasterPv.IsMine && !UIManager.IsMouseVisible)
            TryAttack();
    }

    private void TryAttack()
    {
        if(!IsAttack)
        {
            CurAttackTime += Time.deltaTime;
            if(CurAttackTime > AttackCycle)
            {
                CurAttackTime = 0;
                IsAttack = true;
            }
        }

        if(Input.GetMouseButton(0) && IsAttack)
        {
            Attack(BallGeneratePos.position, transform.rotation);
            IsAttack = false;
        }
    }

    private void Attack(Vector3 ballPosition, Quaternion ballRotation)
    {
        GameObject _clone = PhotonNetwork.Instantiate(ballObjectPos, ballPosition, ballRotation, 0);
        _clone.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        _clone.GetComponent<BallController>().BallDamage = PlayerAtt.PlayerDamage;
        _clone.GetComponent<BallController>().ThrowPlayerNum = PlayerAtt.PlayerNumber;
        StaticObjects.BallCount = 1;
        Debug.Log(StaticObjects.BallCount);
    }

    [PunRPC]
    public void AttackingPlayer(int PlayerNumber, float PlayerDamage)
    {
        GameObject AttackedPlayer = GameObject.Find(PlayerNumber.ToString());
        AttackedPlayer.GetComponent<PlayerAttribute>().PlayerHealth = PlayerDamage;
        if (AttackedPlayer.GetComponent<PlayerAttribute>().PlayerHealth <= 0)
            PManager.PlayerDead(PlayerNumber);
    }
}
