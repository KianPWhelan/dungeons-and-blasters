using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData
{
    public Vector2Int gridLocation;
    public string obj;
    public bool isObjOrigin;
    public Vector3 objOrientation;
    public string enemy;

    public NodeData(Vector2Int gridLocation, string obj, bool isObjOrigin, Vector3 objOrientation, string enemy)
    {
        this.gridLocation = gridLocation;
        this.obj = obj;
        this.isObjOrigin = isObjOrigin;
        this.objOrientation = objOrientation;
        this.enemy = enemy;
    }
}
