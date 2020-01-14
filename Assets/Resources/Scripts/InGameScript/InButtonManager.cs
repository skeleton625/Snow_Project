using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class InButtonManager : MonoBehaviour
{
    private PlayerManager PManager;

    private void Awake()
    {
        PManager = GameObject.Find("InGameObjectManager").GetComponent<PlayerManager>();
    }

    public void ReturnToLobbyButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PManager.PlayerOutTheGame(StaticObjects.MasterPlayerNum);
        SceneManager.LoadScene("MenuScene");
    }

    public void GameEndButtonClick()
    {
        Application.Quit();
    }
}
