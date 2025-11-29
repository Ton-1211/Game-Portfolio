using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideUIScript : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [Header("残っている時間"), SerializeField] float stayTime = 1f;
    [Header("フェードアウトにかかる時間"), SerializeField] float fadeOutTime = 0.5f;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(timer < stayTime + fadeOutTime)
        {
            if(timer >= stayTime)
            {
                canvasGroup.alpha = (fadeOutTime - timer + stayTime) / fadeOutTime;
            }
            timer += Time.deltaTime;

            if(timer >= stayTime + fadeOutTime)
            {
                canvasGroup.alpha = 0f;
            }
        }
    }
}
