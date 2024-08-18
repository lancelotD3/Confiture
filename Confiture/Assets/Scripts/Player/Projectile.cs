using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerEntity>(out PlayerEntity player))
        {
            player.RemoveBlobs(damage);
        }

        Destroy(gameObject);
    }
}
