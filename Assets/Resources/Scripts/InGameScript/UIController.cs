using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject HealthUI;
    [SerializeField]
    private PlayerAttribute PlayerAtt;

    private float PrePlayerHealth;
    private Slider HealthBar;

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
