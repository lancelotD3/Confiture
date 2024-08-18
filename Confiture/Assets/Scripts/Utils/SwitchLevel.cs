using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchLevel : MonoBehaviour
{
    public string nextScene = string.Empty;
    public UnityEvent startLevel;

    private void Start()
    {
        GameManager.instance.FadeOut();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.SwitchScene(nextScene);
        }
    }
}
