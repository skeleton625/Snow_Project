﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private float ActivateFullTime;
    [SerializeField]
    private Text NickNameBar;
    [SerializeField]
    private Slider HealthBar;
    [SerializeField]
    private GameObject HealthUI;
    [SerializeField]
    private PlayerAttribute PlayerAtt;

    private bool IsActivate;
    private float ActivateTime;
    private float PrePlayerHealth;
    private Transform CameraPos;
    private PhotonView PlayerPv;

    void Start()
    {
        NickNameBar.text = PlayerAtt.PlayerName;
        PlayerPv = GetComponent<PhotonView>();
        CameraPos = GameObject.Find("Main Camera").transform;
        if (PlayerPv.IsMine)
            HealthBar = GameObject.Find("CharacterUI").transform
                                                      .GetChild(0)
                                                      .GetComponent<Slider>();
        InitPlayerHealthBar();
    }

    void Update()
    {
        if(!PlayerPv.IsMine)
            HealthUI.transform.rotation = CameraPos.rotation;
    }

    public void InitPlayerHealthBar()
    {
        PlayerAtt.PlayerHealth = 0;
        HealthBar.value = 1;
        IsActivate = false;
        HealthUI.SetActive(false);
    }

    public void SetPlayerHealthBar()
    {
        HealthBar.value = PlayerAtt.PlayerHealth / PlayerAtt.PlayerHealthMax;
    }

    public void VisibleHealthBar()
    {
        ActivateTime = 0;
        if (!IsActivate)
            StartCoroutine(ActivateHealthBar());
    }

    private IEnumerator ActivateHealthBar()
    {
        IsActivate = true;
        HealthUI.SetActive(true);
        while(ActivateTime < ActivateFullTime)
        {
            ActivateTime += Time.deltaTime;
            yield return null;
        }

        IsActivate = false;
        HealthUI.SetActive(false);
        yield return null;
    }
}
