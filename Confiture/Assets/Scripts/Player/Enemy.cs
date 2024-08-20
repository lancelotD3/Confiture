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

    public GameObject diedParticle;
    public Transform diedParticleSpawnPos;

    [Header("Sounds")]
    public AudioClip damageClip;
    public AudioClip diedClip;

    virtual protected void Awake()
    {
        life = 0;
        lifeText.text = life + " / " + maxLife;
    }

    public void AddDamage(int damage)
    {
        life += damage;
        lifeText.text = life + " / " + maxLife;

        if (life >= maxLife)
        {
            GameObject particleGo = Instantiate(diedParticle, diedParticleSpawnPos.position, Quaternion.identity);
            Destroy(particleGo, 5f);

            CameraShake.instance.TriggerShake(.2f, .3f, 1f);
            GameManager.instance.PlaySound(diedClip);

            GameManager.instance.RemoveEnemy();
            Destroy(gameObject);
        }
        else
        {
            GameManager.instance.PlaySound(damageClip);

            CameraShake.instance.TriggerShake(.15f, .15f, 1f);
        }
    }
}
