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
    [SerializeField]
    private float playerHealthMax;
    public float PlayerHealthMax
    {
        get { return playerHealthMax; }
    }

    private float playerHealth;
    public float PlayerHealth
    {
        get { return playerHealth; }
        set
        {
            if(playerHealth > 0)
                playerHealth -= value;
        }
    }

    void Start()
    {
        playerHealth = playerHealthMax;    
    }
}
