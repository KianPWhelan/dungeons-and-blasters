using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [SerializeField]
    public Vector2Int tilesOccupied = new Vector2Int(1, 1);
    public bool isStartingPoint;
    public bool isExitPoint;
    public bool doNotUse;
}
