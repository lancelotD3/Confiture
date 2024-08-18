using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public int life;
    public int maxLife;

    public TextMeshProUGUI lifeText;

    virtual protected void Awake()
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
            GameManager.instance.RemoveEnemy();
            Destroy(gameObject);
        }
    }
}
