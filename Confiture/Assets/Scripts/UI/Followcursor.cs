using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Followcursor : MonoBehaviour
{
    public Vector3 Offset;
    public int BN_number;
    public PlayerEntity PE;
    public GameObject cursor;

    public Image BN1;
    public Image BN2;
    public Image BN3;

    public void Awake()
    {
        PE = GameObject.FindObjectOfType<PlayerEntity>();
    }
    public void Update()
    {
        Vector2 cursorPos = Input.mousePosition + Offset;
        transform.position = new Vector2(cursorPos.x, cursorPos.y);

        if(PE ==  null)
        {
            PE = GameObject.FindObjectOfType<PlayerEntity>();
        }

        BN_number = PE.blobNumber;

        switch (BN_number)
        {
            case 1:
                BN1.gameObject.SetActive(false);
                BN2.gameObject.SetActive(false);
                BN3.gameObject.SetActive(false);
                break;
            case 2:
                BN1.gameObject.SetActive(true);
                BN2.gameObject.SetActive(false);
                BN3.gameObject.SetActive(false);
                break;
            case 3:
                BN1.gameObject.SetActive(true);
                BN2.gameObject.SetActive(true);
                BN3.gameObject.SetActive(false);
                break;
            default:
                Debug.LogWarning("currentValue doit être entre 1 et 3 !");
                break;
        }
    }
}
