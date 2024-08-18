using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent onTrigger;
    public bool destroyAfterUse = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            onTrigger?.Invoke();
            
            if(destroyAfterUse)
                Destroy(gameObject);
        }
    }
}
