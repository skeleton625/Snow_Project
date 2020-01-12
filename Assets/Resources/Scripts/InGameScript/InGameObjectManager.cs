using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InGameObjectManager : MonoBehaviour
{
    // 플레이어 캐릭터 관련 오브젝트들
    [SerializeField]
    private Transform[] Beacons;
    [SerializeField]
    private string[] PlayerTypes;
    [SerializeField]
    private string[] PlayerDeadTypes;
    private GameObject[] PlayerModels = new GameObject[4];
    private GameObject[] PlayerDeadModels = new GameObject[4];

    // 플레이어 죽음 효과 오브젝트
    [SerializeField]
    private string PlayerDeadEffectPos;

    // 눈덩이 오브젝트 객체들
    [SerializeField]
    private GameObject SnowBall;
    private GameObject[,] BallArray;
    private Queue<int>[] BallCylinder;

    // 눈덩이 효과 오브젝트 객체들
    [SerializeField]
    private GameObject AttackEffect;
    private Queue<GameObject> EffectCylinder;

    // 각 플레이어당 할당된 눈덩이 개수
    [SerializeField]
    private int EachBallCount;
    // 실제 눈덩이의 위치
    [SerializeField]
    private Vector3 BallGenPos;
    private int BallCount = 6;

    void Awake()
    {
        // 공 배열 및 공 탄창 초기화
        BallArray = new GameObject[4, EachBallCount];
        BallCylinder = new Queue<int>[4];
        for (int i = 0; i < 4; i++)
            BallCylinder[i] = new Queue<int>();
    }

    /* 해당 씬의 시작 시, 필요한 오브젝트 생성 함수들 */
    public bool GenPlayerObjects(int _MPN, int _MPMN)
    {
        GameObject _player = Instantiate(Resources.Load("Prefabs/"+PlayerTypes[_MPMN]) as GameObject, Beacons[_MPN].transform.position, Quaternion.identity);
        GameObject _dead = Instantiate(Resources.Load("Prefabs/" + PlayerDeadTypes[_MPMN]) as GameObject, Beacons[_MPN].transform.position, Quaternion.identity);
        PhotonView _pv = _player.GetComponent<PhotonView>();
        
        _pv.TransferOwnership(PhotonNetwork.PlayerList[_MPN]);
        _pv.ViewID = _MPN + 1;
        _player.name = _MPN + "";
        _dead.SetActive(false);
        PlayerModels[_MPN] = _player;
        PlayerDeadModels[_MPN] = _dead;

        return true;
    }

    public void GenAttackEffect(int _num)
    {
        EffectCylinder = new Queue<GameObject>();
        for (int j = 0; j < EachBallCount; j++)
        {
            // 필요한 효과 오브젝트 생성
            GameObject _effect = Instantiate(AttackEffect, BallGenPos, Quaternion.identity);

            // 비활성화 상태로 Queue에 추가
            _effect.SetActive(false);
            EffectCylinder.Enqueue(_effect);
        }
    }

    public void GenAttackSnowBall(int _num)
    {
        for (int j = 0; j < EachBallCount; j++)
        {
            // 필요한 공 오브젝트 생성
            GameObject _ball = Instantiate(SnowBall, BallGenPos, Quaternion.identity);
            _ball.name = _num + "_" + j;
            _ball.GetComponent<BallController>().BallControllerInit(_num, j, this);
            _ball.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[_num]);
            _ball.GetComponent<PhotonView>().ViewID = _num*10 + BallCount;
            ++BallCount;

            // 비활성화 상태로 Queue에 추가
            _ball.SetActive(false);
            BallArray[_num, j] = _ball;
            BallCylinder[_num].Enqueue(j);
        }
    }

    /* 플레이어 관련 함수들 */
    public Transform GetBeacons(int _num)
    {
        return Beacons[_num];
    }

    public GameObject GetPlayerModels(int _num)
    {
        return PlayerModels[_num];
    }

    public IEnumerator PlayerDeadMotion(int _num, Vector3 _deadPos, Vector3 _deadRot)
    {
        PlayerDeadModels[_num].transform.position = _deadPos;
        PlayerDeadModels[_num].transform.rotation = Quaternion.Euler(_deadRot);
        PlayerDeadModels[_num].SetActive(true);
        // 죽음 이펙트 표현
        GameObject _deadEffect =
            Instantiate(Resources.Load(PlayerDeadEffectPos) as GameObject, 
                                    _deadPos, Quaternion.Euler(-90, 0, 0));
        // Player Dead Effect 생성 및 3초 뒤 제거
        Destroy(_deadEffect, 3f);

        float _rot = 0;
        while(_rot > -90f)
        {
            _rot = Mathf.Lerp(_rot, -91f, 0.1f);
            PlayerDeadModels[_num].transform.localEulerAngles = new Vector3(_rot, _deadRot.y, 0);
            yield return null;
        }

        // 죽는 오브젝트 1초 후에 원래 각도로 변경한 뒤, 비활성화 함
        yield return new WaitForSeconds(0.5f);
        PlayerDeadModels[_num].transform.position = new Vector3(-6, 0, -6);
        PlayerDeadModels[_num].transform.rotation = Quaternion.identity;
        PlayerDeadModels[_num].SetActive(false);
    }

    /* 눈덩이 관련 함수들 */
    public void GetSnowBall(int _player, Vector3 pos, Quaternion rot)
    {
        GameObject _ball = BallArray[_player, BallCylinder[_player].Dequeue()];
        _ball.transform.position = pos;
        _ball.transform.rotation = rot;
        _ball.SetActive(true);
        StartCoroutine(_ball.GetComponent<BallController>().ThrowingBall());
    }
    public void SetSnowBall(int _player, int _num)
    {
        GameObject _ball = BallArray[_player, _num];
        _ball.transform.position = BallGenPos;
        _ball.SetActive(false);
        BallCylinder[_player].Enqueue(_num);
    }

    public void GetAttackEffect(Vector3 pos, Vector3 rot)
    {
        StartCoroutine(ActiveAttackEffect(pos, rot));
    }

    private IEnumerator ActiveAttackEffect(Vector3 pos, Vector3 rot)
    {
        // 공 충돌 효과 생성
        GameObject _effect = EffectCylinder.Dequeue();
        _effect.SetActive(true);
        _effect.transform.position = pos;
        _effect.transform.rotation = Quaternion.LookRotation(rot);

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 공 충돌 효과 제거 및 EffectCylinder에 입력
        _effect.SetActive(false);
        _effect.transform.position = BallGenPos;
        EffectCylinder.Enqueue(_effect);
    }
}
