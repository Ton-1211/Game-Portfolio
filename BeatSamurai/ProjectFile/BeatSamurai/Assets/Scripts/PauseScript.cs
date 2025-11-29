using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class PauseScript : MonoBehaviour
{
    [SerializeField] Button firstSelectButton;
    [SerializeField] PlayableDirector director;

    void Start()
    {
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        director.Pause();
        firstSelectButton.Select();// 選択状態にする
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        director.Resume();
        gameObject.SetActive(false);
    }
}
