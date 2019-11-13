using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerModel;
    [SerializeField]
    private GameObject HealthUI;

    private float PrePlayerHealth;
    private Slider HealthBar;
    private PlayerAttribute PlayerAtt;

    public UIController()
    {
    }

    public UIController(GameObject _player, GameObject _healthUI)
    {
        HealthUI.SetActive(false);

        HealthUI = _healthUI;
        PlayerModel = _player;

        PlayerAtt = _player.GetComponent<PlayerAttribute>();
        HealthBar = _healthUI.GetComponent<Slider>();

        PrePlayerHealth = PlayerAtt.getHealthBar();
        HealthBar.value = PrePlayerHealth / PlayerAtt.getHealthBarMax();
    }

    public void SetPlayerHealthBar()
    {
        if(PlayerAtt.getHealthBar() != PrePlayerHealth)
        {
            PrePlayerHealth = PlayerAtt.getHealthBar();
            HealthBar.value = PrePlayerHealth / PlayerAtt.getHealthBarMax();
        }
    }
    public void VisibleHealthBar()
    {
        StopCoroutine(ActivateHealthBar());
        SetPlayerHealthBar();
        StartCoroutine(ActivateHealthBar());
    }

    private IEnumerator ActivateHealthBar()
    {
        HealthUI.SetActive(true);

        yield return new WaitForSeconds(3f);

        HealthUI.SetActive(false);

        yield return null;
    }
}
