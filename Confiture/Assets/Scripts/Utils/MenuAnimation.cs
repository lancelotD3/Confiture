using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    public void OnFadeComplete()
    {
        GameManager.instance.OnFadeComplete();
    }
}