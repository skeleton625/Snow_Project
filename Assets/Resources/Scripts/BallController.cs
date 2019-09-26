﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private float ballSpeed;
    [SerializeField]
    private float limitTime;
    private float curTime;

    // Update is called once per frame
    void Update()
    {
        ThrowBall();
    }

    private void ThrowBall()
    {
        if(curTime < limitTime)
        {
            curTime += Time.deltaTime;
            transform.Translate(Vector3.forward * ballSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name.Substring(0, 5) != "Player")
        {
            Destroy(gameObject);
        }
            
    }
}