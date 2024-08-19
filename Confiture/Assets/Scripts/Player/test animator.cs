using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testanimator : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetTrigger("Dash");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetTrigger("Shoot");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("Hit");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetBool("IsJumping", !animator.GetBool("IsJumping"));
        }

    }
}
