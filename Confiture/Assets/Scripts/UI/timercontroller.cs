using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class timercontroller : MonoBehaviour
{
    public float StartTime;
    private float TimeLeft;
    private bool TimerHasBegin = false;
    public TMP_Text TimerText;
    // Start is called before the first frame update
    void Start()
    {
        TimerText.text = StartTime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey&&!TimerHasBegin)
        {
            TimerHasBegin = true;
            TimeLeft = StartTime;
        }

        if (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            TimerText.text = TimeLeft.ToString("0.00");
        }
        else
        {
            Debug.Log("trop tard");
        }
    }
}
