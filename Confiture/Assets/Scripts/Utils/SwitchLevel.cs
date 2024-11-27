using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SwitchLevel : MonoBehaviour
{
    public string nextScene = string.Empty;
    public UnityEvent startLevel;
    public bool quitGame = false;
    private bool doOnce = false;

    private void Start()
    {
        if (quitGame)
            return;

        GameManagerNG.instance.FadeOut();

        if (InGameManager.instance.mode == InGameManager.E_ModeType.Train)
        {
            GameManagerNG.instance.inGameManager.StartLevel(InGameManager.instance.levelId);
        }
        else if (InGameManager.instance.mode == InGameManager.E_ModeType.Run)
        {
            GameManagerNG.instance.inGameManager.StartLevel(InGameManager.instance.levelId + 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !doOnce)
        {
            GameManagerNG.instance.SetLastChapterAndLevel();

            doOnce = true;
            if (quitGame)
                Application.Quit();
            else
            {
                if(InGameManager.instance.mode == InGameManager.E_ModeType.Train)
                {
                    InGameManager.instance.EndLevelTraining();
                    InGameManager.instance.ResetManagerStats();
                    //GameManagerNG.instance.SwitchLevel(SceneManager.GetActiveScene().name);
                }
                else if (InGameManager.instance.mode == InGameManager.E_ModeType.Run)
                {
                    GameManagerNG.instance.inGameManager.FinishLevel();
                    GameManagerNG.instance.SwitchLevel(nextScene);
                }
            }
        }
    }
}
