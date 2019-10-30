using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallController : MonoBehaviourPunCallbacks
{
    // 날아가는 공의 속도
    [SerializeField]
    private float ballSpeed;
    // 공이 생성되어 날아가는 동안의 시간
    [SerializeField]
    private float limitTime;
    private float curTime;

    private GameObject hitEffect;

    // 공의 피해량
    public float ballDamage;
    // 공을 던진 Player의 번호
    public int ThrowPlayer;

    void Start()
    {
        hitEffect = Resources.Load("hitEffect") as GameObject;    
    }

    // Update is called once per frame
    void Update()
    {
        ThrowBall();
    }

    // 공을 던지는 함수
    private void ThrowBall()
    {
        // 제한 시간 동안 공이 움직이도록 함
        if (curTime < limitTime)
        {
            curTime += Time.deltaTime;
            transform.Translate(Vector3.forward * ballSpeed * Time.deltaTime);
        }
        // 제한 시간이 지나면 공을 삭제
        else
           Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 공과 충돌한 Player의 번호를 파악
        string objectName = collision.gameObject.name;
        if (objectName == ThrowPlayer+"")
            return;

        Vector3 conflictPos = collision.contacts[0].point;
        Vector3 conflictRot = collision.contacts[0].normal;
        GameObject clone = Instantiate(hitEffect, conflictPos, Quaternion.LookRotation(conflictRot));
        /* 생성된 피격 효과 오브젝트가 2초 뒤에 삭제되도록 함 */
        Destroy(clone, 1f);

        // 충돌한 Player의 PhotonView 가져옴
        PhotonView pv = GameObject.Find(objectName+"").GetComponent<PhotonView>();
        if(objectName != "SnowBall(Clone)" && objectName != "Wall")
            pv.RPC("AttackingPlayer", RpcTarget.All, int.Parse(objectName), ballDamage);
        // 충돌된 경우 공을 삭제
        Destroy(gameObject);
    }

    [PunRPC]
    public void AttackingPlayer(int PlayerNumber, float PlayerDamage)
    {
        GameObject AttackedPlayer = GameObject.Find(PlayerNumber.ToString());
        Debug.Log(PlayerNumber + " " + PlayerDamage);
        AttackedPlayer.GetComponent<PlayerAttribute>().setHealthBar(PlayerDamage);
    }
}
