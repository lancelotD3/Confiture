using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    Animator animator;

    public TextMeshProUGUI chronoText;
    public TextMeshProUGUI chronoForNext;

    private void Start()
    {
        animator = GetComponent<Animator>();

        float chrono = GameManager.instance.gameTimer;
        chronoText.text = chrono.ToString();

        foreach(GameObject go in GameManager.instance.canvas)
        {
            go.SetActive(false);
        }

        if (chrono > GameManager.instance.timeForBronze)
        {
            chronoForNext.text = GameManager.instance.timeForBronze.ToString() + "seconds for bronze medal";
            animator.Play("NoMedal");
        }
        else if (chrono > GameManager.instance.timeForSilver)
        {
            chronoForNext.text = GameManager.instance.timeForSilver.ToString() + "seconds for silver medal";
            animator.Play("Bronze");
        }
        else if (chrono > GameManager.instance.timeForGold)
        {
            chronoForNext.text = GameManager.instance.timeForGold.ToString() + "seconds for gold medal";
            animator.Play("Silver");
        }
        else
        {
            chronoForNext.text = "Excellent time! Well played!";
            animator.Play("Gold");
        }

        chronoText.text = GameManager.instance.gameTimer.ToString("0.00");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MainMenu");
            GameManager.instance.ResetManagerStats();
        }
    }
}
