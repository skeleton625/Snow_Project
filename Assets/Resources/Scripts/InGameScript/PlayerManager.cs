using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform[] Beacons;
    [SerializeField]
    private GameObject[] Players;
    [SerializeField]
    private GameObject[] PlayerDeads;
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
    private float PlayerDeadTime;
    private PhotonView PlayerPv;
    private bool IsDead;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // SettingPlayerModelName 함수를 위한 PhotonView 컴포넌트
        PlayerPv = gameObject.GetComponent<PhotonView>();
        masterPlayerNum = int.Parse(PhotonNetwork.NickName.Split('_')[1]);
        StartCoroutine(CreatePlayer());
    }

    public void PlayerDead(int _playerNum)
    {

        if(Players[_playerNum].name == masterPlayerNum+"")
        {
            MainCamera.transform.parent = null;
            MainCamera.GetComponent<MasterUIManager>().VisibleCountScene(3, true);
            IsDead = true;
        }

        StartCoroutine(PlayerDeadMotion(_playerNum, Players[_playerNum].transform.position, 
                                        Players[_playerNum].transform.localEulerAngles));
        Players[_playerNum].SetActive(false);
    }

    private IEnumerator PlayerDeadMotion(int _playerNum, Vector3 _deadPos, Vector3 _deadRot)
    {
        // 죽은 위치에서 죽는 오브젝트 Enable
        PlayerDeads[_playerNum].transform.position = _deadPos;
        PlayerDeads[_playerNum].transform.rotation = Quaternion.Euler(_deadRot);
        PlayerDeads[_playerNum].SetActive(true);

        // Dead Model을 죽는 것 처럼 표현 -> Animation으로 대체가 필요함
        float _xRot = 0;
        while (_xRot > -90)
        {
            _xRot = Mathf.Lerp(_xRot, -91, 0.1f);
            PlayerDeads[_playerNum].transform.localEulerAngles = new Vector3(_xRot, _deadRot.y, 0);
            yield return null;
        }
        // 죽음 이펙트 표현
        GameObject _deadEffect = 
            Instantiate(Resources.Load(PlayerDeadEffectPos) as GameObject, _deadPos, Quaternion.Euler(-90,0,0));

        // 죽는 오브젝트 Disable
        yield return new WaitForSeconds(0.5f);
        PlayerDeads[_playerNum].SetActive(false);

        // Player Dead Effect 생성 및 3초 뒤 제거
        Destroy(_deadEffect, 3f);
        yield return null;
    }

    private IEnumerator CreatePlayer()
    {
        for(int i = 0; i < 4; i++)
        {
            if (!StaticObjects.GetPlayerExist(i))
                continue;

            Players[i].SetActive(true);
            if (i == MasterPlayerNum)
            {
                Players[i].GetComponent<MasterUIManager>().enabled = true;
                Players[i].GetComponent<UIController>().enabled = false;
            }

            // Player 캐릭터의 세부 사항 설정
            Players[i].GetComponent<PlayerController>().PlayerCamera = MainCamera;
            Players[i].GetComponent<PlayerAttribute>().setAttackDamage(100);
            Players[i].GetComponent<PlayerAttribute>().setPlayerNumb(masterPlayerNum);
            Players[i].transform.position = Beacons[masterPlayerNum].position;
        }

        yield return null;
    }
}
