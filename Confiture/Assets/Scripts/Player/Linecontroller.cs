using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linecontroller : MonoBehaviour
{
    private LineRenderer Lr;
    private Transform[] points;

    private void Awake()
    {
        Lr = GetComponent<LineRenderer>();
    }

    public void SetupLine(Transform[] points)
    {
        Lr.positionCount = points.Length;
        this.points = points;
    }

    private void Update()
    {
        for(int i = 0; i < points.Length; i++)
        {
            Lr.SetPosition(i, points[i].position);
        }
    }
}
