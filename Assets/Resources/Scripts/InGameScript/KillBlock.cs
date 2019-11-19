using UnityEngine;
using UnityEngine.UI;

public class KillBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject KillerPlayer;
    [SerializeField]
    private GameObject KilledPlayer;

    public void SetPlayerNames(string _killedName, string _killerName)
    {
        KillerPlayer.GetComponent<Text>().text = _killedName;
        KilledPlayer.GetComponent<Text>().text = _killerName;
    }
}
