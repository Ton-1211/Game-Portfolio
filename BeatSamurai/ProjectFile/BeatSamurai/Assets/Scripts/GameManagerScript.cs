using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] int bpm;
    [SerializeField] PlayableDirector playableDirector;

    float bps;
    float soundTime;

    void Start()
    {
        bps = bpm / 60f;
        soundTime = 1 / bps;
        StartCoroutine(CountDown());
    }

    public void OnEndEvent()
    {
        Debug.Log("ゲーム終了!!!!");
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1);// カウントダウン前に少し待つ
        //float timer = soundTime;
        //for (int i = 0; i < 5; i++)
        //{
        //    while (timer > 0)
        //    {
        //        timer -= Time.deltaTime;
        //        yield return null;
        //    }
        //    if (i < 4)
        //    {
        //        Click();
        //    }
        //    timer = soundTime;
        //}
        playableDirector.Play();
    }

    void Click()
    {
        Debug.Log("Click!");
        // 音の再生をする予定
    }
}
