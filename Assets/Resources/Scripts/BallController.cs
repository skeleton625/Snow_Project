using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float ballSpeed;
    [SerializeField]
    private float limitTime;
    private float curTime;

    private GameObject AttackedPlayer;
    private int isAttacked;
    public float ballDamage;
    public int ThrowPlayer;

    // Update is called once per frame
    void Update()
    {
        ThrowBall();
    }

    private void ThrowBall()
    {
        if (curTime < limitTime)
        {
            curTime += Time.deltaTime;
            transform.Translate(Vector3.forward * ballSpeed * Time.deltaTime);
        }
        else
           Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        string objectName = collision.gameObject.name;
        if(objectName != "SnowBall(Clone)" && objectName != "Wall" && objectName != (ThrowPlayer+""))
        {
            AttackedPlayer = collision.gameObject;
            isAttacked = 1;
        }
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log(isAttacked);
        if (stream.IsWriting && isAttacked == 1)
        {
            stream.SendNext(ThrowPlayer);
            stream.SendNext(ballDamage);
            stream.SendNext(AttackedPlayer);
            isAttacked = 2;
        }
        else if(isAttacked == 2)
        {
            int AttackPlayer = (int) stream.ReceiveNext();
            float AttackDamage = (float) stream.ReceiveNext();
            GameObject AttackedPlayer = (GameObject)stream.ReceiveNext();
            AttackedPlayer.GetComponent<PlayerAttribute>().setHealthBar(AttackDamage);
            Debug.Log(AttackedPlayer.GetComponent<PlayerAttribute>().getHealthBar());
            isAttacked = 0;
        }
    }
}
