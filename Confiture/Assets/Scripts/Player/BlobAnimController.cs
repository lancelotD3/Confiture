using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobAnimController : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = this.GetComponent <Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            animator.SetTrigger("Jump");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Hit");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("Shoot");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.SetTrigger("Dash");
        }
    }
}
