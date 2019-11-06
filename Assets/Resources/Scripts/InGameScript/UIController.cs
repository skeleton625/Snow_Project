using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerUI;
    [SerializeField]
    private float MaxTime;

    private Slider HealthBar;
    private PhotonView pv;
    private PlayerAttribute PlayerAtt;
    private Transform TargetCamera;
    private float PrePlayerHealth;

    private bool IsTimerStart;
    private float CurrTime;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerAtt = GetComponent<PlayerAttribute>();
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

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine)
            HealthBar.transform.LookAt(TargetCamera);
        SetPlayerHealthBar();
        ActivateHealthBar();
    }

    private void SetPlayerHealthBar()
    {
        if(PlayerAtt.getHealthBar() != PrePlayerHealth)
        {
            PrePlayerHealth = PlayerAtt.getHealthBar();
            HealthBar.value = PrePlayerHealth / PlayerAtt.getHealthBarMax();
        }
    }

    private void ActivateHealthBar()
    {
        if(IsTimerStart)
        {
            if (CurrTime < MaxTime)
                CurrTime += Time.deltaTime;
            else
            {
                IsTimerStart = false;
                PlayerUI.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "SnowBall(Clone)")
        {
            if(!pv.IsMine)
            {
                IsTimerStart = true;
                CurrTime = 0;
                PlayerUI.SetActive(true);
            }
        }
    }
}
