using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Followcursor : MonoBehaviour
{
    public Vector3 Offset;
    [SerializeField] TextMeshProUGUI BN_Text;
    public int BN_number;
    public PlayerEntity PE;

    public void Awake()
    {
        PE = GameObject.FindObjectOfType<PlayerEntity>();
    }
    public void Update()
    {
        Vector2 cursorPos = Input.mousePosition + Offset;
        transform.position = new Vector2(cursorPos.x, cursorPos.y);

        BN_Text.text = PE.blobNumber.ToString();
    }
}
