using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.musicAudioSource.Stop();
            GameManager.instance.musicAudioSource.clip = GameManager.instance.menuMusic;
            GameManager.instance.musicAudioSource.Play();

            GameManager.instance.SwitchScene("EndGame");
        }
    }
}
