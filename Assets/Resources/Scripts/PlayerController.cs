using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Character의 RigidBody 객체
    [SerializeField]
    private Rigidbody Character;

    // Player Character의 Animator 객체
    [SerializeField]
    private Animator charAnim;

    // 카메라의 원래 위치 객체
    [SerializeField]
    private Transform originCameraPos;
    // Ray를 발사하는 위치 객체
    [SerializeField]
    private Transform rayPosition;

    // 걷기, 달리기, 현재 속도 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;
    // 걷는지 달리는지 판단하는 변수
    private bool isWalk;
    private bool isRun;
    // 캐릭터의 이전 위치 백터
    private Vector3 prePosition;

    // 카메라 민감도 변수
    [SerializeField]
    private float lookSensitivity;

    // 카메라 이동과 관련된 객체들
    [SerializeField]
    private Camera theCamera;
    private float currentRotationY;

    // 오브젝트 통과를 억제할 객체들
    private RaycastHit hitInfo;
    private Vector3 targetPos;
    private float rayDist;
    private Vector3 applyCameraPos;

    // Start is called before the first frame update
    void Start()
    {
        applySpeed = walkSpeed;
        rayDist = Mathf.Sqrt(originCameraPos.localPosition.y * originCameraPos.localPosition.y +
                                originCameraPos.localPosition.z * originCameraPos.localPosition.z);
    }

    // 물리적인 이동을 담당하는 Update 함수
    private void FixedUpdate()
    {
        TryRun();
        Move();
        setCharacterRotation();
        setCameraPosition();
    }

    // 캐릭터의 이동 함수
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        // 수평, 수직에 대한 이동 백터
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        // 수평, 수직으로 정의된 Character의 이동 velocity
        // 방향(수평, 수직에 대한 정규화) * 속도 = 캐릭터의 움직임
        Character.velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        // 캐릭터가 움직였는지 확인
        if (_moveDirZ != 0 || _moveDirX != 0)
            isWalk = true;
        else
            isWalk = false;

        // Character의 움직임을 Animation에 적용
        charAnim.SetBool("FrontWalk", isWalk);
        charAnim.SetFloat("SideWalk", _moveDirX);
    }

    // 캐릭터의 방향 전환 함수
    private void setCharacterRotation()
    {
        // 마우스 X축(좌, 우)에 대한 이동 값 반환
        float _yRotation = Input.GetAxisRaw("Mouse X");
        // 마우스 이동 값 * 마우스의 민감도 -> Character의 회전값 정의
        currentRotationY += _yRotation * lookSensitivity;
        // Character의 지역 Y 축에 대한 회전 값 정의 -> 캐릭터 방향 회전
        gameObject.transform.localEulerAngles = new Vector3(0, currentRotationY, 0);
    }

    // 카메라의 오브젝트 통과를 억제하는 함수
    private void setCameraPosition()
    {
        targetPos = transform.up * originCameraPos.localPosition.y +
                   transform.forward * originCameraPos.localPosition.z;
        Physics.Raycast(rayPosition.position, targetPos, out hitInfo, rayDist);
        Debug.DrawRay(rayPosition.position, targetPos, Color.green);
        // 카메라 Ray에 부딛히는 물체가 있을 경우
        if (hitInfo.point != Vector3.zero)
        {
            Debug.Log(hitInfo.transform.name);
            applyCameraPos = hitInfo.point;
            isnWall = false;
            StopAllCoroutines();
            StartCoroutine(moveGlobalCamera());
        }
        // 카메라 Ray에 부딛히는 물체가 없을 경우
        else
        {
            applyCameraPos = originCameraPos.position;
            isnWall = false;
            StopAllCoroutines();
            StartCoroutine(moveLocalCamera());
        }

            
    }

    private IEnumerator moveGlobalCamera()
    {
        if(theCamera.transform.position != applyCameraPos)
        {
            int cnt = 0;
            while (true)
            {
                theCamera.transform.position =
                    Vector3.Lerp(theCamera.transform.position, applyCameraPos, 0.1f);
                if (cnt > 15) break;
                else ++cnt;

                yield return null;
            }

            theCamera.transform.position =
                applyCameraPos;
        }
    }

    private IEnumerator moveLocalCamera()
    {
        if (theCamera.transform.localPosition != originCameraPos.localPosition)
        {
            int cnt = 0;
            while (true)
            {
                theCamera.transform.position =
                    Vector3.Lerp(theCamera.transform.position, applyCameraPos, 0.1f);
                if (cnt > 15) break;
                else ++cnt;

                yield return null;
            }

            theCamera.transform.localPosition = originCameraPos.localPosition;
            isnWall = true;
        }
    }

    // 달리기 시도 함수
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            Running();
        else
            stopRunning();
        charAnim.SetBool("Running", isRun);
    }

    // 달릴 때의 함수
    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    // 달리기를 멈췄을 때의 함수
    private void stopRunning()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }
}
