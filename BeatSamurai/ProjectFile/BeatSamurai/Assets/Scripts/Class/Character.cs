using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] int maxHp;

    int hp;

    public int MaxHp
    {
        get { return maxHp; }
    }
    public int HP
    {
        get { return hp; }
    }
    public bool IsDead
    {
        get { return hp <= 0; }
    }

    void Start()
    {
        SetHpMax();
    }

    public void SetHpMax()
    {
        hp = maxHp;
    }

    public void Hurt(int damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            hp = 0;
        }
    }
}
