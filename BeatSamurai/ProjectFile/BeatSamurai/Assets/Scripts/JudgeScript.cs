using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeScript : MonoBehaviour
{
    [SerializeField] Vector2 jumpPower = new Vector2(0.5f, 2f);
    [SerializeField] float destoySecond;

    Rigidbody judgeRigidbody;
    SpriteRenderer spriteRenderer;
    PlayerBehaviorScript player;
    void Start()
    {
        judgeRigidbody = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<PlayerBehaviorScript>();
        StartCoroutine(JudgeBehavior());
    }

    IEnumerator JudgeBehavior()
    {
        jumpPower.x = player.transform.position.x < transform.position.x ? jumpPower.x : -jumpPower.x;
        judgeRigidbody.AddForce(new Vector3(jumpPower.x, jumpPower.y, 0f), ForceMode.Impulse);
        float timer = destoySecond;
        float fadeRate = 1f / destoySecond;
        while(timer > 0f)
        {
            Color color = spriteRenderer.color;
            color.a -= fadeRate * Time.deltaTime;
            spriteRenderer.color = color;
            timer -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
