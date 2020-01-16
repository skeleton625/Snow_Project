using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallController : MonoBehaviour, IPunObservable
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
    private InGameObjectManager OManager;
    private AudioManager AManager;
    private int MasterPlayerNum;
    private int BallNumber;

    public void BallControllerInit(int _player, int _num, InGameObjectManager _OManager)
    {
        OManager = _OManager;
        BallNumber = _num;
        MasterPlayerNum = _player;
        AManager = GetComponent<AudioManager>();
    }

    // 공을 던지는 함수
    public IEnumerator ThrowingBall()
    {
        GetComponent<Rigidbody>().velocity = 
            gameObject.transform.forward * BallSpeed *Time.deltaTime;
        yield return new WaitForSeconds(AttackLimitTime);

        // 제한된 시간 뒤에 공의 이동을 종료하고 BallCylinder에 추가
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        OManager.SetSnowBall(MasterPlayerNum, BallNumber);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 같은 눈덩이 오브젝트의 경우 무시
        if (collision.gameObject.tag == "SnowBall")
            return;

        AManager.PlayAudioEffect(0, 1f);
        // 충돌한 위치에 충돌 효과 코루틴 실행
        Vector3 conflictPos = collision.contacts[0].point;
        Vector3 conflictRot = collision.contacts[0].normal;
        SendGetAttackEffect(conflictPos, conflictRot);
        // 공 이동 코루틴 종료
        StopCoroutine(ThrowingBall());
        // 충돌된 공에 대해 다시 BallCylinder에 입력
        OManager.SetSnowBall(MasterPlayerNum, BallNumber);
    }

    [PunRPC]
    private void SendGetAttackEffect(Vector3 pos, Vector3 rot)
    {
        OManager.GetAttackEffect(pos, rot);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
