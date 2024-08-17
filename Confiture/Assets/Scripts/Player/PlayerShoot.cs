using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot parameters")]

    [SerializeField] LayerMask mouseLayer;
    [SerializeField] GameObject blobPrefab;

    [SerializeField] float minForce;
    [SerializeField] float maxForce;

    PlayerInput input;
    InputAction shootAction;

    private bool waitForRelease = false;
    private PlayerEntity player;

    void Start()
    {
        input = GetComponent<PlayerInput>();
        player = GetComponent<PlayerEntity>();

        shootAction = input.actions.FindAction("Shoot");
    }

    void Update()
    {
        if (shootAction.ReadValue<float>() > 0 && !waitForRelease)
        {
            waitForRelease = true;
            TryShoot();
        }
        else if (shootAction.ReadValue<float>() == 0 && waitForRelease)
        {
            waitForRelease = false;
        }
    }

    private void TryShoot()
    {
        if(player.blobNumber > player.minBlob) Shoot();
    }

    private void Shoot()
    {
        Vector3 mousePos = Input.mousePosition;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(mousePos).direction, out hit, 100000f, mouseLayer))
        {
            GameObject newBlob = Instantiate(blobPrefab, transform.position, Quaternion.identity);

            Blob blob = newBlob.GetComponent<Blob>();

            Vector3 force = (hit.point - transform.position).normalized * Mathf.Lerp(minForce, maxForce, player.blobRatio);
            
            blob.SpawnByShoot(force);

            player.TryRemoveBlobs(1);
        }
    }
}
