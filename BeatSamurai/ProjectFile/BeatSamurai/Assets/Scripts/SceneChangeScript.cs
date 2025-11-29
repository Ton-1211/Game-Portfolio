using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeScene(string sceneName)
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1f;
        }
        SceneManager.LoadScene(sceneName);
    }
}
