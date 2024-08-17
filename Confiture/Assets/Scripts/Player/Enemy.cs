using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int life;
    public int maxLife;

    private void Awake()
    {
        life = 0;
    }

    public void AddDamage(int damage)
    {
        life += damage;

        if(life == maxLife)
        {
            Destroy(gameObject);
        }
    }
}
