using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class InButtonManager : MonoBehaviour
{
    private PlayerManager PManager;

    // Start is called before the first frame update
    void Start()
    {
        PManager = GameObject.Find("StaticObjects").GetComponent<PlayerManager>();
    }

    public void ReturnToLobbyButtonClick()
    {
        PManager.PlayerOutTheGame(StaticObjects.MasterPlayerNum);
        SceneManager.LoadScene("MainMenu");
    }

    public void GameEndButtonClick()
    {
        Application.Quit();
    }
}
