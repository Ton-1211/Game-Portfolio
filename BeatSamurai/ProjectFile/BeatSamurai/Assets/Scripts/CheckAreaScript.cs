using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAreaScript : MonoBehaviour
{
    [SerializeField] List<EnemyBehaviorScript> hitableEnemies = new List<EnemyBehaviorScript>();
    [SerializeField] JudgeScript greatPrefab;
    [SerializeField] JudgeScript goodPrefab;
    [SerializeField] JudgeScript badPrefab;

    ScoreScript scoreScript;

    void Start()
    {
        scoreScript = FindObjectOfType<ScoreScript>();
    }

    void Update()
    {
        for (int i = hitableEnemies.Count - 1; i >= 0; i--)
        {
            if (hitableEnemies[i] == null)
            {
                hitableEnemies.Remove(hitableEnemies[i]);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hurtable")
        {
            hitableEnemies.Add(other.GetComponent<EnemyBehaviorScript>());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hurtable")
        {
            hitableEnemies.Remove(other.GetComponent<EnemyBehaviorScript>());
        }
    }

    public void Hit()
    {
        if (hitableEnemies.Count <= 0)
        {
            return;
        }
        foreach (var hit in hitableEnemies)
        {
            if (hit != null)
            {
                HitJudge(hit);
                hit.DestroyAfterEffect();
            }
        }
        hitableEnemies.Clear();
    }

    void HitJudge(EnemyBehaviorScript hit)
    {
        if (hit != null && hit.CompareTag("Hurtable"))
        {
            float distance = Mathf.Abs(hit.transform.position.x - transform.position.x);// 判定用の距離
            JudgeType judge;
            if (distance <= 0.5f)
            {
                Instantiate(greatPrefab, transform);
                judge = JudgeType.great;
            }
            else if (distance <= 0.8f)
            {
                Instantiate(goodPrefab, transform);
                judge = JudgeType.good;
            }
            else
            {
                Instantiate(badPrefab, transform);
                judge = JudgeType.bad;
            }
            CreateEnemyPieces(hit, judge);
            scoreScript.AddCombo();
        }
    }

    void CreateEnemyPieces(EnemyBehaviorScript nodeEnemy, JudgeType judgeType)
    {
        nodeEnemy.CreatePieces(judgeType);
        foreach (EnemyBehaviorScript connectedEnemy in nodeEnemy.ConnectedEnemies)// 関連付けられた敵の破片生成
        {
            connectedEnemy.CreatePieces(judgeType);
        }
    }
}
