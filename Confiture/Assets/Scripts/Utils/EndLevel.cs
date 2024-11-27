using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    bool doOnce = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !doOnce)
        {
            doOnce = true;

            GameManagerNG.instance.SetLastChapterAndLevel();

            if (InGameManager.instance.mode == InGameManager.E_ModeType.Train)
            {
                InGameManager.instance.EndLevelTraining();
                InGameManager.instance.ResetManagerStats();
            }
            else if (InGameManager.instance.mode == InGameManager.E_ModeType.Run)
            {
                GameManagerNG.instance.musicAudioSource.Stop();
                GameManagerNG.instance.musicAudioSource.clip = GameManagerNG.instance.menuMusic;
                GameManagerNG.instance.musicAudioSource.Play();

                GameManagerNG.instance.SwitchScene("EndGame");
            }
        }
    }
}
