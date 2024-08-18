using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonBehavior : MonoBehaviour
{
    [SerializeField]
    [Range(1, 6)] private int blobNumberRequiered;
    private PlayerEntity playerEntity;

    public UnityEvent ButtonActivated;

    public TextMeshProUGUI blobNumberText;

    private void Awake()
    {
        blobNumberText.text = blobNumberRequiered.ToString();

        playerEntity = GameObject.FindObjectOfType<PlayerEntity>();
    }
    private void Update()
    {
        
        if (playerEntity.blobNumber >= blobNumberRequiered)
        {
            blobNumberText.color = Color.green;
        }
        else
        {
            blobNumberText.color = Color.red;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerEntity playerEntityScript = other.GetComponent<PlayerEntity>();

        if (playerEntityScript != null)
        {
            if(playerEntityScript.blobNumber >= blobNumberRequiered)
            {
                ButtonActivated?.Invoke();
            }
        }
    }
}
