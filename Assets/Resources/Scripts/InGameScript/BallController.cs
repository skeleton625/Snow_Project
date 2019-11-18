using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallController : MonoBehaviour
{
    // 공의 속성 변수
    [SerializeField]
    private float BallSpeed;
    [SerializeField]
    private float BallForce;
    // 1회 공격 가능 시간 및 공격하고 지난 시간 변수
    [SerializeField]
    private float AttackLimitTime;
    private bool IsActive;
    // 공 관리 객체
    [SerializeField]
    private InGameObjects Models;
    private PhotonView PlayerPv;

    public void BallControllerInit(InGameObjects _models)
    {
        Models = _models;
        PlayerPv = GetComponent<PhotonView>();
    }

    // 공을 던지는 함수
    public IEnumerator ThrowingBall()
    {
        // 제한된 시간 동안 공이 이동하도록 구현
        float _time = 0;
        while (_time < AttackLimitTime)
        {
            transform.Translate(Vector3.forward * BallSpeed * Time.deltaTime);
            _time += Time.deltaTime;
            yield return null;
        }

        string[] _nums = gameObject.name.Split('_');
        // 제한 시간이 다되면 움직임을 종료하고 탄창에 추가
        Models.SetSnowBall(int.Parse(_nums[0]), int.Parse(_nums[1]));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 같은 눈덩이 오브젝트의 경우 무시
        if (collision.gameObject.tag == "SnowBall")
            return;

        // 충돌한 위치에 충돌 효과 코루틴 실행
        Vector3 conflictPos = collision.contacts[0].point;
        Vector3 conflictRot = collision.contacts[0].normal;
        PlayerPv.RPC("SendGetAttackEffect", RpcTarget.All, conflictPos, conflictRot);

        // 충돌된 공에 대해 다시 BallCylinder에 입력
        string[] _nums = gameObject.name.Split('_');
        Models.SetSnowBall(int.Parse(_nums[0]), int.Parse(_nums[1]));
    }

    [PunRPC]
    private void SendGetAttackEffect(Vector3 pos, Vector3 rot)
    {
        Models.GetAttackEffect(pos, rot);
    }
}
