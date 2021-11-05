using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Vector2Int gridLocation;
    public GameObject tile;
    public GameObject obj;
    public bool isObjOrigin;
    public Vector3 objOrientation;
    public GameObject enemy;
}
