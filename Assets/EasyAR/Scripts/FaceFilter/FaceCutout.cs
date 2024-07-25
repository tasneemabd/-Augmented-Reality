﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaceCutout : MonoBehaviour
{
    // [SerializeField] bool collect = false;
    [SerializeField] List<int> mouseTriangles = new List<int>();
    [SerializeField] List<int> leftEyeTriangles = new List<int>();
    [SerializeField] List<int> rightEyeTriangles = new List<int>();

    public List<int> allTrianglesToCutout { get; private set; }

    void Awake()
    {
        allTrianglesToCutout = mouseTriangles.Concat(leftEyeTriangles).Concat(rightEyeTriangles).ToList();
    }
}
