using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Vector3 mouseWorldPosition = Vector3.zero;

    void Start()
    {
        input = GetComponent<PlayerInput>();
        player = GetComponent<PlayerEntity>();

        shootAction = input.actions.FindAction("Shoot");
    }

    void Update()
    {
        MousePosition();

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

    private void MousePosition()
    {
        Vector3 mousePos = Input.mousePosition;

        RaycastHit hit;

        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(mousePos).direction, out hit, 100000f, mouseLayer))
        //{
        //    mouseWorldPosition = hit.point;
        //}
        
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
    }

    private void TryShoot()
    {
        if(player.blobNumber > player.minBlob) Shoot();
    }

    private void Shoot()
    {
        GameObject newBlob = Instantiate(blobPrefab, transform.position, Quaternion.identity);

        Blob blob = newBlob.GetComponent<Blob>();

        Vector3 force = (mouseWorldPosition - transform.position).normalized * Mathf.Lerp(minForce, maxForce, player.blobRatio);

        blob.SpawnByShoot(force);

        CameraShake.instance.TriggerShake(.1f, .1f, 1f);

        player.TryRemoveBlobs(1);
    }
}
