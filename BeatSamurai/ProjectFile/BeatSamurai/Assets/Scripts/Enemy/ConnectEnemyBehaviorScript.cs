using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectEnemyBehaviorScript : EnemyBehaviorScript
{
    [SerializeField] List<float> spawnPointsZ;
    [SerializeField] float maxLifeTime = 5f;

    float timer;
    // Start is called before the first frame update
    protected override void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, spawnPointsZ[Random.Range(0, spawnPointsZ.Count)]);
        base.Start();

        animator.SetBool("Up", transform.position.z - player.transform.position.z >= 0f);
        timer = 0f;
    }

    protected override void FixedUpdate()
    {
        if(timer < maxLifeTime)
        {
            timer += Time.deltaTime;
            if(timer >= maxLifeTime)
            {
                Destroy(gameObject);
            }
        }
        base.FixedUpdate();
    }
}
