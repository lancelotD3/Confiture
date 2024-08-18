using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_drawlineclass : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private Linecontroller line;

    private void Start()
    {
        line.SetupLine(points);
    }
}
