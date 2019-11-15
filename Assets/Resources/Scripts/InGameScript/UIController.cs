using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private float ActivateFullTime;
    [SerializeField]
    private GameObject HealthUI;
    [SerializeField]
    private PlayerAttribute PlayerAtt;

    private bool IsActivate;
    private Slider HealthBar;
    private float PrePlayerHealth;
    private float ActivateTime;


    void Start()
    {
        HealthBar = HealthUI.GetComponent<Slider>();    
    }

    public UIController(GameObject _player, GameObject _healthUI)
    {
        HealthUI = _healthUI;
        PlayerAtt = _player.GetComponent<PlayerAttribute>();
        HealthBar = _healthUI.GetComponent<Slider>();

        PrePlayerHealth = PlayerAtt.PlayerHealth;
        HealthBar.value = PrePlayerHealth / PlayerAtt.PlayerHealthMax;
    }

    public void SetPlayerHealthBar()
    {
        if(PlayerAtt.PlayerHealth != PrePlayerHealth)
        {
            PrePlayerHealth = PlayerAtt.PlayerHealth;
            HealthBar.value = PrePlayerHealth / PlayerAtt.PlayerHealthMax;
        }
    }
    public void VisibleHealthBar()
    {
        ActivateTime = 0;
        if (!IsActivate)
            StartCoroutine(ActivateHealthBar());
    }

    private IEnumerator ActivateHealthBar()
    {
        IsActivate = true;
        HealthUI.SetActive(true);
        while(ActivateTime < ActivateFullTime)
        {
            ActivateTime += Time.deltaTime;
            yield return null;
        }

        IsActivate = false;
        HealthUI.SetActive(false);
        yield return null;
    }
}
