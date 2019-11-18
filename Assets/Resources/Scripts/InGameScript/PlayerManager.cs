using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string PlayerDeadEffectPos;
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private InGameObjects Models;
    [SerializeField]
    private float[] PlayerAttackDamage;

    private int masterPlayerNum;
    public int MasterPlayerNum
        { get{ return masterPlayerNum; } }

    void Start()
    {
        masterPlayerNum = int.Parse(PhotonNetwork.NickName.Split('_')[1]);

        for(int i = PhotonNetwork.PlayerList.Length; i < 4; i++)
        {
            GameObject _player = Models.GetPlayerModels(i);
            _player.SetActive(false);
        }
        CreateAllPlayer();
    }

    public void PlayerDead(int _playerNum)
    {
        GameObject _player = Models.GetPlayerModels(_playerNum);

        if (_player.name == masterPlayerNum + "")
        {
            MainCamera.transform.parent = null;
            StartCoroutine(MainCamera.GetComponent<MasterUIManager>().ActivateDeadScene(5));
        }
        StartCoroutine(PlayerDeadMotion(_playerNum, _player.transform.position,
                                        _player.transform.localEulerAngles));
        StartCoroutine(CreatePlayer(_playerNum, 5f));
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

        // 죽는 오브젝트 1초 후에 원래 각도로 변경한 뒤, 비활성화 함
        yield return new WaitForSeconds(1f);
        _dead.transform.rotation = Quaternion.identity;
        _dead.SetActive(false);

        // Player Dead Effect 생성 및 3초 뒤 제거
        Destroy(_deadEffect, 3f);
        yield return null;
    }

    private void CreateAllPlayer()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject _player = Models.GetPlayerModels(i);

            if (PhotonNetwork.PlayerList.Length == i)
                break;

            _player.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
            _player.GetComponent<PlayerAttribute>().PlayerName = 
                PhotonNetwork.PlayerList[i].NickName.Split('_')[0];
            if (i == masterPlayerNum)
                _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;

            GetComponent<InGameObjects>().GenAttackSnowBall(i);
            GetComponent<InGameObjects>().GenAttackEffect(i);
            _player.GetComponent<PlayerController>().enabled = true;
            _player.GetComponent<AttackController>().enabled = true;
            _player.GetComponent<PlayerAttribute>().enabled = true;
            _player.GetComponent<UIController>().enabled = true;
        }
    }

    private IEnumerator CreatePlayer(int _num, float _cnt)
    {
        GameObject _player = Models.GetPlayerModels(_num);
        _player.GetComponent<PlayerController>().enabled = false;
        _player.GetComponent<AttackController>().enabled = false;
        _player.GetComponent<PlayerAttribute>().enabled = false;
        _player.GetComponent<UIController>().enabled = false;

        yield return new WaitForSeconds(_cnt);
        if (_num == masterPlayerNum)
            _player.GetComponent<PlayerController>().PlayerCamera = MainCamera;

        _player.GetComponent<PlayerController>().InitPlayerController();
        _player.GetComponent<UIController>().InitPlayerHealthBar();

        _player.GetComponent<PlayerController>().enabled = true;
        _player.GetComponent<AttackController>().enabled = true;
        _player.GetComponent<PlayerAttribute>().enabled = true;
        _player.GetComponent<UIController>().enabled = true;
        _player.SetActive(true);
    }

    public float GetPlayerDamage(int _player)
    {
        return PlayerAttackDamage[_player];
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetComponent<PhotonView>().RPC("PlayerDeactive", RpcTarget.All, int.Parse(otherPlayer.NickName.Split('_')[1]));
        base.OnPlayerLeftRoom(otherPlayer);
    }
}
