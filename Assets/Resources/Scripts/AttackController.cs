using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AttackController : MonoBehaviourPunCallbacks
{
    public bool isControllable;
    private bool isAttack = false;

    // 공격 주기 변수
    [SerializeField]
    private float attackCycle;
    // 공격 눈덩이의 생성 위치
    [SerializeField]
    private Transform ballGeneratePos;

    // 플레이어가 공을 던진 후의 시간
    private float curAttackTime;
    // Photon View 객체
    private PhotonView pv;
    // 공격 오브젝트 객체
    private GameObject AttackBall;
    // 플레이어의 상태 객체
    private PlayerAttribute playerAtt;

    private bool otherAttack = false;
    private Vector3 otherGenerate;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        playerAtt = GetComponent<PlayerAttribute>();
        AttackBall = Resources.Load("SnowBall") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if(!isAttack)
        {
            curAttackTime += Time.deltaTime;
            if(curAttackTime > attackCycle)
            {
                curAttackTime = 0;
                isAttack = true;
            }
        }

        if(Input.GetMouseButton(0) && isAttack)
        {
            Attack(ballGeneratePos.position, transform.rotation);
            isAttack = false;
        }
    }

    private void Attack(Vector3 ballPosition, Quaternion ballRotation)
    {
        GameObject _clone = PhotonNetwork.Instantiate(AttackBall.name, ballPosition, ballRotation, 0);
        _clone.GetComponent<MeshRenderer>().material = StaticObjects.getMaterial(playerAtt.getPlayerNumb());
        _clone.GetComponent<BallController>().ballDamage = playerAtt.getAttackDamage();
        _clone.GetComponent<BallController>().ThrowPlayer = playerAtt.getPlayerNumb();
    }
}
