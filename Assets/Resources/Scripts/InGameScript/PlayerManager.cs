﻿using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private StaticObjects Models;
    [SerializeField]
    private string PlayerDeadEffectPos;
    [SerializeField]
    private int PlayerNumbers;
    [SerializeField]
    private Camera MainCamera;
    

    private int masterPlayerNum;
    public int MasterPlayerNum
    {
        get
        {
            return masterPlayerNum;
        }
    }
    private bool IsDead;

    void Start()
    {
        masterPlayerNum = int.Parse(PhotonNetwork.NickName.Split('_')[1]);
        StartCoroutine(CreatePlayer());
    }

    public void PlayerDead(int _playerNum)
    {
        GameObject _player = Models.GetPlayerModels(_playerNum);

        if (_player.name == masterPlayerNum + "")
        {
            MainCamera.transform.parent = null;
            StartCoroutine(MainCamera.GetComponent<MasterUIManager>().ActivateDeadScene(5));
            IsDead = true;
        }

        StartCoroutine(PlayerDeadMotion(_playerNum, _player.transform.position,
                                        _player.transform.localEulerAngles));
        _player.SetActive(false);
    }

    private IEnumerator PlayerDeadMotion(int _playerNum, Vector3 _deadPos, Vector3 _deadRot)
    {
        GameObject _dead = Models.GetPlayerDeads(_playerNum);
        // 죽은 위치에서 죽는 오브젝트 Enable
        _dead.transform.position = _deadPos;
        _dead.transform.rotation = Quaternion.Euler(_deadRot);
        _dead.SetActive(true);

        // Dead Model을 죽는 것 처럼 표현 -> Animation으로 대체가 필요함
        float _xRot = 0;
        while (_xRot > -90)
        {
            _xRot = Mathf.Lerp(_xRot, -91, 0.1f);
            _dead.transform.localEulerAngles = new Vector3(_xRot, _deadRot.y, 0);
            yield return null;
        }
        // 죽음 이펙트 표현
        GameObject _deadEffect =
            Instantiate(Resources.Load(PlayerDeadEffectPos) as GameObject, _deadPos, Quaternion.Euler(-90, 0, 0));

        // 죽는 오브젝트 Disable
        yield return new WaitForSeconds(0.5f);
        _dead.SetActive(false);

        // Player Dead Effect 생성 및 3초 뒤 제거
        Destroy(_deadEffect, 3f);
        yield return null;
    }

    private IEnumerator CreatePlayer()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!StaticObjects.GetPlayerExist(i))
                continue;

            GameObject _player = Models.GetPlayerModels(i);
            _player.SetActive(true);
            if (i == masterPlayerNum)
                _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;

            _player.GetComponent<PlayerAttribute>().PlayerNumber = i;
            _player.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
        }

        yield return null;
    }

    public void PlayerOutTheGame(int _num)
    {
        GetComponent<PhotonView>().RPC("PlayerDeactive", RpcTarget.All, _num);
    }

    [PunRPC]
    private void PlayerDeactive(int _num)
    {
        Models.GetPlayerModels(_num).SetActive(false);
    }
}
