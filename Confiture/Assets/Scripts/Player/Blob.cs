using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Blob : MonoBehaviour
{
    [Header("Blob parameters")]

    [SerializeField] GameObject blobPrefab;
    [SerializeField] LayerMask blobMask;

    [SerializeField] int blobNumber = 1;
    [SerializeField] float sizePerBlob = .5f;
    [SerializeField] float minSize = 0.5f;


    [HideInInspector] public bool allreadySpawn = false;

    [SerializeField] private bool collisionOn = false;
    [HideInInspector] public Rigidbody rb;

    public bool dashable = false;

    [SerializeField] float timeForCollisionAfterShooted = .5f;

    [Header("Splash parameters")]
    public List<Material> decalsMats = new List<Material>();
    public GameObject decalPrefab;
    public GameObject splashPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        UpdateBlob();
    }

    private void Update()
    {
        if (!collisionOn) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2, blobMask);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            if (collider.gameObject.TryGetComponent<PlayerEntity>(out PlayerEntity player))
            {
                if(player.playerMovement.isDashing)
                {
                    player.playerMovement.isDashing = false;

                    if (!player.playerMovement.isJumping)
                    {
                        player.playerMovement.dashFastFallTime = 0f;
                        player.playerMovement.dashFastFallReleaseSpeed = player.playerMovement.verticalVelocity;

                        //if(!isGrounded)
                        //{
                        //    isDashFastFalling = true;
                        //}
                    }

                    player.LockInput(false);
                }

                player.AddBlobs(blobNumber);
                Destroy(gameObject);

                return;
            }
            else if (collider.gameObject.TryGetComponent<Blob>(out Blob blob))
            {
                if (allreadySpawn)
                    return;

                GameObject blobMergedGo = Instantiate(blobPrefab, Vector3.Lerp(transform.position, blob.transform.position, 0.5f), Quaternion.identity);
                Blob blobMerged = blobMergedGo.GetComponent<Blob>();

                blobMerged.blobNumber = blobNumber + blob.blobNumber;
                blobMerged.UpdateBlob();

                allreadySpawn = true;
                blob.allreadySpawn = true;

                Destroy(gameObject);
                Destroy(blob.gameObject);
                return;
            }
            else if (collider.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.AddDamage(1);
                gameObject.transform.parent = enemy.transform;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;

                collisionOn = false;

                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;
        if(!collision.collider.TryGetComponent<Enemy>(out Enemy enemy) && !collision.collider.TryGetComponent<Blob>(out Blob blob))
        {
            dashable = true;

            GameObject splashGo = Instantiate(splashPrefab, transform.position, Quaternion.identity);

            splashGo.transform.forward = collision.contacts[0].normal;
            Destroy(splashGo, 2f);

            //GameObject decalGo = Instantiate(decalPrefab, transform.position, Quaternion.identity);
            //decalGo.GetComponent<DecalProjector>().material = decalsMats[Random.Range(1, (decalsMats.Count - 1))];
        }
    }

    public void UpdateBlob()
    {
        gameObject.transform.localScale = new Vector3(minSize + sizePerBlob * blobNumber, minSize + sizePerBlob * blobNumber, minSize +sizePerBlob * blobNumber);
    }

    public void SpawnByShoot(Vector3 force)
    {
        collisionOn = false;
        rb.velocity = force;
     
        Invoke(nameof(ActiveCollision), timeForCollisionAfterShooted);

    }

    public void ActiveCollision()
    {
        collisionOn = true;
    }
}
