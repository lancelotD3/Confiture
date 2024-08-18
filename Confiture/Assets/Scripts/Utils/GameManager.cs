using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Animator fadeAnimator;
    static public GameManager instance;

    private string nextScene;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
     
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SwitchScene(SceneManager.GetActiveScene().name);
    }

    public void SwitchScene(string sceneName)
    {
        nextScene = sceneName;
        FadeIn();
    }

    public void FadeIn()
    {
        fadeAnimator.Play("FadeIn");
    }

    public void FadeOut()
    {
        fadeAnimator.Play("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(nextScene);
        nextScene = string.Empty;
    }
}
