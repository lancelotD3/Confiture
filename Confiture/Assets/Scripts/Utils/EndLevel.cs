using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManagerNG.instance.musicAudioSource.Stop();
            GameManagerNG.instance.musicAudioSource.clip = GameManagerNG.instance.menuMusic;
            GameManagerNG.instance.musicAudioSource.Play();

            GameManagerNG.instance.SwitchScene("EndGame");
        }
    }
}
