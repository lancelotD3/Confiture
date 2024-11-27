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

        float chrono = InGameManager.instance.gameTimer;

        chronoText.text = chrono.ToString();

        if (chrono > GameManagerNG.instance.timeForBronze)
        {
            chronoForNext.text = (chrono - GameManagerNG.instance.timeForBronze).ToString("0.00") + " seconds for bronze medal";
            animator.Play("NoMedal");
        }
        else if (chrono > GameManagerNG.instance.timeForSilver)
        {
            chronoForNext.text = (chrono - GameManagerNG.instance.timeForSilver).ToString("0.00") + " seconds for silver medal";
            animator.Play("Bronze");
        }
        else if (chrono > GameManagerNG.instance.timeForGold)
        {
            chronoForNext.text = (chrono - GameManagerNG.instance.timeForGold).ToString("0.00") + " seconds for gold medal";
            animator.Play("Silver");
        }
        else
        {
            chronoForNext.text = "Excellent time! Well played!";
            animator.Play("Gold");
        }

        InGameManager.instance.FinishChapter(chrono);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManagerNG.instance.lastScene = "EndGame";
            GameManagerNG.instance.canSwitch = true;
            SceneManager.LoadScene("MainMenuNG");

            //GameManagerNG.instance.SwitchScene("MainMenuNG");
        }
    }
}
