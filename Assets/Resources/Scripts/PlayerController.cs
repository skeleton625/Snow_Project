using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Character;

    [SerializeField]
    private Animator charAnim;

    [SerializeField]
    private Camera theCamera;

    [SerializeField]
    private float walkSpeed;
    private Vector3 prePosition;

    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    private bool isGround;
    private bool isWalk;
    private bool isRun;
    private bool isSide;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float limitRotationX;
    private float currentRotationY;
    private float currentRotationX;

    // Start is called before the first frame update
    void Start()
    {
        applySpeed = walkSpeed;
        currentRotationX = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        SetIsGround();
        TryRun();
        TryJump();
        Move();
        setCharacterRotation();
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        Character.MovePosition(Character.position + _velocity * Time.deltaTime);
        if (prePosition == Character.position && _moveDirX == 0)
            isWalk = false;
        else
        {
            isWalk = true;
            prePosition = Character.position;
        }

        charAnim.SetBool("Walking", isWalk);
        charAnim.SetFloat("Right", _moveDirX);
    }

    private void setCharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        currentRotationY += _yRotation * lookSensitivity;
        currentRotationX -= _xRotation * lookSensitivity;
        currentRotationX = Mathf.Clamp(currentRotationX, -limitRotationX, limitRotationX);

        gameObject.transform.localEulerAngles = new Vector3(0, currentRotationY, 0);
        theCamera.transform.localEulerAngles = new Vector3(currentRotationX, 0, 0);
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            Running();
        else
            stopRunning();
        charAnim.SetBool("Running", isRun);
    }

    private void TryJump()
    {
        if(Input.GetKey(KeyCode.Z)&&isGround)
        {
            Jump();
        }
        charAnim.SetBool("Jumping", isGround);
    }

    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void stopRunning()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Jump()
    {
        Character.velocity = transform.up * jumpForce;
        isGround = false;
    }

    private void SetIsGround()
    {
        isGround = Physics.Raycast(Character.position, Vector3.down,0.1f);
    }
}
