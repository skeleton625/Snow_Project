using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class InButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public void ReturnToLobbyButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
