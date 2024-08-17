using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour
{
    [Header("Blob parameters")]

    [SerializeField] GameObject blobPrefab;
    [SerializeField] LayerMask blobMask;

    [SerializeField] int blobNumber = 1;
    [SerializeField] float sizePerBlob = .5f;

    [HideInInspector] public bool allreadySpawn = false;

    [SerializeField] private bool collisionOn = false;
    [HideInInspector] public Rigidbody rb;

    [SerializeField] float timeForCollisionAfterShooted = .5f;

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
    }

    public void UpdateBlob()
    {
        gameObject.transform.localScale = new Vector3(sizePerBlob * blobNumber, sizePerBlob * blobNumber, sizePerBlob * blobNumber);
    }

    public void SpawnByShoot(Vector3 force)
    {
        collisionOn = false;
        rb.velocity = force;
     
        Invoke(nameof(ActiveCollision), timeForCollisionAfterShooted);

    }

    private void ActiveCollision()
    {
        collisionOn = true;
    }
}
