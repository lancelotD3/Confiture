using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchLevel : MonoBehaviour
{
    public string nextScene = string.Empty;
    public UnityEvent startLevel;
    public bool quitGame = false;

    private void Start()
    {
        if (quitGame)
            return;

        GameManagerNG.instance.FadeOut();
        GameManagerNG.instance.inGameManager.StartLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (quitGame)
                Application.Quit();
            else
                GameManagerNG.instance.SwitchScene(nextScene);
        }
    }
}
