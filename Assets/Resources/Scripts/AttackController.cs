using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public bool isControllable;

    // 공격 주기 변수
    [SerializeField]
    private float attackCycle;
    // 공격 눈덩이의 생성 위치
    [SerializeField]
    private Transform ballGeneratePos;

    private float curAttackTime;
    private GameObject AttackBall;
    private PlayerAttribute playerAtt;
    private PhotonView pv;

    private bool isAttack = true;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        playerAtt = GetComponent<PlayerAttribute>();
        AttackBall = Resources.Load("SnowBall") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(isControllable && pv.isMine)
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
            Attack();
            isAttack = false;
        }
    }

    private void Attack()
    {
        GameObject _clone = PhotonNetwork.Instantiate(AttackBall.name,ballGeneratePos.position, transform.rotation, 0);
        _clone.name = "SnowBall_" + playerAtt.getPlayerNumb()+"_"+playerAtt.getAttackDamage();
        _clone.GetComponent<MeshRenderer>().material = StaticObjects.getMaterial(playerAtt.getPlayerNumb());
    }

    private void OnCollisionEnter(Collision collision)
    {
        string ObjectName = collision.gameObject.name;
        if (ObjectName.Length < AttackBall.name.Length)
            return;

        if(ObjectName.Substring(0, AttackBall.name.Length) == AttackBall.name)
        {
            string[] PlayerInfo = ObjectName.Split('_');

            if (playerAtt.getPlayerNumb().ToString() != PlayerInfo[1])
            {
                Debug.Log(playerAtt.getHealthBar());
                playerAtt.setHealthBar(int.Parse(PlayerInfo[2]));
                Destroy(collision.gameObject);
            }
        }
    }
}
