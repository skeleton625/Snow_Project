﻿
using UnityEngine;
using Photon.Pun;

public class AttackController : MonoBehaviour
{
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
    // 플레이어의 PhotonView 객체
    private PhotonView PlayerPv;
    private MasterUIManager UIManager;
    private PlayerManager PManager;
    private InGameObjects Models;
    
    private bool IsAttack;

    void Start()
    {
        GameObject staticObj = GameObject.Find("StaticObjects");
        PManager = staticObj.GetComponent<PlayerManager>();
        Models = staticObj.GetComponent<InGameObjects>();
        PlayerPv = GetComponent<PhotonView>();

        if (PlayerPv.IsMine)
            UIManager = GameObject.Find("MainCamera").GetComponent<MasterUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPv.IsMine && !UIManager.IsMouseVisible)
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
            PlayerPv.RPC("SendGetSnowBall",RpcTarget.All, 
                         StaticObjects.MasterPlayerNumber, BallGeneratePos.position, BallGeneratePos.rotation);
            IsAttack = false;
        }
    }

    [PunRPC]
    private void SendGetSnowBall(int _player, Vector3 pos, Quaternion rot)
    {
        Models.GetSnowBall(_player, pos, rot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 같은 눈덩이 오브젝트의 경우 무시
        if (collision.gameObject.tag == "SnowBall")
        {
            int _player = int.Parse(collision.gameObject.name.Split('_')[0]);

            // 자기 공에 대해선 무시
            if (_player == StaticObjects.MasterPlayerNumber)
                return;

            PlayerPv.RPC("SendPlayerAttacked", RpcTarget.All, StaticObjects.MasterPlayerNumber, _player);
        }
    }

    [PunRPC]
    private void SendPlayerAttacked(int _player, int _attackPlayer)
    {
        // 각 플레이어 모델에 피해 적용
        GameObject AttackedPlayer = Models.GetPlayerModels(_player);
        AttackedPlayer.GetComponent<PlayerAttribute>().PlayerHealth =
                                    PManager.GetPlayerDamage(_attackPlayer);
        AttackedPlayer.GetComponent<UIController>().SetPlayerHealthBar();

        if(_player != StaticObjects.MasterPlayerNumber)
            AttackedPlayer.GetComponent<UIController>().VisibleHealthBar();

        // 공격 당한 플레이어의 체력이 0보다 작을 경우, 해당 플레이어 사망
        if (AttackedPlayer.GetComponent<PlayerAttribute>().PlayerHealth <= 0)
            PManager.PlayerDead(_player, _attackPlayer);
    }
}
