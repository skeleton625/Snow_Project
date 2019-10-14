using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttribute : MonoBehaviourPunCallbacks, IPunObservable
{
    // 현재 Player의 체력
    [SerializeField]
    private float PlayerHealthBar;
    // 현재 Player의 공격력
    [SerializeField]
    private float PlayerAttackDamage;
    // 현재 Player의 번호
    [SerializeField]
    private int PlayerNumb;

    // Player의 최대 체력
    private float PlayerHealthBarMax;

    void Start()
    {
        PlayerHealthBarMax = PlayerHealthBar;
    }

    public void setHealthBar(float health)
    {
        PlayerHealthBar -= health;
    }

    public float getHealthBar()
    {
        return PlayerHealthBar;
    }

    public float getHealthBarMax()
    {
        return PlayerHealthBarMax;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
