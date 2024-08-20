using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;
    public bool opened = false;

    private void Start()
    {
        if(opened)
        {
            animator.Play("IdleOpen");
        }
    }

    public void UseDoor(bool open)
    {
        if (open)
            animator.Play("OpenDoor");
        else
            animator.Play("CloseDoor");
    }
}
