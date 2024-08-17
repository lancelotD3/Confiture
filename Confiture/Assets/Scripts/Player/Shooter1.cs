using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter1 : Enemy
{
    [Header("Shooter infos")]
    public GameObject projectilePrefab;
    public GameObject canon;
    public LayerMask obstruction;

    public Transform spawnTransform;
    public float cooldown = 1f;
    public float projectileForce = 1f;

    float timer;
    bool canShoot = true;

    PlayerEntity player;

    protected override void Awake()
    {
        base.Awake();

        player = FindAnyObjectByType<PlayerEntity>();

        timer = cooldown;
    }

    private void Update()
    {
        //RaycastHit hit;
        if (!player) return;

        Vector3 direction = (player.transform.position - spawnTransform.position).normalized;
        float distance = Vector3.Distance(player.transform.position, spawnTransform.position);


        if (Physics.Raycast(spawnTransform.position, direction, out RaycastHit hit, distance, obstruction))
        {
            if(hit.collider.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
            {
                canon.transform.LookAt(player.transform.position);

                if (canShoot)
                {
                    canShoot = false;
                    Shoot(direction * projectileForce);
                }
            }
        }
        else
        {
            canon.transform.LookAt(player.transform.position);

            if (canShoot)
            {
                canShoot = false;
                Shoot(direction * projectileForce);
            }
        }

        if(!canShoot)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                canShoot=true;
                timer = cooldown;
            }
        }
    }

    private void Shoot(Vector3 force)
    {
        GameObject projectileGo = Instantiate(projectilePrefab, spawnTransform.position, Quaternion.identity);
        projectileGo.GetComponent<Rigidbody>().velocity = force;
    }
}
