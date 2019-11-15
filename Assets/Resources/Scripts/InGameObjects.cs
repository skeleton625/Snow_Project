using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InGameObjects : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        BallArray = new GameObject[4, EachBallCount];
        MasterPv = GetComponent<PhotonView>();
        GenAttackSnowBall(EachBallCount);
    }

    private void GenAttackSnowBall(int _cnt)
    {
        int _ballNum = 6;
        BallCylinder = new Queue<int>[4];
        EffectCylinder = new Queue<GameObject>();
        for (int i = 0; i < 4; i++)
            BallCylinder[i] = new Queue<int>();

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            for (int j = 0; j < _cnt; j++)
            {
                // 필요한 공 및 효과 오브젝트 생성
                GameObject _ball = Instantiate(SnowBall, BallGenPos, Quaternion.identity);
                GameObject _effect = Instantiate(AttackEffect, BallGenPos, Quaternion.identity);
                _ball.name = i + "_" + j;
                _ball.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
                _ball.GetComponent<PhotonView>().ViewID = _ballNum++;
                _ball.GetComponent<BallController>(this);

                // 비활성화 상태로 Queue에 추가
                _ball.SetActive(false);
                _effect.SetActive(false);
                BallArray[i, j] = _ball;
                BallCylinder[i].Enqueue(j);
                EffectCylinder.Enqueue(_effect);
            }
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
}
