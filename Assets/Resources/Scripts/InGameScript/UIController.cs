using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerUI;

    private Slider HealthBar;
    private PhotonView pv;
    private PlayerAttribute PlayerAtt;
    private Transform TargetCamera;
    private float PrePlayerHealth;

    private GameObject DeadScene, ResumeScene;

    private bool MouseVisible;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerAtt = GetComponent<PlayerAttribute>();
        DeadScene = GameObject.Find("CharacterUI").transform.GetChild(0).gameObject;
        ResumeScene = GameObject.Find("CharacterUI").transform.GetChild(1).gameObject;
        InitCharacterUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine)
            HealthBar.transform.LookAt(TargetCamera);
        SetPlayerHealthBar();
        MouseLockInScene();
    }

    private void InitMouseVisible()
    {
        MouseVisible = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void InitCharacterUI()
    {
        TargetCamera = GameObject.Find("MainCamera").transform;
        if (pv.IsMine)
        {
            HealthBar = GameObject.Find("CharacterUI/MainHealthBar").GetComponent<Slider>();
            Destroy(PlayerUI);
        }
        else
            HealthBar = PlayerUI.transform.Find("HealthBar").GetComponent<Slider>();

        PrePlayerHealth = PlayerAtt.getHealthBar();
        HealthBar.value = PrePlayerHealth / PlayerAtt.getHealthBarMax();
        PlayerUI.SetActive(false);
    }

    private void SetPlayerHealthBar()
    {
        if(PlayerAtt.getHealthBar() != PrePlayerHealth)
        {
            PrePlayerHealth = PlayerAtt.getHealthBar();
            HealthBar.value = PrePlayerHealth / PlayerAtt.getHealthBarMax();
        }
    }

    private IEnumerator ActivateHealthBar()
    {
        PlayerUI.SetActive(true);

        yield return new WaitForSeconds(3f);

        PlayerUI.SetActive(false);

        yield return null;
    }

    private void MouseLockInScene()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MouseVisible = !MouseVisible;

            Cursor.visible = MouseVisible;
            if (MouseVisible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            ResumeScene.SetActive(MouseVisible);
        }
    }

    public void VisibleHealthBar()
    {
        StopCoroutine(ActivateHealthBar());
        StartCoroutine(ActivateHealthBar());
    }

    public void VisibleDeadScene()
    {
        DeadScene.SetActive(true);
    }
}
