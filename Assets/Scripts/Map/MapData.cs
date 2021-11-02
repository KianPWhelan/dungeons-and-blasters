using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public RoomData[,] map;
    public Vector2Int mapSize;
}

public class RoomData
{
    public string prefabName;

    public RoomData(string prefabName)
    {
        this.prefabName = prefabName;
    }
}
