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

        GameManager.instance.FadeOut();
        GameManager.instance.StartLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (quitGame)
                Application.Quit();
            else
                GameManager.instance.SwitchScene(nextScene);
        }
    }
}
