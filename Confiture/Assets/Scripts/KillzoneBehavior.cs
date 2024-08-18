using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillzoneBehavior : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.GetComponent<PlayerEntity>() != null)
        {
            collision.gameObject.GetComponent<PlayerEntity>().RemoveBlobs(100);
        }
    }
}
