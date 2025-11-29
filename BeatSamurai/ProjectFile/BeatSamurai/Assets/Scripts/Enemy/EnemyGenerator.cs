using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [Tooltip("生成する敵"),SerializeField] List<EnemyBehaviorScript> enemies;
    [Tooltip("右の生成位置"),SerializeField] Vector3 spawnPointRight = new Vector3(13f, 0f, 7.5f);
    [Tooltip("左の生成位置"),SerializeField] Vector3 spawnPointLeft = new Vector3(-13f, 0f, 7.5f);
    [SerializeField] AudioSource audioSource;

    void Start()
    {

    }

    /// <summary>
    /// 敵を生成する（右から）
    /// </summary>
    public void SpawnEnemyRight()
    {
        EnemyBehaviorScript nodeEnemy = SpawnEnemy(spawnPointRight);
        if(nodeEnemy.ConnectedEnemiesToSpawn != null)
        {
            SpawnConnectedEnemy(nodeEnemy, spawnPointRight);
        }
    }

    /// <summary>
    /// 敵を生成する（左から）
    /// </summary>
    public void SpawnEnemyLeft()
    {
        EnemyBehaviorScript nodeEnemy = SpawnEnemy(spawnPointLeft);
        if (nodeEnemy.ConnectedEnemiesToSpawn != null)
        {
            SpawnConnectedEnemy(nodeEnemy, spawnPointLeft);
        }
    }

    EnemyBehaviorScript SpawnEnemy(Vector3 spawnPoint)
    {
        EnemyBehaviorScript enemy = Instantiate(enemies[Random.Range(0, enemies.Count)], transform.position /*+ spawnPoint*/, Quaternion.identity, transform);
        enemy.transform.localPosition = spawnPoint;
        enemy.SetAudioSource(audioSource);
        return enemy;
    }

    void SpawnConnectedEnemy(EnemyBehaviorScript nodeEnemy, Vector3 spawnPoint)
    {
        foreach (ConnectEnemyBehaviorScript connectedEnemy in nodeEnemy.ConnectedEnemiesToSpawn)
        {
            ConnectEnemyBehaviorScript enemy = Instantiate(connectedEnemy, transform.position /*+ spawnPoint*/, Quaternion.identity, transform);
            enemy.transform.localPosition = spawnPoint;
            enemy.SetAudioSource(audioSource);
            nodeEnemy.AddConnectedEnemies(enemy);
        }
    }
}
