using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InGameObjects : MonoBehaviour, IPunObservable
{
    // 플레이어 오브젝트 관련 객체들
    [SerializeField]
    private Transform[] Beacons;
    [SerializeField]
    private GameObject[] Players;
    [SerializeField]
    private GameObject[] PlayerDeads;

    // 눈덩이 오브젝트 관련 객체들
    [SerializeField]
    private GameObject SnowBall;
    [SerializeField]
    private GameObject AttackEffect;
    [SerializeField]
    private int EachBallCount;
    private GameObject[,] BallArray;
    private Queue<int>[] BallCylinder;
    private Queue<GameObject> EffectCylinder;
    [SerializeField]
    private Vector3 BallGenPos;
    public PhotonView MasterPv;
    private int BallCount = 6;

    void Awake()
    {
        BallCylinder = new Queue<int>[4];
        for (int i = 0; i < 4; i++)
            BallCylinder[i] = new Queue<int>();

        BallArray = new GameObject[4, EachBallCount];
        MasterPv = GetComponent<PhotonView>();
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
            _ball.GetComponent<BallController>().BallControllerInit(this);
            _ball.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[_num]);
            _ball.GetComponent<PhotonView>().ViewID = BallCount;
            ++BallCount;

            // 비활성화 상태로 Queue에 추가
            _ball.SetActive(false);
            BallArray[_num, j] = _ball;
            BallCylinder[_num].Enqueue(j);
        }
    }

    // 플레이어 관련 함수들
    public Transform GetBeacons(int _num)
    {
        return Beacons[_num];
    }

    public GameObject GetPlayerModels(int _num)
    {
        return Players[_num];
    }

    public GameObject GetPlayerDeads(int _num)
    {
        return PlayerDeads[_num];
    }

    // 눈덩이 관련 함수들
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
        // 공 이동 코루틴 종료
        StopCoroutine(_ball.GetComponent<BallController>().ThrowingBall());
        _ball.SetActive(false);
        _ball.transform.position = BallGenPos;

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
