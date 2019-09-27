using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    [SerializeField]
    private float PlayerHealthBar;
    [SerializeField]
    private float PlayerAttackDamage;
    [SerializeField]
    private int PlayerNumb;

    public void setHealthBar(float health)
    {
        PlayerHealthBar -= health;
    }

    public float getHealthBar()
    {
        return PlayerHealthBar;
    }

    public void setAttackDamage(float attack)
    {
        PlayerAttackDamage = attack;
    }

    public float getAttackDamage()
    {
        return PlayerAttackDamage;
    }

    public void setPlayerNumb(int num)
    {
        PlayerNumb = num;
    }

    public int getPlayerNumb()
    {
        return PlayerNumb;
    }
}
