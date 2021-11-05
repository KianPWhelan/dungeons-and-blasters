using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData
{
    public Vector2Int gridLocation;
    public string obj;
    public string enemy;

    public NodeData(Vector2Int gridLocation, string obj, string enemy)
    {
        this.gridLocation = gridLocation;
        this.obj = obj;
        this.enemy = enemy;
    }
}
