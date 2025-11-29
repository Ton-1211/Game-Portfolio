using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorScript : MonoBehaviour
{
    [Header("速度"), SerializeField] Vector2 absoluteSpeed = new Vector2(12f, 0f);
    [Header("輪郭を表示するまでのプレイヤーとの距離"), SerializeField] float outLineShowDistance = 2f;
    [Header("演出に関連付けられた敵"), SerializeField] List<ConnectEnemyBehaviorScript> connectedEnemiesToSpawn;
    [SerializeField] List<AudioClip> deadSounds = new List<AudioClip>();
    [SerializeField] List<PieceScript> pieceTypes;
    [SerializeField] EffectScript effectPrefab;
    protected PlayerBehaviorScript player;
    List<ConnectEnemyBehaviorScript> connectedEnemies = new List<ConnectEnemyBehaviorScript>();
    AudioSource audioSource;
    protected Animator animator;
    Vector2 speed;

    public List<ConnectEnemyBehaviorScript> ConnectedEnemiesToSpawn => connectedEnemiesToSpawn;
    public List<ConnectEnemyBehaviorScript> ConnectedEnemies => connectedEnemies;
    virtual protected void Start()
    {
        player = FindObjectOfType<PlayerBehaviorScript>();
        animator = GetComponent<Animator>();
        if(player.transform.position.x < transform.position.x)// 敵がプレイヤーの左右どちら側にいるか
        {
            SetDirection(true);
            SetSpeed(true);
        }
        else
        {
            SetDirection(false);
            SetSpeed(false);
        }
        //StartCoroutine(DieCount(limitTime));
    }

    virtual protected void FixedUpdate()
    {
        transform.Translate(speed.x * Time.deltaTime, speed.y * Time.deltaTime, 0f);// 移動
        if(Mathf.Pow(player.transform.position.x - transform.position.x, 2) <= outLineShowDistance * outLineShowDistance)
        {
            animator.SetBool("NearPlayer", true);
        }
    }

    void OnDestroy()
    {
        //if(connectedEnemies.Count > 0)
        //{
        //    for(int i = connectedEnemies.Count - 1; i >= 0; i--)
        //    {
        //        Destroy(connectedEnemies[i].gameObject);
        //    }
        //}
    }

    public void DestroyAfterEffect()
    {
        if(gameObject == null) return;
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }
        PlaySE(deadSounds);
        if(connectedEnemies.Count > 0)
        {
            for(int i = connectedEnemies.Count - 1; i >= 0; i--)
            {
                connectedEnemies[i].DestroyAfterEffect();
                connectedEnemies.Remove(connectedEnemies[i]);
            }
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// 破片の生成と判定結果による吹き飛ばし
    /// </summary>
    /// <param name="judgeType"></param>
    public void CreatePieces(JudgeType judgeType)
    {
        int counts = Random.Range(1, 3);
        List<int> spawnableType = new List<int>();
        if (player == null)
        {
            player = FindObjectOfType<PlayerBehaviorScript>();
        }
        for (int i = 0; i < pieceTypes.Count; i++)
        {
            spawnableType.Add(i);
        }
        for (int i = 0; i < counts; i++)
        {
            if (spawnableType.Count == 0) continue;// すべての破片が生成されているときはなにもしない

            int type = Random.Range(0, spawnableType.Count);
            PieceScript piece = Instantiate(pieceTypes[spawnableType[type]], transform.position, Quaternion.identity);
            piece.AddForceByJudge(judgeType, transform.position.x - player.transform.position.x >= 0f);
            spawnableType.Remove(type);
        }
    }

    public void AddConnectedEnemies(ConnectEnemyBehaviorScript connectedEnemy)
    {
        connectedEnemies.Add(connectedEnemy);
    }

    public void SetAudioSource(AudioSource source)
    {
        audioSource = source;
    }

    void PlaySE(List<AudioClip> clips)
    {
        foreach (AudioClip clip in clips)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    //IEnumerator DieCount(float lifeLimit)
    //{
    //    yield return new WaitForSeconds(lifeLimit);
    //    Destroy(gameObject);
    //}
    void SetDirection(bool spawnedRight)
    {
        if (spawnedRight) transform.localEulerAngles.Set(-1f, 1f, 1f);
        else transform.localEulerAngles.Set(1f, 1f, 1f);
    }
    void SetSpeed(bool spawnedRight)
    {
        if (spawnedRight) speed = -absoluteSpeed;
        else speed = absoluteSpeed;
    }
}
