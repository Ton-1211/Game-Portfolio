using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstance : MonoBehaviour
{
    enum Side
    {
        right = 0,
        left = 1
    }

    [SerializeField] EnemyGenerator enemyGenerator;

    float cooldown = 0.6f;
    //float cooldown = 0.2f;
    float rightTimer;
    float leftTimer;

    void Start()
    {
        rightTimer = 0f;
        leftTimer = 0f;
    }

    void FixedUpdate()
    {
        if (rightTimer > 0)
        {
            rightTimer -= Time.deltaTime;
        }
        if (leftTimer > 0)
        {
            leftTimer -= Time.deltaTime;
        }
    }
    public void NodeEventRight()
    {
        if (rightTimer <= 0)
        {
            enemyGenerator.SpawnEnemyRight();
            rightTimer = cooldown;
        }
    }

    public void NodeEventLeft()
    {
        if (leftTimer <= 0)
        {
            enemyGenerator.SpawnEnemyLeft();
            leftTimer = cooldown * 2;
        }
    }

    public void NodeEventRandom()
    {
        Side side = Random.Range(0, 3) < 1 ? Side.left : Side.right;
        if(side == Side.right)
        {
            if (rightTimer <= 0)
            {
                NodeEventRight();
            }
            else
            {
                NodeEventLeft();
            }
        }
        else
        {
            if (leftTimer <= 0)
            {
                NodeEventLeft();
            }
            else
            {
                NodeEventRight();
            }
        }
    }
}
