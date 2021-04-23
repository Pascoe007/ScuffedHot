using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDistance : MonoBehaviour
{
    public float distance;
    public Terrain terrain;
    void Start()
    {
        terrain.treeDistance = distance;
    }
}
