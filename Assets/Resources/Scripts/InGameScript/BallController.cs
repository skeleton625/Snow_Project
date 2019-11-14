using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallController : MonoBehaviourPunCallbacks
{
    // 공의 속성 값
    [SerializeField]
    private float ballSpeed;
    [SerializeField]
    private float ballForce;
    private float ballDamage;
    public float BallDamage
        { set { ballDamage = value; } }
    private int throwPlayerNum;
    public int ThrowPlayerNum
        { set { throwPlayerNum = value; } }

    [SerializeField]
    private float limitTime;
    private float curTime;

    [SerializeField]
    private GameObject hitEffect;

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
        int _parsedName;
        // 공과 충돌한 Player의 번호를 파악
        string _objectName = collision.gameObject.name;
        if (_objectName == throwPlayerNum+"")
            return;

        Vector3 conflictPos = collision.contacts[0].point;
        Vector3 conflictRot = collision.contacts[0].normal;
        GameObject clone = Instantiate(hitEffect, conflictPos, Quaternion.LookRotation(conflictRot));
        /* 생성된 피격 효과 오브젝트가 2초 뒤에 삭제되도록 함 */
        Destroy(clone, 1f);
        
        // 충돌한 물체가 Player일 경우
        if (int.TryParse(_objectName, out _parsedName))
        {
            collision.gameObject.GetComponent<UIController>().VisibleHealthBar();

            // 충돌한 Player의 피해를 다른 플레이어들에게도 갱신해 줌
            PhotonView pv = GameObject.Find(_objectName).GetComponent<PhotonView>();
            pv.RPC("AttackingPlayer", RpcTarget.All, _parsedName, ballDamage, gameObject);

            // 충돌한 Player의 Rigidbody를 통해 넉백을 진행
            Vector3 _knockBack = collision.contacts[0].point.normalized + new Vector3(0, 1f, 0);
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(_knockBack * ballForce);
        }
    }
}
