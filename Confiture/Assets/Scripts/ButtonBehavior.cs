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
    private Animator ButtonAnimator;
    public UnityEvent ButtonActivated;

    public TextMeshProUGUI blobNumberText;

    public AudioClip openDoorClip;

    private void Awake()
    {
        blobNumberText.text = blobNumberRequiered.ToString();
        ButtonAnimator = GetComponent<Animator>();
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

                // Animation
                ButtonAnimator.SetTrigger("ButtonActivated");

                // SFX
                GameManager.instance.PlaySound(openDoorClip);

                // VFX
            }
        }
    }
}
