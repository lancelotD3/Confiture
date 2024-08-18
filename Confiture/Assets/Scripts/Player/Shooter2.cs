using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shooter2 : Enemy
{
    [Header("Enemy parameters")]
    public float timeBeforeDamage = 2f;

    public GameObject canon;
    public LayerMask obstruction;

    public Transform spawnTransform;
    public float cooldownAfterShoot = 1f;

    public int damage = 1;

    float timerTriggered;
    float timer;
    bool canShoot = true;
    bool startShoot = false;

    PlayerEntity player;

    LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();

        player = FindAnyObjectByType<PlayerEntity>();
        lineRenderer = GetComponent<LineRenderer>();

        timerTriggered = timeBeforeDamage;
        timer = cooldownAfterShoot;
    }

    private void Update()
    {
        if (!player) return;

        Vector3 direction = (player.transform.position - spawnTransform.position).normalized;
        float distance = Vector3.Distance(player.transform.position, spawnTransform.position);


        if (!Physics.Raycast(spawnTransform.position, direction, out RaycastHit hit, distance, obstruction))
        {
            canon.transform.LookAt(player.transform.position);

            lineRenderer.SetPosition(0, spawnTransform.position);
            lineRenderer.SetPosition(1, player.transform.position);

            if (!startShoot && canShoot)
            {
                startShoot = true;
            }
        }
        else
        {
            lineRenderer.SetPosition(0, spawnTransform.position);
            lineRenderer.SetPosition(1, spawnTransform.position);
            startShoot = false;
        }

        if(startShoot)
        {
            timerTriggered -= Time.deltaTime;
            if(timerTriggered < 0)
            {
                Shoot();
            }
        }
        else
        {
            timerTriggered = timeBeforeDamage;
        }

        if (!canShoot)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                canShoot = true;
                timer = cooldownAfterShoot;
            }
        }
    }

    private void Shoot()
    {
        canShoot = false;
        startShoot = false;
        timer = cooldownAfterShoot;

        player.RemoveBlobs(damage);
        SetHitColor();

        Invoke(nameof(ResetColor), .2f);
    }

    private void SetHitColor()
    {
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor= Color.red;
    }

    private void SetChargeColor()
    {
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    private void ResetColor()
    {
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }
}
