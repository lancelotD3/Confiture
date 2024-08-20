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

    [Header("Enemy laser")]
    public Color colorStartCharge = Color.white;
    public Color colorEndCharge = Color.red;

    PlayerEntity player;

    public LineRenderer lineRendererBeforeShoot;
    public LineRenderer lineRendererShoot;
    public GameObject particleStart;
    public GameObject particleEnd;

    [Header("Sounds")]
    public AudioClip shootClip;
    //public AudioClip warmUpClip;
    //public AudioClip criticalClip;

    protected override void Awake()
    {
        base.Awake();

        player = FindAnyObjectByType<PlayerEntity>();

        timerTriggered = timeBeforeDamage;
        timer = cooldownAfterShoot;

        lineRendererShoot.SetPosition(0, spawnTransform.position);
        lineRendererShoot.SetPosition(1, spawnTransform.position);
    }

    private void Update()
    {
        if (!player) return;

        Vector3 direction = (player.transform.position - spawnTransform.position).normalized;
        float distance = Vector3.Distance(player.transform.position, spawnTransform.position);


        Color color = Color.Lerp(colorStartCharge, colorEndCharge, 1 - timerTriggered / timeBeforeDamage);

        lineRendererBeforeShoot.startColor = color;
        lineRendererBeforeShoot.endColor = color;

        if (!Physics.Raycast(spawnTransform.position, direction, out RaycastHit hit, distance, obstruction))
        {
            canon.transform.LookAt(player.transform.position);

            lineRendererBeforeShoot.SetPosition(0, spawnTransform.position);
            lineRendererBeforeShoot.SetPosition(1, player.mesh.transform.position);

            if (!startShoot && canShoot)
            {
                startShoot = true;
            }
        }
        else
        {
            lineRendererBeforeShoot.SetPosition(0, spawnTransform.position);
            lineRendererBeforeShoot.SetPosition(1, spawnTransform.position);
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

        SetHitColor();

        particleStart.SetActive(true);
        particleEnd.SetActive(true);

        particleEnd.transform.parent = null;
        particleEnd.transform.position = player.mesh.transform.position;

        GameManager.instance.PlaySound(shootClip);

        player.RemoveBlobs(damage);
        
        Invoke(nameof(ResetColor), .2f);
    }

    private void SetHitColor()
    {
        //lineRendererBeforeShoot.startColor = Color.red;
        //lineRendererBeforeShoot.endColor = Color.red;

        lineRendererBeforeShoot.gameObject.SetActive(false);
        lineRendererShoot.gameObject.SetActive(true);

        lineRendererShoot.SetPosition(0, spawnTransform.position);
        lineRendererShoot.SetPosition(1, player.mesh.transform.position);
    }

    private void SetChargeColor()
    {
        lineRendererBeforeShoot.startColor = Color.red;
        lineRendererBeforeShoot.endColor = Color.red;
    }

    private void ResetColor()
    {
        lineRendererBeforeShoot.gameObject.SetActive(true);
        lineRendererShoot.gameObject.SetActive(false);

        lineRendererShoot.SetPosition(0, spawnTransform.position);
        lineRendererShoot.SetPosition(1, spawnTransform.position);
    }
}
