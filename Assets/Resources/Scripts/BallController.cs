using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private float ballSpeed;
    [SerializeField]
    private float limitTime;
    private float curTime;

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
            GameObject AttackedPlayer = collision.gameObject;
            AttackedPlayer.GetComponent<PlayerAttribute>().setHealthBar(ballDamage);
            Debug.Log(AttackedPlayer.GetComponent<PlayerAttribute>().getHealthBar());
        }
        Destroy(gameObject);
    }
}
