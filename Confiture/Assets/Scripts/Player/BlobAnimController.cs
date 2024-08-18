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
    }
}
