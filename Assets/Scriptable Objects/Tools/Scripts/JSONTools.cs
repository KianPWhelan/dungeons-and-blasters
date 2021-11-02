using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Map;
using Newtonsoft.Json;

public class JSONTools : ScriptableObject
{
    [SerializeField]
    private string fileName;

    public void SaveGameObjectsToJSONTextFile()
    {
        PrefabSaver.SaveEnemyPrefabBalanceData(fileName);
    }

    public void LoadEnemyPrefabBalanceDataFromTextFile()
    {
        PrefabLoader.LoadEnemyPrefabBalanceData(fileName);
    }

    public static string SaveMapData(RoomContainer[,] rooms, Vector2Int mapSize)
    {
        MapData mapData = new MapData();
        mapData.map = new RoomData[mapSize.x, mapSize.y];
        mapData.mapSize = mapSize;

        for(int i = 0; i < mapSize.x; i++)
        {
            for(int j = 0; j < mapSize.y; j++)
            {
                if (rooms[i, j] != null)
                {
                    mapData.map[i, j] = new RoomData(rooms[i, j].room.name);
                }

                else
                {
                    mapData.map[i, j] = new RoomData("empty");
                }
            }
        }

        string mapJson = JsonConvert.SerializeObject(mapData);

        return mapJson;
    }

    public static MapData LoadMapData(string jsonMap)
    {
        MapData mapData = JsonConvert.DeserializeObject<MapData>(jsonMap);

        return mapData;
    }
}
