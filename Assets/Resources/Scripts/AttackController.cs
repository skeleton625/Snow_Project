using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField]
    private PlayerAttribute playerAtt;
    [SerializeField]
    private GameObject AttackBall;
    [SerializeField]
    private float attackCycle;
    [SerializeField]
    private Vector3 defaultBallPos;
    private float curAttackTime;
    

    private bool isAttack = true;

    // Update is called once per frame
    void Update()
    {
        TryAttack();
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
        Vector3 ballPosition = transform.position + defaultBallPos;
        GameObject _clone = Instantiate(AttackBall, ballPosition, transform.rotation);
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
