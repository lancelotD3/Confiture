using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    public int blobNumber = 2;
    [HideInInspector] public float blobRatio = 0;

    [Header("Blob parameters")]
    [SerializeField] public int minBlob = 2;
    [SerializeField] int maxBlob = 10;
    [SerializeField] float minBlobSize;
    [SerializeField] float maxBlobSize;

    private void Awake()
    {
        UpdateBlob();
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

    private void RemoveBlobs(int number)
    {
        blobNumber -= number;

        UpdateBlob();
    }

    public void UpdateBlob()
    {
        blobRatio = (float)(blobNumber - 1) / (float)(maxBlob - 1);

        gameObject.transform.localScale = Vector3.Lerp(new Vector3(minBlobSize, minBlobSize, minBlobSize), new Vector3(maxBlobSize, maxBlobSize, maxBlobSize), blobRatio);
    }
}
