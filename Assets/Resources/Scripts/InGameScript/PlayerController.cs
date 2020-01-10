using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerController : MonoBehaviour, IPunObservable
{
    // 카메라의 원래 위치 객체
    [SerializeField]
    private Transform OiriginCameraPos;
    // Ray를 발사하는 위치 객체
    [SerializeField]
    private Transform RayPosition;
    // 걷고 있는 속도 변수
    [SerializeField]
    private float WalkSpeed;
    // 달리는 속도 변수
    [SerializeField]
    private float RunSpeed;
    // 카메라 민감도 변수
    [SerializeField]
    private float LookSensitivity;

    // Player Character의 RigidBody 객체
    private Rigidbody Character;
    // Player Character의 Animator 객체
    private Animator CharAnimator;
    // 현재 달리기 속도 변수
    private float ApplySpeed;
    // 카메라 이동과 관련된 객체들
    private float CurrentRotationY;

    // 걷고 있는 여부를 확인하는 변수
    private bool IsWalk;
    // 달리고 있는 여부를 확인하는 변수
    private bool IsRun;
    // 좌우 이동에 대한 변수
    private float SideWalk;
    private float FrontWalk;
    
    // 오브젝트 통과를 억제할 객체들
    private RaycastHit HitInfo;
    private Vector3 TargetPos;
    private Vector3 ApplyCameraPos;
    private float RayDist;

    // 상대방 플레이어에 대한 위치, 회전 변수
    private Vector3 CurrPos;
    private Quaternion CurrRot;

    // Player가 현재 클라이언트에서 Master인지 확인
    private PhotonView PlayerPv;
    // Player Camera 오브젝트
    private Camera PlayerCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name == StaticObjects.MasterPlayerNum + "")
            PlayerCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        PlayerPv = GetComponent<PhotonView>();
        Character = GetComponent<Rigidbody>();
        CharAnimator = GetComponent<Animator>();
        RayDist = Mathf.Sqrt(OiriginCameraPos.localPosition.y * OiriginCameraPos.localPosition.y +
                                OiriginCameraPos.localPosition.z * OiriginCameraPos.localPosition.z);
        InitPlayerController();
    }

    // 물리적인 이동을 담당하는 Update 함수
    private void FixedUpdate()
    {
        if(PlayerPv.IsMine)
        {
            TryRun();
            Move();
            setCharacterRotation();
            setCameraPosition();
        }
        else
        {
            Character.position = Vector3.Lerp(Character.position, CurrPos, Time.deltaTime * 10.0f);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, CurrRot, Time.deltaTime * 10.0f);
        }
        // Character의 움직임을 Animation에 적용
        CharAnimator.SetBool("Running", IsRun);
        CharAnimator.SetBool("FrontWalk", IsWalk);
        CharAnimator.SetFloat("SideWalk", SideWalk);
    }

    public void InitPlayerController()
    {
        ApplySpeed = WalkSpeed;
        CurrPos = gameObject.transform.position;

        if (GetComponent<PlayerAttribute>().PlayerNumber % 2 == 1)
            CurrentRotationY = 180;
        else
            CurrentRotationY = 0;
        transform.rotation = Quaternion.Euler(0, CurrentRotationY, 0);

        if (gameObject.name == StaticObjects.MasterPlayerNum + "")
        {
            Debug.Log(PlayerCamera);
            PlayerCamera.transform.rotation = Quaternion.Euler(10, CurrentRotationY, 0);
            PlayerCamera.transform.parent = gameObject.transform;
        }
    }

    // 캐릭터의 이동 함수
    private void Move()
    {
        SideWalk = Input.GetAxisRaw("Horizontal");
        FrontWalk = Input.GetAxisRaw("Vertical");
        
        // 수평, 수직에 대한 이동 백터
        Vector3 _moveHorizontal = transform.right * SideWalk;
        Vector3 _moveVertical = transform.forward * FrontWalk;
        // 수평, 수직으로 정의된 Character의 이동 velocity
        // 방향(수평, 수직에 대한 정규화) * 속도 = 캐릭터의 움직임
        Character.velocity = (_moveHorizontal + _moveVertical).normalized * ApplySpeed;

        // 캐릭터가 움직였는지 확인
        if (FrontWalk != 0 || SideWalk != 0)
            IsWalk = true;
        else
            IsWalk = false;
    }

    // 달리기 시도 함수
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && FrontWalk >= 0)
            Running();
        else
            stopRunning();
    }

    // 달릴 때의 함수
    private void Running()
    {
        IsRun = true;
        ApplySpeed = RunSpeed;
    }

    // 달리기를 멈췄을 때의 함수
    private void stopRunning()
    {
        IsRun = false;
        ApplySpeed = WalkSpeed;
    }

    // 캐릭터의 방향 전환 함수
    private void setCharacterRotation()
    {
        // 마우스 X축(좌, 우)에 대한 이동 값 반환
        float _yRotation = Input.GetAxisRaw("Mouse X");
        // 마우스 이동 값 * 마우스의 민감도 -> Character의 회전값 정의
        CurrentRotationY += _yRotation * LookSensitivity;
        // Character의 지역 Y 축에 대한 회전 값 정의 -> 캐릭터 방향 회전
        gameObject.transform.localEulerAngles = new Vector3(0, CurrentRotationY, 0);
    }

    // 카메라의 오브젝트 통과를 억제하는 함수
    private void setCameraPosition()
    {
        TargetPos = transform.up * OiriginCameraPos.localPosition.y +
                   transform.forward * OiriginCameraPos.localPosition.z;
        Physics.Raycast(RayPosition.position, TargetPos, out HitInfo, RayDist);
        // 카메라 Ray에 부딛히는 물체가 있을 경우
        if (HitInfo.point != Vector3.zero)
        {
            ApplyCameraPos = HitInfo.point;
            StopAllCoroutines();
            StartCoroutine(moveGlobalCamera());
        }
        // 카메라 Ray에 부딛히는 물체가 없을 경우
        else
        {
            ApplyCameraPos = OiriginCameraPos.position;
            StopAllCoroutines();
            StartCoroutine(moveLocalCamera());
        }
    }

    private IEnumerator moveGlobalCamera()
    {
        if(PlayerCamera.transform.position != ApplyCameraPos)
        {
            int cnt = 0;
            while (true)
            {
                PlayerCamera.transform.position =
                    Vector3.Lerp(PlayerCamera.transform.position, ApplyCameraPos, 0.1f);
                if (cnt > 15) break;
                else ++cnt;

                yield return null;
            }
            PlayerCamera.transform.position = ApplyCameraPos;
        }
    }

    private IEnumerator moveLocalCamera()
    {
        if (PlayerCamera.transform.localPosition != OiriginCameraPos.localPosition)
        {
            int cnt = 0;
            while (true)
            {
                PlayerCamera.transform.position =
                    Vector3.Lerp(PlayerCamera.transform.position, ApplyCameraPos, 0.1f);
                if (cnt > 15) break;
                else ++cnt;

                yield return null;
            }

            PlayerCamera.transform.localPosition = OiriginCameraPos.localPosition;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(IsRun);
            stream.SendNext(IsWalk);
            stream.SendNext(SideWalk);
        }
        else
        {
            CurrPos = (Vector3)stream.ReceiveNext();
            CurrRot = (Quaternion)stream.ReceiveNext();
            IsRun = (bool)stream.ReceiveNext();
            IsWalk = (bool)stream.ReceiveNext();
            SideWalk = (float)stream.ReceiveNext();
        }
    }
}
