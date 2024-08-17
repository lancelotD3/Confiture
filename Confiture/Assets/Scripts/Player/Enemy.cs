using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    int life;
    public int maxLife;

    public TextMeshProUGUI lifeText;

    private void Awake()
    {
        life = 0;
        lifeText.text = life + " / " + maxLife;
    }

    public void AddDamage(int damage)
    {
        life += damage;
        lifeText.text = life + " / " + maxLife;

        if (life == maxLife)
        {
            Destroy(gameObject);
        }
    }
}
