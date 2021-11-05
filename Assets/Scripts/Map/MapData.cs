using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Room;

public class MapData
{
    public string name;
    public RoomDataDeprecated[,] map;
    public Vector2Int mapSize;
}

public class RoomDataDeprecated
{
    public string prefabName;
    public int slotOption;
    public string[] enemies;

    public RoomDataDeprecated(string prefabName, int slotOption, string[] enemies)
    {
        this.prefabName = prefabName;
        this.slotOption = slotOption;
        this.enemies = enemies;
    }
}
