using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEntity : MonoBehaviour
{
    public int blobNumber = 2;
    [HideInInspector] public float blobRatio = 0;

    [Header("Blob parameters")]
    [SerializeField] public int minBlob = 2;
    [SerializeField] int maxBlob = 10;
    [SerializeField] float minBlobSize;
    [SerializeField] float maxBlobSize;
    [Space]
    [SerializeField] bool winOneOnKill = true;
    public GameObject mesh;

    public PlayerShoot playerShoot;
    public PlayerMovement playerMovement;

    public GameObject splashPrefab;
    public GameObject splashPrefabDash;
    public GameObject splashPrefabDamage;
    public Transform feetPos;

    [SerializeField] public bool lockInput = false;

    [Header("Sounds")]
    public AudioClip diedClip;
    public AudioClip growthClip;
    public AudioClip eatEnemyClip;
    public AudioClip hitWallClip;

    private void Awake()
    {
        UpdateBlob();
        playerShoot = GetComponent<PlayerShoot>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V)) 
            AddBlobs(1);
    }

    public void AddBlobs(int number)
    {
        blobNumber += number;

        if(blobNumber > maxBlob) blobNumber = maxBlob;

        GameManager.instance.PlaySound(growthClip);

        UpdateBlob();
    }

    public bool TryRemoveBlobs(int number)
    {
        if(blobNumber - number < minBlobSize)
        {
            return false;
        }
        else
        {
            RemoveBlobs(number);
            return true;
        }
    }

    public void RemoveBlobs(int number)
    {
        blobNumber -= number;

        if (blobNumber <= 0)
            Died();

        UpdateBlob();
    }

    public void UpdateBlob()
    {
        blobRatio = (float)(blobNumber - 1) / (float)(maxBlob - 1);

        gameObject.transform.localScale = Vector3.Lerp(new Vector3(minBlobSize, minBlobSize, minBlobSize), new Vector3(maxBlobSize, maxBlobSize, maxBlobSize), blobRatio);

        GameObject splashGo = Instantiate(splashPrefabDamage, transform.position, Quaternion.identity);

        Destroy(splashGo, 2f);
    }

    private void Died()
    {
        GameManager.instance.PlayerDied();

        CameraShake.instance.TriggerShake(.15f, .3f, 1f);

        GameManager.instance.PlaySound(diedClip);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if(blobNumber > enemy.maxLife - enemy.life)
            {
                blobNumber = blobNumber - (enemy.maxLife - enemy.life);

                if (winOneOnKill)
                    blobNumber++;

                UpdateBlob();

                GameManager.instance.PlaySound(eatEnemyClip);

                enemy.AddDamage(99);
            }
            else
            {
                Died();
            }
        }
        else if(!playerMovement.isGrounded && !playerMovement.isDashing)
        {
            GameManager.instance.PlaySound(hitWallClip);
        }
    }

    public void LockInput(bool locked)
    {
        lockInput = locked;
        //playerShoot.enabled = !locked;
    }
}
