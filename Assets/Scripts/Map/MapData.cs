using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Room;

public class MapData
{
    public string name;
    public RoomData[,] map;
    public Vector2Int mapSize;
}

public class RoomData
{
    public string prefabName;
    public int slotOption;
    public string[] enemies;

    public RoomData(string prefabName, int slotOption, string[] enemies)
    {
        this.prefabName = prefabName;
        this.slotOption = slotOption;
        this.enemies = enemies;
    }
}
