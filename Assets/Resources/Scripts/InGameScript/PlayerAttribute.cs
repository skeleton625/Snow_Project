using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    // 현재 Player의 번호
    [SerializeField]
    private int playerNumber;
    public int PlayerNumber
    {
        get{ return playerNumber; }
        set{ playerNumber = value; }
    }
    
    // 현재 Player의 공격력
    [SerializeField]
    private float playerDamage;
    public float PlayerDamage
    {
        get { return playerDamage; }
        set { playerDamage = value; }
    }
    // Player의 최대 체력
    private float playerHealthMax = 100;
    public float PlayerHealthMax
    {
        get { return playerHealthMax; }
        set { playerHealthMax = value; }
    }

    private float playerHealth;
    public float PlayerHealth
    {
        get { return playerHealth; }
        set {
            if (playerHealth <= 0)
                playerHealth = playerHealthMax;
            playerHealth -= value;
        }
    }
    private string playerName;
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }
}
