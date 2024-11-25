using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public string sceneToLoad = string.Empty;

    private void Start()
    {
        GameManagerNG.instance.FadeOut();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.anyKeyDown)
        {
            GameManagerNG.instance.SwitchScene(sceneToLoad);
        }
    }
}
